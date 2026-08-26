using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax.Expressions;

namespace Hazel.Parsing;

public sealed class ExpressionParser
{
    private readonly IReadOnlyList<Token> _tokens;

    private int _position;

    public ExpressionParser(
        IReadOnlyList<Token> tokens,
        int position)
    {
        _tokens = tokens;
        _position = position;
    }

    public int Position => _position;

    public Expression ParseExpression(int minimumPrecedence = 0)
    {
        Expression left = ParsePrimary();

        while (true)
        {
            int precedence = GetPrecedence(Peek().Kind);

            if (precedence < minimumPrecedence)
                break;

            Token operatorToken = Advance();

            Expression right =
                ParseExpression(precedence + 1);

            left = new BinaryExpression(
                left,
                GetOperator(operatorToken.Kind),
                right,
                SourceSpan.FromBounds(
                    left.Span.Start,
                    right.Span.End));
        }

        return left;
    }

    private Expression ParsePrimary()
    {
        if (Match(TokenKind.Integer, out var integer))
        {
            return new IntegerExpression(
                long.Parse(integer.Text),
                integer.Span);
        }

        if (Match(TokenKind.StringLiteral, out var stringLiteral))
        {
            string value = stringLiteral.Text[1..^1];

            return new StringExpression(
                value,
                stringLiteral.Span);
        }

        if (Match(TokenKind.Identifier, out var identifier))
        {
            return new IdentifierExpression(
                identifier.Text,
                identifier.Span);
        }

        if (Match(TokenKind.LeftParen, out var leftParen))
        {
            var expression = ParseExpression();

            Consume(TokenKind.RightParen);

            return expression;
        }

        throw Error(
            $"Expected expression, got {Peek().Kind}");
    }

    private static int GetPrecedence(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Plus => 10,
            TokenKind.Minus => 10,

            TokenKind.Star => 20,
            TokenKind.Slash => 20,

            _ => -1
        };
    }

    private static BinaryOperator GetOperator(
        TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Plus => BinaryOperator.Add,
            TokenKind.Minus => BinaryOperator.Subtract,
            TokenKind.Star => BinaryOperator.Multiply,
            TokenKind.Slash => BinaryOperator.Divide,

            _ => throw new Exception(
                $"Not a binary operator: {kind}")
        };
    }

    private bool Match(
        TokenKind kind,
        out Token token)
    {
        if (Peek().Kind == kind)
        {
            token = Advance();
            return true;
        }

        token = default;
        return false;
    }

    private Token Consume(TokenKind kind)
    {
        if (Peek().Kind != kind)
            throw Error(
                $"Expected {kind}, got {Peek().Kind}");

        return Advance();
    }

    private Token Advance()
    {
        return _tokens[_position++];
    }

    private Token Peek()
    {
        return _tokens[_position];
    }

    private Exception Error(string message)
    {
        return new Exception(
            $"{message} at " +
            $"{Peek().Line}:{Peek().Column}");
    }
}