using Antlr4.Runtime.Misc;
public class SwiftHalsteadVisitor : Swift5ParserBaseVisitor<object>
{
    public Dictionary<string, int> Operators { get; } = new();
    public Dictionary<string, int> Operands { get; } = new();

    // Множество ключевых слов, которые считаем операторами
    private readonly HashSet<string> _keywordOperators = new()
    {
        "let", "var", "if", "else", "for", "while", "switch", 
        "case", "default", "return", "break", "continue", "func",
        "class", "struct", "enum", "protocol", "import"
    };

    public override object VisitOperator([NotNull] Swift5Parser.OperatorContext context)
    {
        CountOperator(context.GetText());
        return base.VisitOperator(context);
    }

    public override object VisitKeyword([NotNull] Swift5Parser.KeywordContext context)
    {
        var keyword = context.GetText();
        if (_keywordOperators.Contains(keyword))
            CountOperator(keyword);
        else
            CountOperand(keyword);
            
        return base.VisitKeyword(context);
    }

    public override object VisitIdentifier([NotNull] Swift5Parser.IdentifierContext context)
    {
        CountOperand(context.GetText());
        return base.VisitIdentifier(context);
    }

    public override object VisitLiteral([NotNull] Swift5Parser.LiteralContext context)
    {
        CountOperand(context.GetText());
        return base.VisitLiteral(context);
    }

    private void CountOperator(string op)
    {
        if (Operators.ContainsKey(op))
            Operators[op]++;
        else
            Operators[op] = 1;
    }

    private void CountOperand(string op)
    {
        if (Operands.ContainsKey(op))
            Operands[op]++;
        else
            Operands[op] = 1;
    }

    // Метрики Холстеда (те же, что и выше)
    public int UniqueOperators => Operators.Count;
    public int UniqueOperands => Operands.Count;
    public int TotalOperators => Operators.Values.Sum();
    public int TotalOperands => Operands.Values.Sum();
    
    public int ProgramVocabulary => UniqueOperators + UniqueOperands;
    public int ProgramLength => TotalOperators + TotalOperands;
    
    public double CalculatedProgramLength => 
        UniqueOperators * Math.Log2(UniqueOperators) + 
        UniqueOperands * Math.Log2(UniqueOperands);
    
    public double Volume => ProgramLength * Math.Log2(ProgramVocabulary);
    public double Difficulty => (UniqueOperators / 2.0) * ((double)TotalOperands / UniqueOperands);
    public double Effort => Difficulty * Volume;
    public double TimeToProgram => Effort / 18;
    public double DeliveredBugs => Math.Pow(Volume, 2.0 / 3.0) / 3000;

    public void PrintMetrics()
    {
        Console.WriteLine("=== МЕТРИКИ ХОЛСТЕДА ===");
        Console.WriteLine($"Уникальных операторов (η1): {UniqueOperators}");
        Console.WriteLine($"Уникальных операндов (η2): {UniqueOperands}");
        Console.WriteLine($"Всего операторов (N1): {TotalOperators}");
        Console.WriteLine($"Всего операндов (N2): {TotalOperands}");
        Console.WriteLine($"\nСловарь программы (η = η1 + η2): {ProgramVocabulary}");
        Console.WriteLine($"Длина программы (N = N1 + N2): {ProgramLength}");
        Console.WriteLine($"Расчётная длина программы (N̂ = η1·log₂η1 + η2·log₂η2): {CalculatedProgramLength:F2}");
        Console.WriteLine($"\nОбъём программы (V = N·log₂η): {Volume:F2}");
        Console.WriteLine($"Сложность (D = (η1/2)·(N2/η2)): {Difficulty:F2}");
        Console.WriteLine($"Трудоёмкость (E = D·V): {Effort:F2}");
        Console.WriteLine($"Время программирования (T = E/18): {TimeToProgram:F2} сек");
        Console.WriteLine($"Количество ошибок (B = V^(2/3) / 3000): {DeliveredBugs:F4}");
    }
} 
// public class SwiftHalsteadVisitor : Swift5ParserBaseVisitor<object>
// {
//     public HashSet<string> Operators { get; } = new();
//     public HashSet<string> Operands { get; } = new();

//     public int TotalOperators { get; private set; }
//     public int TotalOperands { get; private set; }

//     public override object VisitOperator([NotNull] Swift5Parser.OperatorContext context)
//     {
//         var op = context.GetText();
//         Operators.Add(op);
//         TotalOperators++;
//         return base.VisitOperator(context);
//     }

//     public override object VisitIdentifier([NotNull] Swift5Parser.IdentifierContext context)
//     {
//         var id = context.GetText();
//         Operands.Add(id);
//         TotalOperands++;
//         return base.VisitIdentifier(context);
//     }
// }
