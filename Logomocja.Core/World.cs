
namespace Logomocja.Core;

public sealed class World
{
    public const string DefaultTurtleNme = "logi";
    
    private readonly Dictionary<string, Turtle> _turtles = new();
    private readonly bool _createDefaultTurtle;
    public Canvas Canvas { get; } = new();
    public IReadOnlyCollection<Turtle> Turtles => _turtles.Values;
    
    public World(bool createDefaultTurtle = true)
    {
        _createDefaultTurtle = createDefaultTurtle;
        if (createDefaultTurtle) CreateTurtle(DefaultTurtleNme);
    }

    public Turtle CreateTurtle(string name)
    {
        if (_turtles.ContainsKey(name)) throw new DuplicateTurtleException(name);
        return _turtles[name] = new Turtle(name);
    }

    public Turtle GetTurtle(string name) =>
        _turtles.TryGetValue(name, out var t) ? t : throw new TurtleNotFoundException(name);

    public bool RemoveTurtle(string name, bool keepDrawings = true)
    {
        if (!_turtles.Remove(name)) return false;
        if (!keepDrawings) Canvas.ClearBy(name);
        return true;
    }

    public void Do(string name, Func<Turtle, Drawable?> action)
    {
        var drawn = action(GetTurtle(name));
        if (drawn is not null) Canvas.Add(drawn);
    }

    public void Reset()
    {
        _turtles.Clear();
        Canvas.Clear();
        if (_createDefaultTurtle) CreateTurtle(DefaultTurtleNme);
    }

    public void Render(IRenderer r)
    {
        Canvas.Render(r);
        foreach (var t in _turtles.Values)
        {
            if (t.IsVisible) r.DrawTurtle(t.Pos, t.Heading, t.PenColor);
        }
    }
    
}

public sealed class DuplicateTurtleException(string name)
    : Exception($"Żółw '{name}' już istnieje.");
public sealed class TurtleNotFoundException(string name)
    : Exception($"Nie ma żółwia '{name}'.");