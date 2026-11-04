namespace Logomocja.Core;

public interface IRenderer
{
    void DrawLine(Position a, Position b, Color c, float thickness);
    void DrawCircle(Position center, float radius, Color c, float thickness);
    void FillPolygon(IReadOnlyList<Position> pts, Color fill);
}