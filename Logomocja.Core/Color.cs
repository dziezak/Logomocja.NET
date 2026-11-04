namespace Logomocja.Core;

public readonly record struct Color(byte R, byte G, byte B, byte A = 255)
{
    public static readonly Color White = new(255, 255, 255);
    public static readonly Color Black = new(0, 0, 0);
}