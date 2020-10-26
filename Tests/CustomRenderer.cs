using System.Linq.Expressions;
using TestObjects;
using Xunit;
using static ExpressionTreeToString.Renderers;

namespace ExpressionTreeToString.Tests {
    public class ExtensionWriterVisitor : CSharpWriterVisitor {
        public ExtensionWriterVisitor(object o) : base(o) { }

        protected override void WriteExtension(Expression expr) {
            if (expr.CanReduce) {
                WriteNode("", expr.Reduce());
                return;
            }
            base.WriteExtension(expr);
        }
    }

    public class CustomRenderer {
        public CustomRenderer() => 
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
