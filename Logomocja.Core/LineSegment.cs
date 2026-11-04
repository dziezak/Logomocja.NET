namespace Logomocja.Core;

public sealed class LineSegment : Drawable
{
    public Position Start { get; }
    public Position End { get; }

    public LineSegment(string turtle, Position s, Position e, Color c, float t) :
        base(turtle, c, t) => (Start, End) = (s, e);


    public override void Draw(IRenderer r) => r.DrawLine(Start, End, Color, Thickness);
}