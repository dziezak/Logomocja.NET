using Logomocja.Core;
using Xunit;

namespace Logomocja.Tests;

public sealed class TestRenderer : IRenderer
{
    public record LineCall(Position A, Position B, Color Color, float Thickness);
    public record TurtleCall(Position Pos, float Heading, Color Color);

    public List<string> Order { get; } = new(); 
    public List<LineCall> Lines { get; } = new();
    public List<IReadOnlyList<Position>> Polygons { get; } = new();
    public List<TurtleCall> Turtles { get; } = new();

    public void DrawLine(Position a, Position b, Color c, float thickness)
    {
        Order.Add("Line");
        Lines.Add(new LineCall(a, b, c, thickness));
    }

    public void DrawCircle(Position center, float radius, Color c, float thickness) => Order.Add("Circle");

    public void FillPolygon(IReadOnlyList<Position> pts, Color fill)
    {
        Order.Add("Fill");
        Polygons.Add(pts);
    }

    public void DrawTurtle(Position pos, float heading, Color c)
    {
        Order.Add("Turtle");
        Turtles.Add(new TurtleCall(pos, heading, c));
    }
}

internal static class Assertions
{
    public static void Near(Position expected, Position actual, float eps = 1e-3f)
    {
        Assert.True(
            MathF.Abs(expected.X - actual.X) < eps && MathF.Abs(expected.Y - actual.Y) < eps,
            $"Oczekiwano ({expected.X}, {expected.Y}), jest ({actual.X}, {actual.Y})");
    }

    public static void Near(float expected, float actual, float eps = 1e-3f) =>
        Assert.True(MathF.Abs(expected - actual) < eps, $"Oczekiwano {expected}, jest {actual}");
}

public class TurtleTests
{
    private static readonly Color Red = new(255, 0, 0);

    [Fact]
    public void NewTurtle_StartsAtOriginFacingUp_PenDown()
    {
        var t = new Turtle("a");

        Assertions.Near(new Position(0, 0), t.Pos);
        Assertions.Near(0f, t.Heading);
        Assert.True(t.IsPenDown);
        Assert.True(t.IsVisible);
    }

    [Fact]
    public void Forward_AtHeadingZero_MovesUp()
    {
        var t = new Turtle("a");

        var seg = Assert.IsType<LineSegment>(t.Forward(100));

        Assertions.Near(new Position(0, 0), seg.Start);
        Assertions.Near(new Position(0, 100), seg.End);
        Assertions.Near(new Position(0, 100), t.Pos);
    }

    [Fact]
    public void Forward_AfterRight90_MovesRight()
    {
        var t = new Turtle("a");
        t.Right(90);

        t.Forward(100);

        Assertions.Near(new Position(100, 0), t.Pos);
    }

    [Fact]
    public void Back_MovesOppositeToHeading()
    {
        var t = new Turtle("a");

        t.Right(180);
        t.Forward(50);

        Assertions.Near(new Position(0, -50), t.Pos);
        Assertions.Near(180f, t.Heading);
    }

    [Fact]
    public void Forward_SegmentCarriesPenColorSizeAndTurtleName()
    {
        var t = new Turtle("a") { PenColor = Red, PenSize = 3f };

        var seg = Assert.IsType<LineSegment>(t.Forward(10));

        Assert.Equal(Red, seg.Color);
        Assert.Equal(3f, seg.Thickness);
        Assert.Equal("a", seg.TurtleName);
    }

    [Fact]
    public void Forward_PenUp_ReturnsNullButStillMoves()
    {
        var t = new Turtle("a") { IsPenDown = false };

        var drawn = t.Forward(50);

        Assert.Null(drawn);
        Assertions.Near(new Position(0, 50), t.Pos);
    }

    [Theory]
    [InlineData(90, 90)]
    [InlineData(360, 0)]
    [InlineData(450, 90)]
    [InlineData(-90, 270)]
    [InlineData(-10, 350)]
    [InlineData(-370, 350)]
    public void Right_NormalizesHeadingToZero360(float delta, float expected)
    {
        var t = new Turtle("a");

        t.Right(delta);

        Assertions.Near(expected, t.Heading);
        Assert.InRange(t.Heading, 0f, 359.999f);
    }

    [Fact]
    public void Left_IsNegativeRight()
    {
        var t = new Turtle("a");

        t.Left(90);

        Assertions.Near(270f, t.Heading);
    }

