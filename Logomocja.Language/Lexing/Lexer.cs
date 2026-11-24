using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Logomocja.Language.Lexing;

public sealed class Lexer : ITokenSource, IDisposable
{
    private static readonly Dictionary<string, TokenType> Keywords = new(StringComparer.Ordinal)
    {
        ["start"] = TokenType.Start,
        ["oto"] = TokenType.Oto,
        ["jeżeli"] = TokenType.Jeżeli,
        ["inaczej"] = TokenType.Inaczej,
        ["powtórz"] = TokenType.Powtórz,
        ["wynik"] = TokenType.Wynik,
        ["lub"] = TokenType.Lub,
        ["oraz"] = TokenType.Oraz,
        ["nie"] = TokenType.Nie,
        ["prawda"] = TokenType.Prawda,
        ["fałsz"] = TokenType.Fałsz,
        ["Run"] = TokenType.Run,
        ["brak"] = TokenType.Brak,
        ["spróbuj"] = TokenType.Spróbuj,
        ["złap"] = TokenType.Złap,
        ["rzuć"] = TokenType.Rzuć
    };

    private readonly SourceReader _reader;
    private readonly LexerOptions _options;
    private readonly StringBuilder _buffer = new();

    private readonly Stack<int> _interpolationBraceDepths = new();
    private bool _pendingInterpolatedText;

    public Lexer(TextReader reader, LexerOptions? options = null, bool leaveOpen = false)
    {
        _reader = new SourceReader(reader, leaveOpen);
        _options = options ?? LexerOptions.Default;
    }

    public static Lexer FromString(string source, LexerOptions? options = null)
    {
        return new(new StringReader(source), options, leaveOpen: false);
    }

    public static Lexer FromFile(string filePath, LexerOptions? options = null)
    {
        return new(new StreamReader(filePath, Encoding.UTF8), options, leaveOpen: false);
    }

    public Token NextToken()
    {
        if (_pendingInterpolatedText)
        {
            return ReadInterpolatedSegment();
        }

        SkipTrivia();

        SourcePosition startPos = _reader.Position;
        int peek = _reader.Peek();

        if (peek == -1)
        {
            if (_interpolationBraceDepths.Count > 0)
            {
                throw new LexerException("Niezamknięty tekst interpolowany.", startPos);
            }

            return new Token(TokenType.Eof, startPos);
        }

        char c = (char)peek;

        if (TryReadStringOrInterpolationStart(c, startPos, out Token? stringToken))
        {
            return stringToken!;
        }

        if (TryReadNumber(c, startPos, out Token? numberToken))
        {
            return numberToken!;
        }

        if (TryReadIdentifierOrKeyword(c, startPos, out Token? idToken))
        {
            return idToken!;
        }

        if (TryReadOperatorOrDelimiter(c, startPos, out Token? opToken))
        {
            return opToken!;
        }

        throw new LexerException($"Nierozpoznany znak '{c}'.", startPos);
    }
    
    private void SkipTrivia()
    {
        while (true)
        {
            int peek = _reader.Peek();
            if (peek == -1)
            {
                return;
            }

            char c = (char)peek;

            if (c is ' ' or '\t' or '\n' or '\r')
            {
                _reader.Read();
                continue;
            }

            if (c == '#')
            {
                SkipComment();
                continue;
            }

            return;
        }
    }
    
    private bool TryReadStringOrInterpolationStart(char current, SourcePosition startPos, out Token? token)
    {
        if (current == '"')
        {
            _reader.Read();
            string value = ReadStringContent(startPos, stopAtBrace: false, out _);
            token = new Token(TokenType.String, value, startPos);
            return true;
        }

        if (current == '$')
        {
            _reader.Read();
            if (!_reader.Match('"'))
            {
                throw new LexerException("Oczekiwano '\"' po znaku '$'.", startPos);
            }

            _interpolationBraceDepths.Push(0);
            _pendingInterpolatedText = true;
            token = new Token(TokenType.InterpolatedStringStart, startPos);
            return true;
        }

        token = null;
        return false;
    }
    
    private void SkipComment()
    {
        SourcePosition commentStart = _reader.Position;
        _reader.Read(); 

        while (true)
        {
            int ch = _reader.Read();
            if (ch == -1)
            {
                throw new LexerException("Niezamknięty komentarz rozpoczęty tutaj.", commentStart);
            }

            if (ch == '#')
            {
                return;
            }
        }
    }
    
