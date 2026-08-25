using Hazel.Lexing;
using Hazel.Syntax.Declarations;

namespace Hazel.Parsing;

public interface IDeclarationParser
{
    bool CanParse(Token token);

    Declaration Parse(Parser parser);
}