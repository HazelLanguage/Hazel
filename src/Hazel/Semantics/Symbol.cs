using Hazel.Semantics.Types;

namespace Hazel.Semantics;

public sealed class Symbol
{
    public string Name
    {
        get;
    }
    public SymbolKind Kind
    {
        get;
    }
    public TypeSymbol Type
    {
        get;
    }

    public Symbol(
        string name,
        SymbolKind kind,
        TypeSymbol type)
    {
        Name = name;
        Kind = kind;
        Type = type;
    }
}