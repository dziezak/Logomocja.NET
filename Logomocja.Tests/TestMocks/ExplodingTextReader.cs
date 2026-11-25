namespace Logomocja.Tests.TestMocks;
public sealed class ExplodingTextReader : TextReader
{
    private readonly string _data;
    private readonly int _maxAllowedReads;
    private int _index;

    public int ReadsCount => _index;

    public ExplodingTextReader(string data, int maxAllowedReads)
    {
        _data = data;
        _maxAllowedReads = maxAllowedReads;
    }

    public override int Peek()
    {
        if (_index >= _maxAllowedReads)
        {
            throw new InvalidOperationException("Lekser przeczytał za daleko — naruszono leniwą tokenizację!");
        }

        if (_index < _data.Length)
        {
            return _data[_index];
        }
        return -1;
    }

    public override int Read()
    {
        if (_index >= _maxAllowedReads)
        {
            throw new InvalidOperationException("Lekser przeczytał za daleko — naruszono leniwą tokenizację!");
        }

        if (_index < _data.Length)
        {
            return _data[_index++];
        }
        return -1;
    }
}