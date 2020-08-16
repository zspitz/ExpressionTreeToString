using TestObjects;
using Xunit;
using static ExpressionTreeTestObjects.Functions;

namespace ExpressionTreeToString.Tests {
    public class Tests {
        [Fact]
        public void EscapedString() {
            var expr = Expr(() => "\'\"\\\0\a\b\f\n\r\t\v");
            Assert.Equal(@"() => ""\'\""\\\0\a\b\f\n\r\t\v""", expr.ToString("C#"));
        }

        [Fact]
        public void TestCustomFormatter() {
            var expr = new ExtensionExpression();
            var formatter = new ExtensionExpressionFormatter(expr);
            const string expected = "42 + 27";
            var actual = formatter.ToString();
            Assert.Equal(expected, actual);
        }
    }
}
