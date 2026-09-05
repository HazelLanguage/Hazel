using Hazel.Lexing;
using Hazel.Syntax.Declarations;

namespace Hazel.Parsing;

public sealed class DeclarationParserRegistry
{
    private readonly List<IDeclarationParser> _parsers = new();

    public void Register(
        IDeclarationParser parser)
    {
        _parsers.Add(parser);
    }

    public Declaration Parse(Parser parser)
    {
        Token token = parser.Peek();

        foreach (var declarationParser in _parsers)
        {
            if (declarationParser.CanParse(token))
                return declarationParser.Parse(parser);
        }

        if (_parsers.Any(p => p.RequiresAccessModifier))
        {
            throw new Exception(
                "Missing mandatory access modifier.");
        }

        throw new Exception(
            $"No declaration parser registered " +
            $"for {token.Kind}");
    }
}