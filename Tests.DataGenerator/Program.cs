using ExpressionTreeToString;
using ExpressionTreeTestObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using static ExpressionTreeToString.FormatterNames;
using ExpressionTreeTestObjects.VB;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;

namespace Tests.DataGenerator {
    class Program {
        static void Main(string[] args) {
            Loader.Load();

            var lines = new List<string>();

            foreach (var (formatter, filename) in formatterFileMapping) {
                var ordering = parseFileOrder(@$"C:\Users\Spitz\source\repos\zspitz\ExpressionTreeToString\Tests\expectedResults\{filename}-testdata.txt");

                var language = formatter == VisualBasic ? VisualBasic : CSharp;

                foreach (var (category, source, name, o) in Objects.Get().Where(x => !ordering.ContainsKey($"{x.source}.{x.name}"))) {
                    lines.Add($"---- {source}.{name}");
                    var toWrite = o switch
                    {
                        Expression expr => expr.ToString(formatter, language),
                        MemberBinding mbind => mbind.ToString(formatter, language),
                        ElementInit init => init.ToString(formatter, language),
                        SwitchCase switchCase => switchCase.ToString(formatter, language),
                        CatchBlock catchBlock => catchBlock.ToString(formatter, language),
                        LabelTarget labelTarget => labelTarget.ToString(formatter, language),
                        _ => throw new NotImplementedException(),
                    };
                    lines.Add(toWrite);
                }

                lines.Add("------");
            }

            File.WriteAllLines("generated test data.txt", lines);
        }

        static Dictionary<string, int> parseFileOrder(string filepath) =>
            File.ReadLines(filepath)
                .Where(line => line.StartsWith("---- "))
                .Select((x, index) => (x.Substring(5), index))
                .ToDictionary(x => x.Item1, x => x.index);

        static Dictionary<string, string> formatterFileMapping = new Dictionary<string, string> {
            [CSharp] = "csharp",
            [VisualBasic] = "visual basic",
            [FactoryMethods] = "factory methods",
            [ObjectNotation] = "object notation",
            [TextualTree] ="textual tree"
        };
    }
}
