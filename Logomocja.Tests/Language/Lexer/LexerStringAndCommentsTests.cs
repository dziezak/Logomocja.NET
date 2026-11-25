using Logomocja.Language.Lexing;
using Xunit;

namespace Logomocja.Tests.Language.Lexer;

public sealed class LexerStringsAndCommentsTests
{
    private static List<Token> TokenizeAll(string source)
    {
        using var lexer = Logomocja.Language.Lexing.Lexer.FromString(source);
        var tokens = new List<Token>();

        while (true)
        {
            Token token = lexer.NextToken();
            tokens.Add(token);
            if (token.Type == TokenType.Eof)
            {
                break;
            }
        }

        return tokens;
    }

    [Fact]
    public void DecodesEscapeSequencesInStrings()
    {
        string input = """
                       "linia1\nlinia2\t\"cytat\"\\ukosnik\{klamra\}"
                       """;
        List<Token> tokens = TokenizeAll(input);

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("linia1\nlinia2\t\"cytat\"\\ukosnik{klamra}", tokens[0].Value);
    }

    [Fact]
    public void TokenizesInterpolatedStringWithExpression()
    {
        string input = """$"wynik: {a + b}, ok" """;
        List<Token> tokens = TokenizeAll(input);

        Assert.Equal(10, tokens.Count);
        Assert.Equal(TokenType.InterpolatedStringStart, tokens[0].Type);
        Assert.Equal(new Token(TokenType.InterpolatedText, "wynik: ", new SourcePosition(1, 3)), tokens[1]);
        Assert.Equal(TokenType.LBrace, tokens[2].Type);
        Assert.Equal(new Token(TokenType.Identifier, "a", new SourcePosition(1, 11)), tokens[3]);
        Assert.Equal(TokenType.Plus, tokens[4].Type);
        Assert.Equal(new Token(TokenType.Identifier, "b", new SourcePosition(1, 15)), tokens[5]);
        Assert.Equal(TokenType.RBrace, tokens[6].Type);
        Assert.Equal(new Token(TokenType.InterpolatedText, ", ok", new SourcePosition(1, 17)), tokens[7]);
        Assert.Equal(TokenType.InterpolatedStringEnd, tokens[8].Type);
        Assert.Equal(TokenType.Eof, tokens[9].Type);
    }

    [Fact]
    public void TokenizesNestedInterpolatedStrings()
    {
        string input = """$"zewn: {$"wewn={x}"}" """;
        List<Token> tokens = TokenizeAll(input);

        TokenType[] expected =
        [
            TokenType.InterpolatedStringStart, // $"
            TokenType.InterpolatedText,        // "zewn: "
            TokenType.LBrace,                  // {
            TokenType.InterpolatedStringStart, // $"
            TokenType.InterpolatedText,        // "wewn="
            TokenType.LBrace,                  // {
            TokenType.Identifier,              // x
            TokenType.RBrace,                  // }
            TokenType.InterpolatedStringEnd,   // "
            TokenType.RBrace,                  // }
            TokenType.InterpolatedStringEnd,   // "
            TokenType.Eof
        ];

        Assert.Equal(expected, tokens.Select(t => t.Type));
    }

    [Fact]
    public void SkipsCommentsAndTracksMixedNewlinesAccurately()
    {
        string input = "# komentarz 1 #\r\nstart # wielolinijkowy\nkomentarz #\n{\r\n  np(10)\r}";
        List<Token> tokens = TokenizeAll(input);

        Assert.Equal(8, tokens.Count);
        Assert.Equal(new Token(TokenType.Start, new SourcePosition(2, 1)), tokens[0]);
        Assert.Equal(new Token(TokenType.LBrace, new SourcePosition(4, 1)), tokens[1]);
        Assert.Equal(new Token(TokenType.Identifier, "np", new SourcePosition(5, 3)), tokens[2]);
        Assert.Equal(new Token(TokenType.LParen, new SourcePosition(5, 5)), tokens[3]);
        Assert.Equal(new Token(TokenType.Number, 10.0, new SourcePosition(5, 6)), tokens[4]);
        Assert.Equal(new Token(TokenType.RParen, new SourcePosition(5, 8)), tokens[5]);
        Assert.Equal(new Token(TokenType.RBrace, new SourcePosition(6, 1)), tokens[6]);
    }
}