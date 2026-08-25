namespace Hazel.Lexing;

public enum TokenKind
{
    EOF,

    Identifier,
    Integer,

    Public,
    Private,
    Protected,
    Internal,

    Namespace,
    Import,

    Class,
    Struct,
    Record,

    Var,
    Return,

    Plus,
    Minus,
    Star,
    Slash,

    Equals,

    LeftParen,
    RightParen,

    LeftBrace,
    RightBrace,

    Semicolon,
    Comma,
    Colon,
    Dot
}