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

        var parts = new List<string>
    {
        parser.Consume(TokenKind.Identifier).Text
    };

        while (parser.Check(TokenKind.Dot))
        {
            parser.Consume(TokenKind.Dot);

            parts.Add(
                parser.Consume(TokenKind.Identifier).Text);
        }

        string name =
            string.Join(".", parts);

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
            name,
            members,
            SourceSpan.FromBounds(
                namespaceToken.Span.Start,
                rightBrace.Span.End));
    }
}