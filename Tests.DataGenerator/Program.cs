using ExpressionTreeToString;
using ExpressionTreeTestObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using static ExpressionTreeToString.FormatterNames;
using ExpressionTreeTestObjects.VB;

namespace Tests.DataGenerator {
    class Program {
        static void Main(string[] args) {
            Loader.Load();

            var formatter = TextualTree;
            var language = CSharp;

            var lines = new List<string>();
            foreach (var (category, source, name, o) in Objects.Get()) {
                lines.Add($"---- {source}.{name}");
                var toWrite = o switch {
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

            File.WriteAllLines("generated test data.txt", lines);
        }
    }
}