    [Fact]
    public void Square_ReturnsToStart_AndProducesFourSegments()
    {
        var t = new Turtle("a");
        var segments = new List<Drawable>();

        for (int i = 0; i < 4; i++)
        {
            var d = t.Forward(100);
            if (d is not null) segments.Add(d);
            t.Right(90);
        }

        Assert.Equal(4, segments.Count);
        Assertions.Near(new Position(0, 0), t.Pos);
        Assertions.Near(0f, t.Heading);
    }

    [Fact]
    public void ManyTurns_DoNotDriftHeadingOutOfRange()
    {
        var t = new Turtle("a");

        for (int i = 0; i < 10_000; i++) t.Right(7.3f);

        Assert.InRange(t.Heading, 0f, 359.999f);
    }


    [Fact]
    public void Fill_Square_ProducesPolygonWithAllCorners()
    {
        var t = new Turtle("a") { FillColor = Red };

        t.BeginFill();
        for (int i = 0; i < 4; i++)
        {
            t.Forward(100);
            t.Right(90);
        }
        var poly = Assert.IsType<FilledPolygon>(t.EndFill());

        Assert.Equal(5, poly.Points.Count);
        Assertions.Near(new Position(0, 0), poly.Points[0]);
        Assertions.Near(new Position(0, 100), poly.Points[1]);
        Assertions.Near(new Position(100, 100), poly.Points[2]);
        Assertions.Near(new Position(100, 0), poly.Points[3]);
        Assertions.Near(new Position(0, 0), poly.Points[4]);
        Assert.Equal(Red, poly.Color);
        Assert.Equal("a", poly.TurtleName);
    }

    [Fact]
    public void Fill_CollectsPointsEvenWithPenUp()
    {
        var t = new Turtle("a") { IsPenDown = false };

        t.BeginFill();
        t.Forward(100);
        t.Right(90);
        t.Forward(100);
        var poly = Assert.IsType<FilledPolygon>(t.EndFill());

        Assert.Equal(3, poly.Points.Count);
    }

    [Fact]
    public void EndFill_WithoutBeginFill_ReturnsNull()
    {
        var t = new Turtle("a");

        Assert.Null(t.EndFill());
    }

    [Fact]
    public void EndFill_WithTwoPoints_ReturnsNull_BecauseItIsALineNotAnArea()
    {
        var t = new Turtle("a");

        t.BeginFill();
        t.Forward(100);

        Assert.Null(t.EndFill());
    }

    [Fact]
    public void EndFill_ResetsState_SecondEndFillReturnsNull()
    {
        var t = new Turtle("a");
        t.BeginFill();
        t.Forward(10);
        t.Right(90);
        t.Forward(10);
        Assert.NotNull(t.EndFill());

        Assert.Null(t.EndFill());
    }

    [Fact]
    public void Polygon_IsNotAffectedByLaterMovement()
    {
        var t = new Turtle("a");
        t.BeginFill();
        t.Forward(10);
        t.Right(90);
        t.Forward(10);
        var poly = Assert.IsType<FilledPolygon>(t.EndFill());

        t.Forward(500);

        Assert.Equal(3, poly.Points.Count);
    }
}

public class WorldTests
{
    [Fact]
    public void CreateTurtle_Duplicate_Throws()
    {
        var w = new World();
        w.CreateTurtle("a");

        Assert.Throws<DuplicateTurtleException>(() => w.CreateTurtle("a"));
    }

    [Fact]
    public void GetTurtle_Missing_Throws()
    {
        var w = new World();

        Assert.Throws<TurtleNotFoundException>(() => w.GetTurtle("nope"));
    }

    [Fact]
    public void Do_UnknownTurtle_Throws()
    {
        var w = new World();

        Assert.Throws<TurtleNotFoundException>(() => w.Do("nope", t => t.Forward(10)));
    }

    [Fact]
    public void Do_AddsDrawnSegmentToCanvas()
    {
        var w = new World();
        w.CreateTurtle("a");

        w.Do("a", t => t.Forward(100));

        var r = new TestRenderer();
        w.Canvas.Render(r);
        var line = Assert.Single(r.Lines);
        Assertions.Near(new Position(0, 100), line.B);
    }

    [Fact]
    public void Do_WhenPenUp_AddsNothingToCanvas()
    {
        var w = new World();
        var t = w.CreateTurtle("a");
        t.IsPenDown = false;

        w.Do("a", x => x.Forward(100));

        var r = new TestRenderer();
        w.Canvas.Render(r);
        Assert.Empty(r.Order);
    }

