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

namespace ExpressionTreeToString.Tests {
    public class TestObjectsRunner : IClassFixture<FileDataFixture>, IClassFixture<PathsFixture> {
        private readonly FileDataFixture fileData;
        private readonly PathsFixture pathsFixture;

        public TestObjectsRunner(FileDataFixture fileData, PathsFixture pathsFixture) => 
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

            Assert.True(expectedPaths.IsSupersetOf(paths));
        }

        public static readonly TheoryData<BuiltinRenderer, string, string, object> TestObjectsData = Objects.Get().SelectMany(x =>
            BuiltinRenderers.Cast<BuiltinRenderer>()
                .Select(key => (key, $"{x.source}.{x.name}", x.category, x.o))
                .Where(x => x.key != DebugView || x.o is Expression)
                .Where(x => x.key != DynamicLinq ^ x.Item2.StartsWith(nameof(DynamicLinqTestObjects)))
        ).ToTheoryData();

        [Fact]
        public void CheckMissingObjects() {
            var objectNames = fileData.expectedStrings.GroupBy(x => x.Key.objectName, (key, grp) => (key, grp.Select(x => x.Key.rendererKey).ToList()));
            foreach (var (name, key) in objectNames) {
                var o = Objects.ByName(name);
            }
        }

        static TestObjectsRunner() {
            Loader.Load();
            Objects.LoadType(typeof(DynamicLinqTestObjects));
        }

        static readonly PropertyInfo debugView = typeof(Expression).GetProperty("DebugView", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }
}
