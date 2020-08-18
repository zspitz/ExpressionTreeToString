using TestObjects;
using Xunit;
using static ExpressionTreeToString.Renderers;

namespace ExpressionTreeToString.Tests {
    public class CustomRendererTest {
        public CustomRendererTest() => 
            Register(
                "CustomRenderer",
                (o, language, usePathSpans) => (new ExtensionWriterVisitor(o).ToString(), null)
            );

        [Fact]
        public void TestCustomRenderer() {
            var expr = new ExtensionExpression();
            const string expected = "42 + 27";
            var actual = expr.ToString("CustomRenderer");
            Assert.Equal(expected, actual);
        }

    }
}
