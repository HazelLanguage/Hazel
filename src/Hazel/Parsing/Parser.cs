using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Imports;
using Hazel.Syntax.Statements;
using Hazel.Syntax.Types;

namespace Hazel.Parsing;

public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;

    private readonly StatementParserRegistry _statementRegistry;
    private readonly DeclarationParserRegistry _declarationRegistry;

    private int _position;

    public Parser(
        IReadOnlyList<Token> tokens,
        DeclarationParserRegistry declarationRegistry,
        StatementParserRegistry statementRegistry)
    {
        _tokens = tokens;
        _declarationRegistry = declarationRegistry;
        _statementRegistry = statementRegistry;
    }

    public CompilationUnit Parse()
    {
        var imports = new List<ImportDeclaration>();
        var declarations = new List<Declaration>();

        while (!Check(TokenKind.EOF))
        {
            declarations.Add(
                _declarationRegistry.Parse(this));
        }

        SourceSpan span;

        if (declarations.Count == 0)
        {
            span = new SourceSpan(0, 0);
        }
        else
        {
            span = SourceSpan.FromBounds(
                declarations[0].Span.Start,
                declarations[^1].Span.End);
        }

        return new CompilationUnit(
            imports,
            declarations,
            span);
    }

    // ─────────────────────────────────────────────
    // Expressions
    // ─────────────────────────────────────────────

    public ExpressionParser CreateExpressionParser()
    {
        return new ExpressionParser(
            _tokens,
            _position);
    }

    public void SetPosition(int position)
    {
        _position = position;
    }

    // ─────────────────────────────────────────────
    // Tokens
    // ─────────────────────────────────────────────

    public Token Peek()
    {
        return _tokens[_position];
    }

    public Token Advance()
    {
        return _tokens[_position++];
    }

    public bool Check(TokenKind kind)
    {
        return Peek().Kind == kind;
    }

    public Token Consume(TokenKind kind)
    {
        if (!Check(kind))
        {
            throw new Exception(
                $"Expected {kind}, got {Peek().Kind}");
        }

        return Advance();
    }

    public bool TryConsume(TokenKind kind)
    {
        if (!Check(kind))
            return false;

        Advance();
        return true;
    }

    // ─────────────────────────────────────────────
    // Common syntax
    // ─────────────────────────────────────────────

    public Token ConsumeIdentifier()
    {
        return Consume(TokenKind.Identifier);
    }

    public AccessModifiers ConsumeAccessModifiers()
    {
        AccessModifiers modifiers = AccessModifiers.None;

        while (Peek().Kind.IsAccessModifier())
        {
            Token token = Advance();
            modifiers |= token.Kind.ToAccessModifier();
        }

        if (!modifiers.IsValid())
        {
            throw new Exception(
                $"Invalid combination of access modifiers.");
        }

        return modifiers;
    }

    public TypeReference ConsumeTypeReference()
    {
        Token token = Peek();

        if (token.Kind == TokenKind.String)
        {
            Advance();

            if (Check(TokenKind.LeftBracket))
            {
                Consume(TokenKind.LeftBracket);

                Token lengthToken =
                    Consume(TokenKind.Integer);

                Consume(TokenKind.RightBracket);

                if (!int.TryParse(
                        lengthToken.Text,
                        out int maximumLength))
                {
                    throw new Exception(
                        $"Invalid bounded string length " +
                        $"'{lengthToken.Text}'.");
                }

                return new BoundedStringTypeReference(
                    maximumLength,
                    SourceSpan.FromBounds(
                        token.Span.Start,
                        lengthToken.Span.End));
            }

            return new NamedTypeReference(
                token.Text,
                token.Span);
        }

        if (token.Kind == TokenKind.Identifier)
        {
            Advance();

            return new NamedTypeReference(
                token.Text,
                token.Span);
        }

        throw new Exception(
            $"Expected type, got {token.Kind}");
    }

    // ─────────────────────────────────────────────
    // Statements
    // ─────────────────────────────────────────────

    public Statement ParseStatement()
    {
        return _statementRegistry.Parse(this);
    }
}