    private Token ReadInterpolatedSegment()
    {
        SourcePosition segmentStart = _reader.Position;
        int peek = _reader.Peek();

        if (peek == '"')
        {
            _reader.Read();
            _interpolationBraceDepths.Pop();
            _pendingInterpolatedText = false;
            return new Token(TokenType.InterpolatedStringEnd, segmentStart);
        }

        if (peek == '{')
        {
            _reader.Read();
            int depth = _interpolationBraceDepths.Pop();
            _interpolationBraceDepths.Push(depth + 1);
            _pendingInterpolatedText = false;
            return new Token(TokenType.LBrace, segmentStart);
        }

        string text = ReadStringContent(segmentStart, stopAtBrace: true, out _);
        return new Token(TokenType.InterpolatedText, text, segmentStart);
    }
    
    private string ReadStringContent(SourcePosition startPos, bool stopAtBrace, out char terminator)
    {
        _buffer.Clear();

        while (true)
        {
            int peek = _reader.Peek();
            if (peek == -1)
            {
                throw new LexerException("Niezamknięty tekst rozpoczęty tutaj.", startPos);
            }

            char c = (char)peek;

            if (c == '"')
            {
                if (!stopAtBrace)
                {
                    _reader.Read();
                }

                terminator = '"';
                return _buffer.ToString();
            }

            if (stopAtBrace && c == '{')
            {
                terminator = '{';
                return _buffer.ToString();
            }

            if (stopAtBrace && c == '}')
            {
                throw new LexerException("Nieoczekiwany znak '}' w tekście interpolowanym (użyj '\\}' aby zapisać klamrę).", _reader.Position);
            }

            _reader.Read();

            if (c == '\\')
            {
                SourcePosition escapePos = _reader.Position;
                int escaped = _reader.Read();
                if (escaped == -1)
                {
                    throw new LexerException("Niezamknięty tekst po znaku ucieczki '\\'.", startPos);
                }

                char resolved = (char)escaped switch
                {
                    '"' => '"',
                    '\\' => '\\',
                    'r' => '\r',
                    'n' => '\n',
                    't' => '\t',
                    '{' => '{',
                    '}' => '}',
                    var bad => throw new LexerException($"Nieznana sekwencja ucieczki '\\{bad}'.", escapePos)
                };

                AppendToStringBuffer(resolved, startPos);
            }
            else
            {
                AppendToStringBuffer(c, startPos);
            }
        }
    }

    private void AppendToStringBuffer(char c, SourcePosition startPos)
    {
        if (_buffer.Length >= _options.MaxStringLength)
        {
            throw new LexerException($"Przekroczono maksymalną długość tekstu ({_options.MaxStringLength} znaków.",
                startPos);
        }

        _buffer.Append(c);
    }

    private bool TryReadNumber(char current, SourcePosition startPos, out Token? token)
    {
        if (!char.IsAsciiDigit(current))
        {
            token = null;
            return false;
        }

        _buffer.Clear();
        bool hasDecimalPoint = false;

        while (true)
        {
            int peek = _reader.Peek();
            if (peek != -1 && char.IsAsciiDigit((char)peek))
            {
                if (_buffer.Length >= _options.MaxNumberLength)
                {
                    throw new LexerException(
                        $"Przekroczono maksymalną długość literału liczbowego ({_options.MaxNumberLength} znaków).",
                        startPos);
                }

                _buffer.Append((char)_reader.Read());
                continue;
            }

            if (!hasDecimalPoint && peek == '.')
            {
                if (_buffer.Length >= _options.MaxNumberLength)
                {
                    throw new LexerException(
                        $"Przekroczono maksymalną długość literału liczbowego ({_options.MaxNumberLength} znaków).",
                        startPos);
                }

                _buffer.Append((char)_reader.Read());
                hasDecimalPoint = true;

                int afterDot = _reader.Peek();
                if (afterDot == -1 || !char.IsAsciiDigit((char)afterDot))
                {
                    throw new LexerException("Oczekiwano cyfry po kropce dziesiętnej.", _reader.Position);
                }

                continue;
            }

            break;
        }

        string raw = _buffer.ToString();
        if (!double.TryParse(raw, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double value) ||
            double.IsInfinity(value))
        {
            throw new LexerException($"Nieprawidłowy lub zbyt duży literał liczbowy '{raw}' .", startPos);
        }

        token = new Token(TokenType.Number, value, startPos);
        return true;
    }

