namespace Hazel.Semantics;

public sealed class Scope
{
    private readonly Dictionary<string, Symbol> _symbols = new();

    public Scope? Parent
    {
        get;
    }

    public Scope(Scope? parent = null)
    {
        Parent = parent;
    }

    public bool Define(Symbol symbol)
    {
        return _symbols.TryAdd(symbol.Name, symbol);
    }

    public Symbol? Lookup(string name)
    {
        if (_symbols.TryGetValue(name, out var symbol))
            return symbol;

        return Parent?.Lookup(name);
    }
}