    [Fact]
    public void Do_EndFill_AddsPolygonToCanvas()
    {
        var w = new World();
        w.CreateTurtle("a");

        w.Do("a", t => { t.BeginFill(); return null; });
        for (int i = 0; i < 3; i++)
        {
            w.Do("a", t => t.Forward(50));
            w.Do("a", t => { t.Right(120); return null; });
        }
        w.Do("a", t => t.EndFill());

        var r = new TestRenderer();
        w.Canvas.Render(r);
        Assert.Equal(3, r.Lines.Count);
        Assert.Single(r.Polygons);
    }

    [Fact]
    public void TwoTurtles_HaveIndependentState()
    {
        var w = new World();
        var a = w.CreateTurtle("a");
        var b = w.CreateTurtle("b");

        w.Do("a", t => t.Forward(100));
        w.Do("b", t => { t.Right(90); return null; });
        w.Do("b", t => t.Forward(30));

        Assertions.Near(new Position(0, 100), a.Pos);
        Assertions.Near(new Position(30, 0), b.Pos);
        Assertions.Near(0f, a.Heading);
        Assertions.Near(90f, b.Heading);
    }

    [Fact]
    public void TwoTurtles_DrawOnTheSameCanvas()
    {
        var w = new World();
        w.CreateTurtle("a");
        w.CreateTurtle("b");

        w.Do("a", t => t.Forward(10));
        w.Do("b", t => t.Forward(20));

        var r = new TestRenderer();
        w.Canvas.Render(r);
        Assert.Equal(2, r.Lines.Count);
    }

    [Fact]
    public void RemoveTurtle_KeepDrawings_LeavesItsLines()
    {
        var w = new World();
        w.CreateTurtle("a");
        w.Do("a", t => t.Forward(10));

        Assert.True(w.RemoveTurtle("a", keepDrawings: true));

        var r = new TestRenderer();
        w.Canvas.Render(r);
        Assert.Single(r.Lines);
        Assert.Throws<TurtleNotFoundException>(() => w.GetTurtle("a"));
        var remaining = Assert.Single(w.Turtles);
        Assert.Equal(World.DefaultTurtleNme, remaining.Name);
    }

    [Fact]
    public void RemoveTurtle_DiscardDrawings_RemovesOnlyItsLines()
    {
        var w = new World();
        w.CreateTurtle("a");
        w.CreateTurtle("b");
        w.Do("a", t => t.Forward(10));
        w.Do("b", t => t.Forward(20));

        w.RemoveTurtle("a", keepDrawings: false);

        var r = new TestRenderer();
        w.Canvas.Render(r);
        var line = Assert.Single(r.Lines);
        Assertions.Near(new Position(0, 20), line.B);
    }

    [Fact]
    public void RemoveTurtle_Missing_ReturnsFalse()
    {
        var w = new World();

        Assert.False(w.RemoveTurtle("nope"));
    }

    [Fact]
    public void Reset_ClearsTurtlesAndCanvas()
    {
        var w = new World();
        w.CreateTurtle("a");
        w.Do("a", t => t.Forward(10));

        w.Reset();

        var r = new TestRenderer();
        w.Canvas.Render(r);
        Assert.Empty(r.Order);
        Assert.Single(w.Turtles);
    }

    [Fact]
    public void Render_DrawsTrailsFirstAndTurtlesOnTop()
    {
        var w = new World();
        w.CreateTurtle("a");
        w.Do("a", t => t.Forward(10));
        w.Do("a", t => t.Forward(10));

        var r = new TestRenderer();
        w.Render(r);

        Assert.Equal(new[] { "Line", "Line", "Turtle", "Turtle" }, r.Order);
    }

    [Fact]
    public void Render_SkipsInvisibleTurtles()
    {
        var w = new World();
        w.GetTurtle("logi").IsVisible = false;
        w.CreateTurtle("b");

        var r = new TestRenderer();
        w.Render(r);

        var drawn = Assert.Single(r.Turtles);
        Assertions.Near(new Position(0, 0), drawn.Pos);
    }

    [Fact]
    public void Render_PassesCurrentPositionAndHeadingOfTurtle()
    {
        var w = new World();
        w.Do("logi", t => { t.Right(90); return null; });
        w.Do("logi", t => t.Forward(40));

        var r = new TestRenderer();
        w.Render(r);

        var drawn = Assert.Single(r.Turtles);
        Assertions.Near(new Position(40, 0), drawn.Pos);
        Assertions.Near(90f, drawn.Heading);
    }
}