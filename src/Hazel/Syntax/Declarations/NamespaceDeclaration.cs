using Hazel.Diagnostics;

namespace Hazel.Syntax.Declarations;

public sealed class NamespaceDeclaration : Declaration
{
    public string Name
    {
        get;
    }

    public IReadOnlyList<Declaration> Members
    {
        get;
    }

    public NamespaceDeclaration(
        string name,
        IReadOnlyList<Declaration> members,
        SourceSpan span)
        : base(span)
    {
        Name = name;
        Members = members;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitNamespace(this);
    }
}