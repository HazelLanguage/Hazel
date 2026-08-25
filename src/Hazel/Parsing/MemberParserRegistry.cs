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

        throw new Exception(
            $"No member parser registered " +
            $"for {token.Kind}");
    }
}