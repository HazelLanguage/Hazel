using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Statements;
using Hazel.Syntax.Types;

namespace Hazel.Parsing.Declarations;

public sealed class MethodDeclarationParser : IMemberParser
{
    public bool CanParse(Token token)
    {
        return token.Kind.IsAccessModifier();
    }

    public Declaration Parse(Parser parser)
    {
        Token modifierToken = parser.Peek();

        AccessModifiers accessModifiers =
            parser.ConsumeAccessModifiers();

        TypeReference returnType =
            parser.ConsumeTypeReference();

        Token name =
            parser.ConsumeIdentifier();

        parser.Consume(TokenKind.LeftParen);

        var parameters = new List<Parameter>();

        if (!parser.Check(TokenKind.RightParen))
        {
            while (true)
            {
                TypeReference parameterType =
                    parser.ConsumeTypeReference();

                Token parameterName =
                    parser.ConsumeIdentifier();

                parameters.Add(
                    new Parameter(
                        parameterType,
                        parameterName.Text,
                        SourceSpan.FromBounds(
                            parameterType.Span.Start,
                            parameterName.Span.End)));

                if (!parser.TryConsume(TokenKind.Comma))
                {
                    break;
                }
            }
        }

        parser.Consume(TokenKind.RightParen);

        parser.Consume(TokenKind.LeftBrace);

        var body = new List<Statement>();

        while (!parser.Check(TokenKind.RightBrace))
        {
            body.Add(parser.ParseStatement());
        }

        Token rightBrace =
            parser.Consume(TokenKind.RightBrace);

        return new MethodDeclaration(
            accessModifiers,
            returnType,
            name.Text,
            parameters,
            body,
            SourceSpan.FromBounds(
                modifierToken.Span.Start,
                rightBrace.Span.End));
    }

    public static Declaration ParseStatic(Parser parser)
    {
        return new MethodDeclarationParser().Parse(parser);
    }
}