using Hazel.Syntax.Declarations;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Imports;
using Hazel.Syntax.Statements;
using Hazel.Syntax.Types;

namespace Hazel.Syntax;

public abstract class AstVisitor<T>
{
    public virtual T VisitCompilationUnit(
        CompilationUnit node) => default!;

    public virtual T VisitNamespace(
        NamespaceDeclaration node) => default!;

    public virtual T VisitImport(
        ImportDeclaration node) => default!;

    public virtual T VisitType(
        TypeDeclaration node) => default!;

    public virtual T VisitMethod(
        MethodDeclaration node) => default!;

    public virtual T VisitParameter(
        Parameter node) => default!;

    public abstract T VisitNamedTypeReference(
        NamedTypeReference node);

    public abstract T VisitConversionExpression(
        ConversionExpression expression);

    public abstract T VisitBoundedStringTypeReference(
        BoundedStringTypeReference node);

    public virtual T VisitReturn(
        ReturnStatement node) => default!;

    public virtual T VisitInteger(
        IntegerExpression node) => default!;

    public virtual T VisitIdentifier(
        IdentifierExpression node) => default!;

    public virtual T VisitBinary(
        BinaryExpression node) => default!;

    public virtual T VisitVariable(
        VariableStatement node) => default!;

    public virtual T VisitString(
        StringExpression node) => default!;

    public virtual T VisitExpressionStatement(
        ExpressionStatement node) => default!;
}