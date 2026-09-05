using Hazel.Lexing;
using Hazel.Syntax.Declarations;

namespace Hazel.Parsing;

public interface IDeclarationParser
{
    bool CanParse(Token token);

    bool RequiresAccessModifier
    {
        get;
    }

    Declaration Parse(Parser parser);
}