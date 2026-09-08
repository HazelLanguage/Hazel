using Hazel.Diagnostics;
using Hazel.Lexing;
using Hazel.Syntax;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Expressions;
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
            int savedPosition = parser.Position;

            try
            {
                AccessModifiers fieldAccess = AccessModifiers.None;
                FieldModifiers fieldModifiers = FieldModifiers.None;

                while (parser.Peek().Kind.IsAccessModifier() ||
                       parser.Peek().Kind.IsFieldModifier())
                {
                    if (parser.Check(TokenKind.Static) ||
                        parser.Check(TokenKind.Unpacked))
                    {
                        fieldModifiers |= parser.Advance().Kind.ToFieldModifier();
                    }
                    else
                    {
                        fieldAccess |= parser.Advance().Kind.ToAccessModifier();
                    }
                }

                if ((fieldAccess == AccessModifiers.None && fieldModifiers == FieldModifiers.None) ||
                    !parser.Check(TokenKind.Var))
                {
                    parser.SetPosition(savedPosition);
                    members.Add(_memberRegistry.Parse(parser));
                    continue;
                }

                parser.Consume(TokenKind.Var);

                TypeReference fieldType = parser.ConsumeTypeReference();
                Token fieldName = parser.ConsumeIdentifier();

                if (parser.Check(TokenKind.LeftParen))
                {
                    parser.SetPosition(savedPosition);
                    members.Add(_memberRegistry.Parse(parser));
                    continue;
                }

                Expression? fieldValue = null;
                if (parser.TryConsume(TokenKind.Equals))
                {
                    var expressionParser = parser.CreateExpressionParser();
                    fieldValue = expressionParser.ParseExpression();
                    parser.SetPosition(expressionParser.Position);
                }

                parser.Consume(TokenKind.Semicolon);

                members.Add(new FieldDeclaration(
                    fieldAccess,
                    fieldModifiers,
                    fieldType,
                    fieldName.Text,
                    fieldValue,
                    SourceSpan.FromBounds(
                        savedPosition,
                        parser.Position)));
            }
            catch
            {
                parser.SetPosition(savedPosition);
                members.Add(_memberRegistry.Parse(parser));
            }
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