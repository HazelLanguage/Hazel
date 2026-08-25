using Hazel.Diagnostics;

namespace Hazel.Lexing;

public sealed class Lexer
{
    private readonly string _source;

    private int _position;
    private int _line = 1;
    private int _column = 1;

    public Lexer(string source)
    {
        _source = source;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (!IsAtEnd())
        {
            SkipWhitespace();

            if (IsAtEnd())
                break;

            tokens.Add(ReadToken());
        }

        tokens.Add(new Token(
            TokenKind.EOF,
            "",
            new SourceSpan(_position, 0),
            _line,
            _column));

        return tokens;
    }

    private Token ReadToken()
    {
        int start = _position;
        int line = _line;
        int column = _column;

        char c = Advance();

        if (char.IsLetter(c) || c == '_')
            return ReadIdentifier(start, line, column);

        if (char.IsDigit(c))
            return ReadNumber(start, line, column);

        return c switch
        {
            '+' => MakeToken(
                TokenKind.Plus,
                "+",
                start,
                line,
                column),

            '-' => MakeToken(
                TokenKind.Minus,
                "-",
                start,
                line,
                column),

            '*' => MakeToken(
                TokenKind.Star,
                "*",
                start,
                line,
                column),

            '/' => MakeToken(
                TokenKind.Slash,
                "/",
                start,
                line,
                column),

            '=' => MakeToken(
                TokenKind.Equals,
                "=",
                start,
                line,
                column),

            '(' => MakeToken(
                TokenKind.LeftParen,
                "(",
                start,
                line,
                column),

            ')' => MakeToken(
                TokenKind.RightParen,
                ")",
                start,
                line,
                column),

            ';' => MakeToken(
                TokenKind.Semicolon,
                ";",
                start,
                line,
                column),

            '{' => MakeToken(
                TokenKind.LeftBrace,
                "{",
                start,
                line,
                column),

            '}' => MakeToken(
                TokenKind.RightBrace,
                "}",
                start,
                line,
                column),

            ',' => MakeToken(
                TokenKind.Comma,
                ",",
                start,
                line,
                column),

            ':' => MakeToken(
                TokenKind.Colon,
                ":",
                start,
                line,
                column),

            '.' => MakeToken(
                TokenKind.Dot,
                ".",
                start,
                line,
                column),

            _ => throw new Exception(
                $"[{ErrorCodes.UnexpectedCharacter}] Unexpected character '{c}' ({(int)c}) at {line}:{column}")
        };
    }

    private Token ReadIdentifier(
        int start,
        int line,
        int column)
    {
        while (!IsAtEnd() &&
               (char.IsLetterOrDigit(Peek()) ||
                Peek() == '_'))
        {
            Advance();
        }

        string text = _source[start.._position];

        TokenKind kind = text switch
        {
            "var" => TokenKind.Var,

            "public" => TokenKind.Public,
            "private" => TokenKind.Private,
            "protected" => TokenKind.Protected,
            "internal" => TokenKind.Internal,

            "namespace" => TokenKind.Namespace,
            "import" => TokenKind.Import,

            "class" => TokenKind.Class,
            "struct" => TokenKind.Struct,
            "record" => TokenKind.Record,

            "return" => TokenKind.Return,

            _ => TokenKind.Identifier
        };

        return new Token(
            kind,
            text,
            new SourceSpan(
                start,
                _position - start),
            line,
            column);
    }

    private Token ReadNumber(
        int start,
        int line,
        int column)
    {
        while (!IsAtEnd() &&
               char.IsDigit(Peek()))
        {
            Advance();
        }

        string text = _source[start.._position];

        return new Token(
            TokenKind.Integer,
            text,
            new SourceSpan(
                start,
                _position - start),
            line,
            column);
    }

    private void SkipWhitespace()
    {
        while (!IsAtEnd())
        {
            if (char.IsWhiteSpace(Peek()))
            {
                Advance();
            }
            else
            {
                break;
            }
        }
    }

    private char Advance()
    {
        char c = _source[_position++];

        if (c == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        return c;
    }

    private char Peek()
    {
        return IsAtEnd()
            ? '\0'
            : _source[_position];
    }

    private bool IsAtEnd()
    {
        return _position >= _source.Length;
    }

    private static Token MakeToken(
        TokenKind kind,
        string text,
        int position,
        int line,
        int column)
    {
        return new Token(
            kind,
            text,
            new SourceSpan(
                position,
                text.Length),
            line,
            column);
    }
}