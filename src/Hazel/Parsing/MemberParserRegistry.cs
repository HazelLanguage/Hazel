using Hazel.Lexing;
using Hazel.Syntax.Declarations;

namespace Hazel.Parsing;

public sealed class MemberParserRegistry
{
    private readonly List<IMemberParser> _parsers = new();

    public void Register(
        IMemberParser parser)
    {
        _parsers.Add(parser);
    }

    public Declaration Parse(Parser parser)
    {
        Token token = parser.Peek();

        foreach (var memberParser in _parsers)
        {
            if (memberParser.CanParse(token))
                return memberParser.Parse(parser);
        }

        if (_parsers.Any(p => p.RequiresAccessModifier))
        {
            throw new Exception(
                "Missing mandatory access modifier.");
        }

        throw new Exception(
            $"No member parser registered " +
            $"for {token.Kind}");
    }
}