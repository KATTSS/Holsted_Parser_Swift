using AntlrCSharpLib;
using Antlr4.Runtime;
using System;

class Program 
{
    static void Main() 
    {
        string code = @"
            let x = 10 + 20
            let y = x * 5
            if y > 50 {
                print(""large"")
            } else {
                print(""small"")
            }
        ";

        try 
        {
            var input = new AntlrInputStream(code);
            var lexer = new Swift5Lexer(input);
            var tokens = new CommonTokenStream(lexer);

            var metrics = new HalsteadMetrics();
            metrics.Calculate(tokens);
           // metrics.PrintDetailedMetrics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}