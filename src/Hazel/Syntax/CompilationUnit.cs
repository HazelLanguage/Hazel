using Hazel.Diagnostics;
using Hazel.Syntax.Declarations;

namespace Hazel.Syntax;

public sealed class CompilationUnit : AstNode
{
    public IReadOnlyList<Declaration> Declarations
    {
        get;
    }

    public CompilationUnit(
        IReadOnlyList<Declaration> declarations,
        SourceSpan span)
        : base(span)
    {
        Declarations = declarations;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitCompilationUnit(this);
    }
}