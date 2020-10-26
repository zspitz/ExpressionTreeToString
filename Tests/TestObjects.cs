using ExpressionTreeTestObjects;
using Pather.CSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static System.Globalization.CultureInfo;
using ExpressionTreeTestObjects.VB;
using ZSpitz.Util;
using static ExpressionTreeToString.BuiltinRenderer;
using System.Reflection;
using static ExpressionTreeToString.Tests.Globals;
using System.Diagnostics;
using static ExpressionTreeToString.Tests.Functions;
using static System.Environment;
using static ZSpitz.Util.Functions;
using System.IO;

namespace ExpressionTreeToString.Tests {
    public class TestObjects : IClassFixture<FileDataFixture>, IClassFixture<PathsFixture> {
        private readonly FileDataFixture fileData;
        private readonly PathsFixture pathsFixture;

        public TestObjects(FileDataFixture fileData, PathsFixture pathsFixture) => 
            (this.fileData, this.pathsFixture) = (fileData, pathsFixture);

        private (string toString, HashSet<string> paths) GetToString(BuiltinRenderer rendererKey, object o) {
            Language? language = rendererKey switch
            {
                ObjectNotation => Language.CSharp,
                BuiltinRenderer.FactoryMethods => Language.CSharp,
                TextualTree => Language.CSharp,
                _ => null
            };

            Dictionary<string, (int start, int length)> pathSpans;
            string ret = o switch
            {
                Expression expr => expr.ToString(rendererKey, out pathSpans, language),
                MemberBinding mbind => mbind.ToString(rendererKey, out pathSpans, language),
                ElementInit init => init.ToString(rendererKey, out pathSpans, language),
                SwitchCase switchCase => switchCase.ToString(rendererKey, out pathSpans, language),
                CatchBlock catchBlock => catchBlock.ToString(rendererKey, out pathSpans, language),
                LabelTarget labelTarget => labelTarget.ToString(rendererKey, out pathSpans, language),
                _ => throw new InvalidOperationException(),
            };
            return (
                ret,
                pathSpans.Keys.Select(x => {
                    if (rendererKey != ObjectNotation) {
                        x = x.Replace("_0", "");
                    }
                    return x;
                }).ToHashSet()
            );
        }

        [Theory]
        [MemberData(nameof(TestObjectsData))]
        public void TestMethod(BuiltinRenderer rendererKey, string objectName, string category, object o) {
            if (rendererKey == DynamicLinq) { return; }

#if NET472
            ToStringWriterVisitor.FrameworkCompatible = true;
            DebugViewWriterVisitor.FrameworkCompatible = true;
#endif
            TextualTreeWriterVisitor.ReducePredicate = null;

            CurrentCulture = new CultureInfo("en-IL");

            var expected = rendererKey switch
            {
                BuiltinRenderer.ToString => o.ToString(),
                DebugView => debugView.GetValue(o),
                _ => fileData.expectedStrings[(rendererKey, objectName)]
            };
            var (actual, paths) = GetToString(rendererKey, o);

            // test that the string result is correct
            Assert.Equal(expected, actual);

            var resolver = new Resolver();
            resolver.PathElementFactories.Insert(0, new MethodInvocationFactory());

            // make sure that all paths resolve to a valid object
            Assert.All(paths, path => Assert.NotNull(resolver.Resolve(o, path)));

            // the paths from the Textual tree renderer with all reducible nodes reduced serve as a reference for all the other renderers
            var expectedPaths = pathsFixture.allExpectedPaths[objectName];

            if (paths.Any(x => x.Contains("Reduce"))) {
                Debugger.Break();
            }

            Assert.True(expectedPaths.IsSupersetOf(paths));
        }

        public static TheoryData<BuiltinRenderer, string, string, object> TestObjectsData => Objects.Get().SelectMany(x =>
            BuiltinRenderers.Cast<BuiltinRenderer>().Select(key => (key, $"{x.source}.{x.name}", x.category, x.o)).Where(x => x.key != DebugView || x.o is Expression)
        ).ToTheoryData();

        [Fact]
        public void CheckMissingObjects() {
            var objectNames = fileData.expectedStrings.GroupBy(x => x.Key.objectName, (key, grp) => (key, grp.Select(x => x.Key.rendererKey).ToList()));
            foreach (var (name, key) in objectNames) {
                var o = Objects.ByName(name);
            }
        }

        static TestObjects() => Loader.Load();

        static readonly PropertyInfo debugView = typeof(Expression).GetProperty("DebugView", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    public class FileDataFixture {
        public readonly Dictionary<(BuiltinRenderer rendererKey, string objectName), string> expectedStrings = new Dictionary<(BuiltinRenderer rendererKey, string objectName), string>();
        public FileDataFixture() {
            foreach (var key in BuiltinRenderers) {
                if (key.In(DebugView, BuiltinRenderer.ToString)) { continue; }
                var expectedDataPath = GetFullFilename($"{key.ToString().ToLower()}-testdata.txt");
                string testName = "";
                string expected = "";
                foreach (var line in File.ReadLines(expectedDataPath)) {
                    if (line.StartsWith("----")) {
                        if (testName != "") {
                            if (key == BuiltinRenderer.FactoryMethods) {
                                expected = "// using static System.Linq.Expressions.Expression" + NewLines(2) + expected;
                            }
                            expectedStrings.Add((key, testName), expected.Trim());
                        }
                        testName = line.Substring(5); // ---- typename.testMethod
                        expected = "";
                    } else {
                        expected += line + NewLine;
                    }
                }
            }
        }
    }

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
