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
    Static,

    Namespace,
    Import,

    Class,
    Struct,
    Record,

    Var,
    String,
    Return,
    Print,

    Plus,
    Minus,
    Star,
    Slash,
    Ampersand,
    Pipe,

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