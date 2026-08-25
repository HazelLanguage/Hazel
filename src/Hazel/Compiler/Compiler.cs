using Hazel.CodeGen.CSharp;
using Hazel.Parsing.Declarations;
using Hazel.Lexing;
using Hazel.Lowering;
using Hazel.Parsing;
using Hazel.Parsing.Statements;
using Hazel.Semantics;

namespace Hazel.Compiler;

public sealed class Compiler
{
    public string Compile(string source)
    {
        // 1. Lex
        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();

        // 2. Parse
        var statementRegistry =
            new StatementParserRegistry();

        statementRegistry.Register(
            new VariableStatementParser());

        statementRegistry.Register(
            new ExpressionStatementParser());

        statementRegistry.Register(
            new ReturnStatementParser());

        var memberRegistry =
            new MemberParserRegistry();

        memberRegistry.Register(
            new MethodDeclarationParser());

        memberRegistry.Register(
            new TypeDeclarationParser(
                memberRegistry));

        var declarationRegistry =
            new DeclarationParserRegistry();

        declarationRegistry.Register(
            new NamespaceDeclarationParser(
                declarationRegistry));

        declarationRegistry.Register(
            new TypeDeclarationParser(
                memberRegistry));

        var parser = new Parser(
            tokens,
            declarationRegistry,
            statementRegistry);

        var ast = parser.Parse();

        // 3. Semantic analysis
        var semanticAnalyzer =
            new SemanticAnalyzer();

        semanticAnalyzer.Analyze(ast);

        // 4. Lower to IR
        var lowerer =
            new AstToIrLowerer();

        var ir = lowerer.Lower(ast);

        // 5. Generate C#
        var generator =
            new CSharpGenerator();

        return generator.Generate(ir);
    }
}