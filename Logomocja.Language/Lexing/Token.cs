namespace Logomocja.Language.Lexing;

public sealed record Token(TokenType Type, object? Value, SourcePosition Position)
{
    public Token(TokenType type, SourcePosition position) : this(type, null, position)
    {
    }

    public override string ToString()
    {
        if (Value is null)
        {
            return $"{Type} ({Position})";
        }

        return $"{Type} [{Value}] ({Position})";
    }
}