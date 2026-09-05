using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Types;

namespace Hazel.Parsing.Declarations;

public sealed class TypeDeclarationParser : IDeclarationParser, IMemberParser
{
    public bool RequiresAccessModifier => false;

    private readonly MemberParserRegistry _memberRegistry;

    public TypeDeclarationParser(
        MemberParserRegistry memberRegistry)
    {
        _memberRegistry = memberRegistry;
    }

    public bool CanParse(Token token)
    {
        return token.Kind.IsAccessModifier();
    }

    public Declaration Parse(Parser parser)
    {
        Token modifierToken = parser.Peek();

        AccessModifiers accessModifiers =
            parser.ConsumeAccessModifiers();

        TypeModifiers typeModifiers =
            parser.ConsumeTypeModifiers();

        Token kindToken = parser.Advance();

        TypeKind kind = kindToken.Kind switch
        {
            TokenKind.Class => TypeKind.Class,
            TokenKind.Struct => TypeKind.Struct,
            TokenKind.Record => TypeKind.Record,

            _ => throw new Exception(
                $"Expected type keyword after access modifier, " +
                $"but found '{kindToken.Text}'.")
        };

        Token name =
            parser.ConsumeIdentifier();

        parser.Consume(TokenKind.LeftBrace);

        var members = new List<Declaration>();

        while (!parser.Check(TokenKind.RightBrace))
        {
            members.Add(
                _memberRegistry.Parse(parser));
        }

        Token rightBrace =
            parser.Consume(TokenKind.RightBrace);

        return new TypeDeclaration(
            accessModifiers,
            typeModifiers,
            kind,
            name.Text,
            members,
            SourceSpan.FromBounds(
                modifierToken.Span.Start,
                rightBrace.Span.End));
    }
}