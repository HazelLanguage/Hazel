using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Types;

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
            // Try to parse as a cast expression: (type)value
            int savedPosition = _position - 1; // Save position after '('

            if (TryParseTypeReference(out var typeReference) && typeReference != null)
            {
                // Check if next token is ')'
                if (Match(TokenKind.RightParen, out var rightParen))
                {
                    // This is a cast expression
                    var value = ParsePrimary();

                    return new ConversionExpression(
                        typeReference,
                        value,
                        SourceSpan.FromBounds(
                            leftParen.Span.Start,
                            value.Span.End));
                }
            }

            // Not a cast, rewind and parse as grouped expression
            _position = savedPosition;
            Consume(TokenKind.LeftParen);
            var expression = ParseExpression();
            Consume(TokenKind.RightParen);
            return expression;
        }

        throw Error(
            $"Expected expression, got {Peek().Kind}");
    }

    /// <summary>
    /// Attempts to parse a type reference at the current position.
    /// If successful, advances the position and returns true.
    /// If unsuccessful, rewinds the position and returns false.
    /// </summary>
    private bool TryParseTypeReference(out TypeReference? typeReference)
    {
        int savedPosition = _position;
        typeReference = null;

        Token token = Peek();

        if (token.Kind == TokenKind.String)
        {
            Advance();

            if (Check(TokenKind.LeftBracket))
            {
                Advance(); // consume '['

                if (Peek().Kind != TokenKind.Integer)
                {
                    _position = savedPosition;
                    return false;
                }

                Token lengthToken = Consume(TokenKind.Integer);

                if (!Check(TokenKind.RightBracket))
                {
                    _position = savedPosition;
                    return false;
                }

                Advance(); // consume ']'

                if (!int.TryParse(
                        lengthToken.Text,
                        out int maximumLength))
                {
                    _position = savedPosition;
                    return false;
                }

                typeReference = new BoundedStringTypeReference(
                    maximumLength,
                    SourceSpan.FromBounds(
                        token.Span.Start,
                        lengthToken.Span.End));
                return true;
            }

            typeReference = new NamedTypeReference(
                token.Text,
                token.Span);
            return true;
        }

        if (token.Kind == TokenKind.Identifier)
        {
            Advance();

            typeReference = new NamedTypeReference(
                token.Text,
                token.Span);
            return true;
        }

        _position = savedPosition;
        return false;
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

    private bool Check(TokenKind kind)
    {
        return Peek().Kind == kind;
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