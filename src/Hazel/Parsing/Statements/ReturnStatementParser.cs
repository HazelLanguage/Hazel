using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Statements;

namespace Hazel.Parsing.Statements;

public sealed class ReturnStatementParser : IStatementParser
{
    public bool CanParse(Token token)
    {
        return token.Kind == TokenKind.Return;
    }

    public Statement Parse(Parser parser)
    {
        Token returnToken =
            parser.Consume(TokenKind.Return);

        Expression? expression = null;

        // If the next token is not a semicolon, parse an expression
        if (!parser.Check(TokenKind.Semicolon))
        {
            var expressionParser =
                parser.CreateExpressionParser();

            expression =
                expressionParser.ParseExpression();

            parser.SetPosition(
                expressionParser.Position);
        }

        Token semicolon =
            parser.Consume(TokenKind.Semicolon);

        return new ReturnStatement(
            expression,
            SourceSpan.FromBounds(
                returnToken.Span.Start,
                semicolon.Span.End));
    }
}