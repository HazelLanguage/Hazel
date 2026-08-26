using Hazel.Diagnostics;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Imports;

namespace Hazel.Syntax;

public sealed class CompilationUnit : AstNode
{
    public IReadOnlyList<ImportDeclaration> Imports
    {
        get;
    }

    public IReadOnlyList<Declaration> Declarations
    {
        get;
    }


    public CompilationUnit(
        IReadOnlyList<ImportDeclaration> imports,
        IReadOnlyList<Declaration> declarations,
        SourceSpan span)
        : base(span)
    {
        Imports = imports;
        Declarations = declarations;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitCompilationUnit(this);
    }
}