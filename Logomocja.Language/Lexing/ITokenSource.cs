namespace Logomocja.Language.Lexing;

public interface ITokenSource
{
    Token NextToken();
}

public sealed record LexerOptions(
    int MaxNumberLength = 64,
    int MaxIdentifierLength = 64,
    int MaxStringLength = 10_000)
{
    public static LexerOptions Default { get; } = new();
}