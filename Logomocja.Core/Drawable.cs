namespace Logomocja.Core;

public abstract class Drawable
{
    public string TurtleName { get; }
    public Color Color { get; }
    public float Thickness { get; }

    protected Drawable(string turtle, Color c, float t)
    {
        TurtleName = turtle;
        Color = c;
        Thickness = t;
    }

    public abstract void Draw(IRenderer r);
}