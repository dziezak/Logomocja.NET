namespace Logomocja.Core;

public readonly record struct Position(float X, float Y)
{
    public Position Add(Position o) => new(X + o.X, Y + o.Y);
}