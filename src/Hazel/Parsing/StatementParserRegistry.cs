using Hazel.Lexing;
using Hazel.Syntax.Statements;

namespace Hazel.Parsing;

public sealed class StatementParserRegistry
{
    private readonly List<IStatementParser> _parsers = new();

    public void Register(IStatementParser parser)
    {
        _parsers.Add(parser);
    }

    public Statement Parse(Parser parser)
    {
        Token token = parser.Peek();

        foreach (var statementParser in _parsers)
        {
            if (statementParser.CanParse(token))
                return statementParser.Parse(parser);
        }

        throw new Exception(
            $"No statement parser registered for {token.Kind}");
    }
}