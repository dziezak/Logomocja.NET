namespace Logomocja.Language.Lexing;

public sealed class LexerException : Exception
{
    public SourcePosition Position { get; }

    public LexerException(string message, SourcePosition position) : base($"Błąd leksykalny ({position}): {message}")
    {
        Position = position;
    }
}