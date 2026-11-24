namespace Logomocja.Language.Lexing;

public record class SourcePosition(int Line, int Column)
{
    public static SourcePosition Start => new(1, 1);
    public override string ToString() => $"linia {Line}, kol. {Column}";

}