using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

public static class SwiftParserRunner
{
    public static Swift5Parser.Top_levelContext Parse(string code)
    {
        var input = new AntlrInputStream(code);
        var lexer = new Swift5Lexer(input);
        var tokens = new CommonTokenStream(lexer);
        var parser = new Swift5Parser(tokens);

        return parser.top_level();
    }
}
