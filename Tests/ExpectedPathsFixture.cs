using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionTreeTestObjects;
using ExpressionTreeTestObjects.VB;
using ZSpitz.Util;
using static ExpressionTreeToString.BuiltinRenderer;

namespace ExpressionTreeToString.Tests {
    public class PathsFixture {
        public readonly Dictionary<string, HashSet<string>> allExpectedPaths;
        public PathsFixture() {
            Loader.Load();

            TextualTreeWriterVisitor.ReducePredicate = expr => true;

            allExpectedPaths = Objects.Get()
                .SelectT((category, source, name, o) => (key: $"{source}.{name}", o))
                .Select(x => {
                    Dictionary<string, (int start, int length)> pathSpans;
                    string ret = x.o switch
                    {
                        Expression expr => expr.ToString(TextualTree, out pathSpans, "C#"),
                        MemberBinding mbind => mbind.ToString(TextualTree, out pathSpans, "C#"),
                        ElementInit init => init.ToString(TextualTree, out pathSpans, "C#"),
                        SwitchCase switchCase => switchCase.ToString(TextualTree, out pathSpans, "C#"),
                        CatchBlock catchBlock => catchBlock.ToString(TextualTree, out pathSpans, "C#"),
                        LabelTarget labelTarget => labelTarget.ToString(TextualTree, out pathSpans, "C#"),
                        _ => throw new InvalidOperationException(),
                    };
                    return (x.key, pathSpans.Keys.ToHashSet());
                })
                .ToDictionary();
        }
    }
}
