namespace Logomocja.Core;

public class Turtle
{
   public string Name { get; }
   public Position Pos { get; private set; } = new(0, 0);
   public float Heading { get; private set; }
   public bool IsPenDown { get; set; } = true;
   public Color PenColor { get; set; } = Color.Black;
   public float PenSize { get; set; } = 1f;
   public bool IsVisible { get; set; } = true;
   private List<Position>? _fillPath;
   public Color FillColor { get; set; } = Color.Black;

   public Turtle(string name) => Name = name;

   public Drawable? Forward(float d)
   {
      float rad = Heading * MathF.PI / 180f;
      return MoveTo(new Position(Pos.X + d * MathF.Sin(rad), Pos.Y + d * MathF.Cos(rad)));
   }

   private static float Normalize(float angle)
   {
      angle %= 360f;
      if (angle < 0f) angle += 360f;
      if (angle >= 360) angle = 0f;
      return angle;
   }

   public void Right(float deg) => Heading = Normalize(Heading + deg);
   public void Left(float deg) => Right(-deg);
   
   private Drawable? MoveTo(Position target)
   {
      var seg = IsPenDown ? new LineSegment(Name, Pos, target, PenColor, PenSize) : null;
      _fillPath?.Add(target);
      Pos = target;
      return seg;
   }

   public void BeginFill() => _fillPath = new List<Position>() { Pos };

   public Drawable? EndFill()
   {
      var path = _fillPath;
      _fillPath = null;
      return path is { Count: > 2 }
         ? new FilledPolygon(Name, path, FillColor)
         : null;
   }
}