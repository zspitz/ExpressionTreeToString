using ExpressionTreeToString;
using ExpressionTreeTestObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using ExpressionTreeTestObjects.VB;
using System.Linq;
using static ExpressionTreeToString.BuiltinRenderer;
using ZSpitz.Util;
using ExpressionTreeToString.Tests;

namespace Tests.DataGenerator {
    class Program {
        static void Main(string[] args) {
            Loader.Load();
            Objects.LoadType(typeof(DynamicLinqTestObjects));

            var lines = new List<string>();

            foreach (var (key, filename) in rendererFileMapping.SelectKVP((k,v) => (k,v))) {
                var ordering = parseFileOrder(@$"C:\Users\Spitz\source\repos\zspitz\ExpressionTreeToString\Tests\expectedResults\{filename}-testdata.txt");

                var language = key == VisualBasic ? Language.VisualBasic : Language.CSharp;

                const string dlinq = nameof(DynamicLinqTestObjects);
                var objects = Objects.Get()
                    .Where(x => key == DynamicLinq ? x.source == dlinq : x.source != dlinq)
                    .Where(x => !ordering.ContainsKey($"{x.source}.{x.name}"));

                //var objects = Objects.Get()
                //    .Where(x => key == DynamicLinq ? x.source == dlinq : x.source != dlinq)
                //    .OrderBy(x => ordering.TryGetValue($"{x.source}.{x.name}", out var order) ? order : -1);

                foreach (var (category, source, name, o) in objects) {
                    var toWrite = o switch
                    {
                        Expression expr => expr.ToString(key, out var pathSpans, language),
                        MemberBinding mbind => mbind.ToString(key, out var pathSpans, language),
                        ElementInit init => init.ToString(key, out var pathSpans, language),
                        SwitchCase switchCase => switchCase.ToString(key, out var pathSpans, language),
                        CatchBlock catchBlock => catchBlock.ToString(key, out var pathSpans, language),
                        LabelTarget labelTarget => labelTarget.ToString(key, out var pathSpans, language),
                        _ => throw new NotImplementedException(),
                    };
                    lines.Add($"---- {source}.{name}");
                    if (key == FactoryMethods) {
                        toWrite = toWrite.Replace(@"// using static System.Linq.Expressions.Expression

", "");
                    }
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

        static Dictionary<BuiltinRenderer, string> rendererFileMapping = new Dictionary<BuiltinRenderer, string> {
            [CSharp] = "csharp",
            [VisualBasic] = "visualbasic",
            [FactoryMethods] = "factorymethods",
            [ObjectNotation] = "objectnotation",
            [TextualTree] = "textualtree",
            [DynamicLinq] = "dynamiclinq"
        };
    }
}
