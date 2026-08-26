namespace Hazel.Lexing;

public enum TokenKind
{
    EOF,

    Identifier,
    Integer,
    StringLiteral,

    Public,
    Private,
    Protected,
    Internal,
    Sealed,
    Abstract,

    Namespace,
    Import,

    Class,
    Struct,
    Record,

    Var,
    String,
    Return,

    Plus,
    Minus,
    Star,
    Slash,

    Equals,

    LeftParen,
    RightParen,

    LeftBracket,
    RightBracket,

    LeftBrace,
    RightBrace,

    Semicolon,
    Comma,
    Colon,
    Dot
}