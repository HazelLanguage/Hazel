using Hazel.Lexing;
using Hazel.Syntax.Statements;

namespace Hazel.Parsing;

public interface IStatementParser
{
    bool CanParse(Token token);

    Statement Parse(Parser parser);
}