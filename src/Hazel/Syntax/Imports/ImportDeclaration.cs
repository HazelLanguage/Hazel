using Hazel.Diagnostics;
using Hazel.Syntax;
using Hazel.Syntax.Declarations;

namespace Hazel.Syntax.Imports;

public sealed class ImportDeclaration : Declaration
{
    public IReadOnlyList<string> Parts
    {
        get;
    }

    public ImportDeclaration(
        IReadOnlyList<string> parts,
        SourceSpan span)
        : base(span)
    {
        Parts = parts;
    }

    public string Name =>
        string.Join(".", Parts);

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitImport(this);
    }
}