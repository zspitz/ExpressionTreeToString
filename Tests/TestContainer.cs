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
using static ExpressionTreeToString.Formatter;

namespace ExpressionTreeToString.Tests {
    public class TestContainer : IClassFixture<ExpectedDataFixture> {
        public static readonly Formatter[] Formatters = new[] {
            CSharp, 
            VisualBasic, 
            FactoryMethods, 
            ObjectNotation, 
            TextualTree 
        };

        private readonly ExpectedDataFixture fixture;
        public TestContainer(ExpectedDataFixture fixture) => this.fixture = fixture;

        private (string toString, HashSet<string> paths) GetToString(Formatter formatter, object o) {
            Language? language = formatter switch
            {
                ObjectNotation => Language.CSharp,
                FactoryMethods => Language.CSharp,
                TextualTree => Language.CSharp,
                _ => null
            };

            Dictionary<string, (int start, int length)> pathSpans;
            string ret = o switch
            {
                Expression expr => expr.ToString(formatter, out pathSpans, language),
                MemberBinding mbind => mbind.ToString(formatter, out pathSpans, language),
                ElementInit init => init.ToString(formatter, out pathSpans, language),
                SwitchCase switchCase => switchCase.ToString(formatter, out pathSpans, language),
                CatchBlock catchBlock => catchBlock.ToString(formatter, out pathSpans, language),
                LabelTarget labelTarget => labelTarget.ToString(formatter, out pathSpans, language),
                _ => throw new InvalidOperationException(),
            };
            return (
                ret,
                pathSpans.Keys.Select(x => {
                    if (formatter != ObjectNotation) {
                        x = x.Replace("_0", "");
                    }
                    return x;
                }).ToHashSet()
            );
        }

        [Theory]
        [MemberData(nameof(TestObjectsData))]
        public void TestMethod(Formatter formatter, string objectName, string category, object o) {
            CurrentCulture = new CultureInfo("en-IL");

            var expected = fixture.expectedStrings[(formatter, objectName)];
            var (actual, paths) = GetToString(formatter, o);

            // test that the string result is correct
            Assert.Equal(expected, actual);

            var resolver = new Resolver();
            // make sure that all paths resolve to a valid object
            Assert.All(paths, path => Assert.NotNull(resolver.Resolve(o, path)));

            // the paths from the Object Notation formatter serve as a reference for all the other formatters

            if (formatter == ObjectNotation) {
                fixture.expectedPaths[objectName] = paths;
                return;
            }

            if (!fixture.expectedPaths.TryGetValue(objectName, out var expectedPaths)) {
                (_, expectedPaths) = GetToString(ObjectNotation, o);
                fixture.expectedPaths[objectName] = expectedPaths;
            }

            Assert.True(expectedPaths.IsSupersetOf(paths));
        }

        public static TheoryData<Formatter, string, string, object> TestObjectsData => Objects.Get().SelectMany(x => 
            Formatters.Select(formatter => (formatter, $"{x.source}.{x.name}", x.category, x.o))
        ).ToTheoryData();

        [Fact]
        public void CheckMissingObjects() {
            var objectNames = fixture.expectedStrings.GroupBy(x => x.Key.objectName, (key, grp) => (key, grp.Select(x => x.Key.formatter).ToList()));
            foreach (var (name, formatters) in objectNames) {
                var o = Objects.ByName(name);
            }
        }

        static TestContainer() => Loader.Load();
    }
}
