namespace Logomocja.Core;

public sealed class Canvas
{
    private readonly List<Drawable> _items = new();
    public Color Background { get; set; } = Color.White;

    public void Add(Drawable d) => _items.Add(d);
    public void Clear() => _items.Clear();
    public void ClearBy(string turtle) => _items.RemoveAll(d => d.TurtleName == turtle);

    public void Render(IRenderer r)
    {
        foreach (var d in _items)
        {
            d.Draw(r);
        }
    }
}