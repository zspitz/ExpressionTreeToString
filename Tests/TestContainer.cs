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
using System.Diagnostics;

namespace ExpressionTreeToString.Tests {
    public class TestContainer : IClassFixture<ExpectedDataFixture> {
        [Obsolete("Replace this with the ExpressionTreeToString.BuiltInRenderer enum")]
        public static readonly BuiltinRenderer[] RendererKeys = new[] {
            CSharp,
            VisualBasic,
            BuiltinRenderer.FactoryMethods,
            ObjectNotation,
            TextualTree,
            BuiltinRenderer.ToString,
            DebugView
        };

        private readonly ExpectedDataFixture fixture;
        public TestContainer(ExpectedDataFixture fixture) => this.fixture = fixture;

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
#if NET472
            ToStringWriterVisitor.FrameworkCompatible = true;
            DebugViewWriterVisitor.FrameworkCompatible = true;
#endif

            CurrentCulture = new CultureInfo("en-IL");

            if (rendererKey == DebugView && objectName == "FactoryMethods.DynamicBinaryOperation") {
                Debugger.Break();
            }

            var expected = rendererKey switch
            {
                BuiltinRenderer.ToString => o.ToString(),
                DebugView => debugView.GetValue(o),
                _ => fixture.expectedStrings[(rendererKey, objectName)]
            };
            var (actual, paths) = GetToString(rendererKey, o);

            // test that the string result is correct
            Assert.Equal(expected, actual);

            var resolver = new Resolver();
            // exclude paths to reduced nodes
            paths.RemoveWhere(x => x.Contains("(Reduced)"));
            // make sure that all paths resolve to a valid object
            Assert.All(paths, path => Assert.NotNull(resolver.Resolve(o, path)));

            // the paths from the Object Notation renderer serve as a reference for all the other renderers

            if (rendererKey == ObjectNotation) {
                fixture.expectedPaths[objectName] = paths;
                return;
            }

            if (!fixture.expectedPaths.TryGetValue(objectName, out var expectedPaths)) {
                (_, expectedPaths) = GetToString(ObjectNotation, o);
                fixture.expectedPaths[objectName] = expectedPaths;
            }

            Assert.True(expectedPaths.IsSupersetOf(paths));
        }

        public static TheoryData<BuiltinRenderer, string, string, object> TestObjectsData => Objects.Get().SelectMany(x => 
            RendererKeys.Select(key => (key, $"{x.source}.{x.name}", x.category, x.o)).Where(x => x.key != DebugView || x.o is Expression)
        ).ToTheoryData();

        [Fact]
        public void CheckMissingObjects() {
            var objectNames = fixture.expectedStrings.GroupBy(x => x.Key.objectName, (key, grp) => (key, grp.Select(x => x.Key.rendererKey).ToList()));
            foreach (var (name, key) in objectNames) {
                var o = Objects.ByName(name);
            }
        }

        static TestContainer() => Loader.Load();

        static PropertyInfo debugView = typeof(Expression).GetProperty("DebugView", BindingFlags.NonPublic | BindingFlags.Instance);
    }
}
