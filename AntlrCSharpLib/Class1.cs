using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntlrCSharpLib;

public class HalsteadMetrics
{
    public Dictionary<string, int> Operators { get; } = new();
    public Dictionary<string, int> Operands { get; } = new();

    public int UniqueOperators => Operators.Count;
    public int UniqueOperands => Operands.Count;
    public int TotalOperators => Operators.Values.Sum();
    public int TotalOperands => Operands.Values.Sum();

    public int ProgramVocabulary => UniqueOperators + UniqueOperands;
    public int ProgramLength => TotalOperators + TotalOperands;
    public double Volume => ProgramLength * (ProgramVocabulary > 0 ? Math.Log2(ProgramVocabulary) : 0);

    public void Calculate(CommonTokenStream tokens)
    {
        tokens.Fill();
        var allTokens = tokens.GetTokens();

        for (int i = 0; i < allTokens.Count; i++)
        {
            var token = allTokens[i];
            string name = Swift5Lexer.DefaultVocabulary.GetSymbolicName(token.Type);
            string text = token.Text;

            if (string.IsNullOrEmpty(name) || IsIgnored(name)) continue;

            if (IsLiteral(name)||name.Contains("digits"))
            {
                AddToken(Operands, text);
            }
            else if (name == "Identifier")
            {
                if (IsFunctionCall(allTokens, i))
                {
                    AddToken(Operators, text);
                }
                else
                {
                    AddToken(Operands, text);
                }
            }
            else
            {
                AddToken(Operators, text);
            }
        }
    }

    private bool IsFunctionCall(IList<IToken> allTokens, int currentIndex)
    {
        for (int j = currentIndex + 1; j < allTokens.Count; j++)
        {
            var nextToken = allTokens[j];
            string nextName = Swift5Lexer.DefaultVocabulary.GetSymbolicName(nextToken.Type);
            
            if (nextName == "WS" || nextName.Contains("comment")) continue;

            return nextToken.Text == "(";
        }
        return false;
    }

    private bool IsLiteral(string name)
    {
        string upper = name.ToUpper();
        return upper.Contains("LITERAL") || upper.Contains("STRING") || 
               upper == "TRUE" || upper == "FALSE" || upper == "NIL";
    }

    private bool IsIgnored(string name)
    {
        string upper = name.ToUpper();
        return upper == "WS" || upper.Contains("COMMENT") || upper == "EOF";
    }

    private void AddToken(Dictionary<string, int> dict, string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (dict.ContainsKey(key)) dict[key]++;
        else dict[key] = 1;
    }

    // public void PrintDetailedMetrics()
    // {
    //     Console.WriteLine("\n" + new string('=', 50));
    //     Console.WriteLine("ОТЧЕТ ПО МЕТРИКАМ ХОЛСТЕДА (Swift)");
    //     Console.WriteLine(new string('=', 50));

    //     Console.WriteLine("\n--- ТАБЛИЦА ОПЕРАТОРОВ (n1, N1) ---");
    //     PrintTable(Operators, TotalOperators);

    //     Console.WriteLine("\n--- ТАБЛИЦА ОПЕРАНДОВ (n2, N2) ---");
    //     PrintTable(Operands, TotalOperands);

    //     Console.WriteLine("\n--- ИТОГОВЫЕ РЕЗУЛЬТАТЫ ---");
    //     Console.WriteLine($"n1 (Уникальные операторы): {UniqueOperators}");
    //     Console.WriteLine($"n2 (Уникальные операнды):  {UniqueOperands}");
    //     Console.WriteLine($"N1 (Всего операторов):     {TotalOperators}");
    //     Console.WriteLine($"N2 (Всего операндов):      {TotalOperands}");
    //     Console.WriteLine(new string('-', 30));
    //     Console.WriteLine($"Словарь программы (η):     {ProgramVocabulary}");
    //     Console.WriteLine($"Длина программы (N):       {ProgramLength}");
    //     Console.WriteLine($"Объем программы (V):       {Volume:F2} бит");
    //     Console.WriteLine(new string('=', 50));
    // }

    public int GetUniqueOperators() {return UniqueOperators;}
    public int GetUniqueOperands() {return UniqueOperands;}
    public int GetTotalOperators() {return TotalOperators;}
    public int GetTotalOperands() {return TotalOperands;}
    public int GetVocabulary() {return ProgramVocabulary;}
    public int GetLength() {return ProgramLength;}
    public double GetVolume() {return Volume;}

    // private void PrintTable(Dictionary<string, int> dict, int total)
    // {
    //     Console.WriteLine($"{"Элемент",-20} | {"Кол-во",-7} | {"Частота",-8}");
    //     Console.WriteLine(new string('-', 40));
    //     foreach (var entry in dict.OrderByDescending(x => x.Value))
    //     {
    //         double freq = (double)entry.Value / total;
    //         Console.WriteLine($"{entry.Key,-20} | {entry.Value,-7} | {freq:P1}");
    //     }
    // }
}