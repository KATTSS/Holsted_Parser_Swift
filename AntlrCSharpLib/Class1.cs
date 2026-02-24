using Antlr4.Runtime;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    private readonly HashSet<string> _openBrackets = new() { "(", "{", "[" };
    private readonly HashSet<string> _closingBrackets = new() { ")", "}", "]"};

    // private bool _syntaxError=false;
    // private int opens=0, closes=0;

        public void Calculate(CommonTokenStream tokens)
    {
        tokens.Fill();
        var allTokens = tokens.GetTokens();

        //  opens=0;
        //  closes=0;

        for (int i = 0; i < allTokens.Count; i++)
        {
            var token = allTokens[i];
            string name = Swift5Lexer.DefaultVocabulary.GetSymbolicName(token.Type);
            string text = token.Text;

            if (string.IsNullOrEmpty(name) || IsIgnored(name)) continue;
            if (IsDeclarationKeyword(text))
            {
                i = SkipDeclaration(allTokens, i);
                continue;
            }
            if (_closingBrackets.Contains(text)) {
               // ++closes; 
                continue;}

            if (name == "Identifier")
            {
                int nextIdx = GetNextMeaningfulTokenIndex(allTokens, i);
                if (nextIdx != -1 && allTokens[nextIdx].Text == "(")
                {
                    AddToken(Operators, text + "()");
                   // ++opens;
                    i = nextIdx; 
                }
                else
                {
                    AddToken(Operands, text);
                }
                continue;
            }

            if (IsLiteral(name))
            {
                AddToken(Operands, text);
                continue;
            }

            if (text == "if")
            {
                AddToken(Operators, "if...else"); 
            }
            else if (_openBrackets.Contains(text))
            {
                string display = text switch {
                    "(" => "()",
                    "[" => "[]",
                    "{" => "{}",
                    _ => text
                };
                AddToken(Operators, display);
                //++opens;
            }
            else
            {
                AddToken(Operators, text);
            }
        }
        // if (opens!=closes) _syntaxError=true;
        // Console.WriteLine($"opens {opens} - closes {closes}");
    }
    private int GetNextMeaningfulTokenIndex(IList<IToken> tokens, int currentIndex)
    {
        for (int j = currentIndex + 1; j < tokens.Count; j++)
        {
            string name = Swift5Lexer.DefaultVocabulary.GetSymbolicName(tokens[j].Type);
            if (IsIgnored(name)) continue;
            return j;
        }
        return -1;
    }

    private bool IsDeclarationKeyword(string text)
    {
        return text is "let" or "var" or "func" or "class" or "struct" or "enum" or "typealias";
    }

    private int SkipDeclaration(IList<IToken> tokens, int index)
    {
        int j = index;
        while (j < tokens.Count)
        {
            string t = tokens[j].Text;
            if (t == "=" || t == "{" || t == ";") break;
            j++;
        }
        return j - 1;
    }

    private bool IsLabel(IList<IToken> tokens, int index)
    {
        if (index + 1 >= tokens.Count) return false;
        var next = tokens[index + 1];
         return next.Text == ":";
    }

    private int SkipToPossibleElse(IList<IToken> tokens, int index)
    {
        return index;
    }

    private bool IsFunctionCall(IList<IToken> allTokens, int currentIndex)
    {
        for (int j = currentIndex + 1; j < allTokens.Count; j++)
        {
            var nextToken = allTokens[j];
            string nextName = Swift5Lexer.DefaultVocabulary.GetSymbolicName(nextToken.Type);
            if (nextName == "WS" || nextName.Contains("comment") || nextName == "LINE_BREAK") continue;
            return nextToken.Text == "(";
        }
        return false;
    }

    private bool IsLiteral(string name)
    {
        string upper = name.ToUpper();
        return (upper.Contains("LITERAL") || upper.Contains("STRING") || upper.Contains("DIGITS") ||  upper.Contains("QUOTED") || 
                upper == "TRUE" || upper == "FALSE" || upper == "NIL") & 
               (!upper.Contains("OPEN") && !upper.Contains("CLOSE"));
    }

    private bool IsIgnored(string name)
    {
        string upper = name.ToUpper();
        return upper == "WS" || upper.Contains("COMMENT") || upper == "EOF" || 
               upper == "LINE_BREAK" || upper == "INLINE_SPACES";
    }

    private void AddToken(Dictionary<string, int> dict, string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (dict.ContainsKey(key)) dict[key]++;
        else dict[key] = 1;
    }

    public int GetUniqueOperators() => UniqueOperators;
    public int GetUniqueOperands() => UniqueOperands;
    public int GetTotalOperators() => TotalOperators;
    public int GetTotalOperands() => TotalOperands;
    public int GetVocabulary() => ProgramVocabulary;
    public int GetLength() => ProgramLength;
    public double GetVolume() => Volume;
    // public bool GetError()=>_syntaxError;

    // public bool ResetError() => _syntaxError=false;
}