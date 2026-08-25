using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax.Statements;

namespace Hazel.Parsing.Statements;

public sealed class VariableStatementParser : IStatementParser
{
    public bool CanParse(Token token)
    {
        return token.Kind == TokenKind.Var;
    }

    public Statement Parse(Parser parser)
    {
        Token varToken =
            parser.Consume(TokenKind.Var);

        Token name =
            parser.Consume(TokenKind.Identifier);

        parser.Consume(TokenKind.Equals);

        var expressionParser =
            parser.CreateExpressionParser();

        var expression =
            expressionParser.ParseExpression();

        parser.SetPosition(
            expressionParser.Position);

        parser.TryConsume(
            TokenKind.Semicolon);

        var span = SourceSpan.FromBounds(
            varToken.Span.Start,
            expression.Span.End);

        return new VariableStatement(
            name.Text,
            expression,
            span);
    }
}