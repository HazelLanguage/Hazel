using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Statements;

namespace Hazel.Parsing.Statements;

public sealed class PrintStatementParser : IStatementParser
{
    public bool CanParse(Token token)
    {
        return token.Kind == TokenKind.Print;
    }

    public Statement Parse(Parser parser)
    {
        Token printToken = parser.Consume(TokenKind.Print);

        var expressionParser =
            parser.CreateExpressionParser();

        Expression expression =
            expressionParser.ParseExpression();

        parser.SetPosition(
            expressionParser.Position);

        Token semicolon =
            parser.Consume(TokenKind.Semicolon);

        return new PrintStatement(
            expression,
            SourceSpan.FromBounds(
                printToken.Span.Start,
                semicolon.Span.End));
    }
}