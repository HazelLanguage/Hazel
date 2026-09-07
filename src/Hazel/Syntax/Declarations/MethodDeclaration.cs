using Hazel.Diagnostics;
using Hazel.Syntax.Statements;
using Hazel.Syntax.Types;

namespace Hazel.Syntax.Declarations;

public sealed class MethodDeclaration : Declaration
{
    public AccessModifiers AccessModifiers
    {
        get;
    }
    public MethodModifiers MethodModifiers
    {
        get;
    }
    public TypeReference ReturnType
    {
        get;
    }
    public string Name
    {
        get;
    }
    public IReadOnlyList<Parameter> Parameters
    {
        get;
    }
    public IReadOnlyList<Statement> Body
    {
        get;
    }

    public MethodDeclaration(
        AccessModifiers accessModifiers,
        MethodModifiers methodModifiers,
        TypeReference returnType,
        string name,
        IReadOnlyList<Parameter> parameters,
        IReadOnlyList<Statement> body,
        SourceSpan span)
        : base(span)
    {
        AccessModifiers = accessModifiers;
        MethodModifiers = methodModifiers;
        ReturnType = returnType;
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitMethod(this);
    }
}