using Hazel.Lexing;
using Hazel.Syntax.Statements;

namespace Hazel.Parsing.Statements;

public sealed class ExpressionStatementParser
    : IStatementParser
{
    public bool CanParse(Token token)
    {
        return token.Kind == TokenKind.Identifier ||
               token.Kind == TokenKind.Integer ||
               token.Kind == TokenKind.LeftParen;
    }

    public Statement Parse(Parser parser)
    {
        var expressionParser =
            parser.CreateExpressionParser();

        var expression =
            expressionParser.ParseExpression();

        parser.SetPosition(
            expressionParser.Position);

        parser.TryConsume(
            TokenKind.Semicolon);

        return new ExpressionStatement(
            expression,
            expression.Span);
    }
}