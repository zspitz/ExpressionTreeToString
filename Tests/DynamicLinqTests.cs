using System.Linq;
using Xunit;
using System.Linq.Dynamic.Core;
using ZSpitz.Util;
using System.Linq.Expressions;
using static ExpressionTreeToString.BuiltinRenderer;
using static System.Linq.Expressions.Expression;
using System.Linq.Dynamic.Core.Parser;
using static ExpressionTreeToString.Tests.Globals;
using System;

namespace ExpressionTreeToString.Tests {
    public class DynamicLinqTests {
        public static TheoryData<string, string, Expression> TestData =>
            Objects
                .Where(x => x.source == nameof(DynamicLinqTestObjects) && x.o is Expression)
                .SelectT((category, source, name, o) => (category, name, (Expression)o))
                .ToTheoryData();

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestMethod(string category, string name, Expression expr) {
            var selector = expr.ToString(DynamicLinq, "C#");
            if (category == "_Unrepresentable") {
                Assert.True(selector?.Contains("Not implemented"));
                return;
            }

            var dynamicLinqParameters = Array.Empty<object>();
            var parts = selector.Split(new[] { "\r\n\r\n" }, default);
            if (parts.Length>1) {
                selector = parts.Last();
                dynamicLinqParameters = DynamicLinqTestObjects.Parameters[name];
            }

            if (selector.StartsWith("\"")) {
                selector = selector[1..^1];
            } else {
                return;
            }

            var prm = Parameter(typeof(Person));
            var parser = new ExpressionParser(new[] { prm }, selector, dynamicLinqParameters, ParsingConfig.Default);

            // test that the generated string can be parsed successfully
            var _ = parser.Parse(null);
        }
    }
}
