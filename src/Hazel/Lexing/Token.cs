using Hazel.Diagnostics;

namespace Hazel.Lexing;

public readonly record struct Token(
    TokenKind Kind,
    string Text,
    SourceSpan Span,
    int Line,
    int Column
);