    private bool TryReadIdentifierOrKeyword(char current, SourcePosition startPos, out Token? token)
    {
        if (!char.IsLetter(current))
        {
            token = null;
            return false;
        }

        _buffer.Clear();

        while (true)
        {
            int peek = _reader.Peek();
            if (peek == -1)
            {
                break;
            }

            char c = (char)peek;
            if (!char.IsLetter(c) && !char.IsAsciiDigit(c) && c != '_')
            {
                break;
            }

            if (_buffer.Length >= _options.MaxIdentifierLength)
            {
                throw new LexerException($"Przekroczono maksymalną długość identyfikatora ({_options.MaxIdentifierLength} znaków).", startPos);
            }

            _buffer.Append((char)_reader.Read());
        }

        string word = _buffer.ToString();
        if (Keywords.TryGetValue(word, out TokenType keywordType))
        {
            token = new Token(keywordType, startPos);
            return true;
        }

        token = new Token(TokenType.Identifier, word, startPos);
        return true;
    }
    
    private bool TryReadOperatorOrDelimiter(char current, SourcePosition startPos, out Token? token)
    {
        switch (current)
        {
            case '+':
                _reader.Read();
                token = new Token(TokenType.Plus, startPos);
                return true;

            case '-':
                _reader.Read();
                token = new Token(TokenType.Minus, startPos);
                return true;

            case '*':
                _reader.Read();
                token = new Token(TokenType.Star, startPos);
                return true;

            case '/':
                _reader.Read();
                token = new Token(TokenType.Slash, startPos);
                return true;

            case '%':
                _reader.Read();
                token = new Token(TokenType.Percent, startPos);
                return true;

            case '=':
                _reader.Read();
                token = new Token(TokenType.Equal, startPos);
                return true;

            case '.':
                _reader.Read();
                token = new Token(TokenType.Dot, startPos);
                return true;

            case '(':
                _reader.Read();
                token = new Token(TokenType.LParen, startPos);
                return true;

            case ')':
                _reader.Read();
                token = new Token(TokenType.RParen, startPos);
                return true;

            case ',':
                _reader.Read();
                token = new Token(TokenType.Comma, startPos);
                return true;

            case '{':
                _reader.Read();
                if (_interpolationBraceDepths.Count > 0)
                {
                    int depth = _interpolationBraceDepths.Pop();
                    _interpolationBraceDepths.Push(depth + 1);
                }

                token = new Token(TokenType.LBrace, startPos);
                return true;

            case '}':
                _reader.Read();
                if (_interpolationBraceDepths.Count > 0)
                {
                    int depth = _interpolationBraceDepths.Pop() - 1;
                    _interpolationBraceDepths.Push(depth);
                    if (depth == 0)
                    {
                        _pendingInterpolatedText = true;
                    }
                }

                token = new Token(TokenType.RBrace, startPos);
                return true;

            case '<':
                _reader.Read();
                if (_reader.Match('-'))
                {
                    token = new Token(TokenType.Assign, startPos);
                }
                else if (_reader.Match('>'))
                {
                    token = new Token(TokenType.NotEqual, startPos);
                }
                else if (_reader.Match('='))
                {
                    token = new Token(TokenType.LessOrEqual, startPos);
                }
                else
                {
                    token = new Token(TokenType.Less, startPos);
                }

                return true;

            case '>':
                _reader.Read();
                token = new Token(_reader.Match('=') ? TokenType.GreaterOrEqual : TokenType.Greater, startPos);
                return true;

            case '?':
                _reader.Read();
                if (_reader.Match('.'))
                {
                    token = new Token(TokenType.NullConditionalDot, startPos);
                    return true;
                }

                if (_reader.Match('?'))
                {
                    token = new Token(TokenType.NullCoalescing, startPos);
                    return true;
                }

                throw new LexerException("Oczekiwano '.' lub '?' po znaku '?'.", startPos);

            default:
                token = null;
                return false;
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}