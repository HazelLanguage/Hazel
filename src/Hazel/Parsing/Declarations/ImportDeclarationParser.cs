using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Parsing;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Imports;

public sealed class ImportDeclarationParser : IDeclarationParser
{
    public bool RequiresAccessModifier => false;

    public bool CanParse(Token token)
    {
        return token.Kind == TokenKind.Import;
    }

    public Declaration Parse(Parser parser)
    {
        Token importToken =
            parser.Consume(TokenKind.Import);

        var parts = new List<string>();

        Token first =
            parser.Consume(TokenKind.Identifier);

        parts.Add(first.Text);

        while (parser.TryConsume(TokenKind.Dot))
        {
            Token part =
                parser.Consume(TokenKind.Identifier);

            parts.Add(part.Text);
        }

        Token semicolon =
            parser.Consume(TokenKind.Semicolon);

        return new ImportDeclaration(
            parts,
            SourceSpan.FromBounds(
                importToken.Span.Start,
                semicolon.Span.End));
    }
}