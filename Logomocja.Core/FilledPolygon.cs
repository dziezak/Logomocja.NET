namespace Logomocja.Core;

public sealed class FilledPolygon : Drawable
{
    public IReadOnlyList<Position> Points { get; }

    public FilledPolygon(string turtle, IEnumerable<Position> points, Color fill)
        : base(turtle, fill, 0f)
        => Points = points.ToArray();

    public override void Draw(IRenderer r) => r.FillPolygon(Points, Color);
}