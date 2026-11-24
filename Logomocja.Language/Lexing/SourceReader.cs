namespace Logomocja.Language.Lexing;

public sealed class SourceReader : IDisposable
{
    private readonly TextReader _reader;
    private readonly bool _leaveOpen;
    private int _line = 1;
    private int _column = 1;

    public SourceReader(TextReader reader, bool leaveOpen = false)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _leaveOpen = leaveOpen;
    }

    public SourcePosition Position => new(_line, _column);

    public bool IsAtEnd => _reader.Peek() == -1;

    public int Peek() => _reader.Peek();

    public int Read()
    {
        int current = _reader.Read();
        if (current == -1)
        {
            return -1;
        }

        if (current == '\r')
        {
            if (_reader.Peek() == '\n')
            {
                _reader.Read();
            }

            _line++;
            _column = 1;
            return '\n';
        }

        _column++;
        return current;
    }

    public bool Match(char expected)
    {
        if (_reader.Peek() != expected)
        {
            return false;
        }

        Read();
        return true;
    }

    public void Dispose()
    {
        if (!_leaveOpen)
        {
            _reader.Dispose();
        }
    }
}