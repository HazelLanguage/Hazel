using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax.Declarations;

namespace Hazel.Parsing.Declarations;

public sealed class NamespaceDeclarationParser
    : IDeclarationParser
{
    public bool RequiresAccessModifier => false;

    private readonly DeclarationParserRegistry _registry;

    public NamespaceDeclarationParser(
        DeclarationParserRegistry registry)
    {
        _registry = registry;
    }

    public bool CanParse(Token token)
    {
        return token.Kind == TokenKind.Namespace;
    }

    public Declaration Parse(Parser parser)
    {
        Token namespaceToken =
            parser.Consume(TokenKind.Namespace);

        Token name =
            parser.Consume(TokenKind.Identifier);

        parser.Consume(TokenKind.LeftBrace);

        var members =
            new List<Declaration>();

        while (!parser.Check(TokenKind.RightBrace))
        {
            members.Add(
                _registry.Parse(parser));
        }

        Token rightBrace =
            parser.Consume(TokenKind.RightBrace);

        return new NamespaceDeclaration(
            name.Text,
            members,
            SourceSpan.FromBounds(
                namespaceToken.Span.Start,
                rightBrace.Span.End));
    }
}