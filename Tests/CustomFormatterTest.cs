using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestObjects;
using Xunit;
using static ExpressionTreeToString.Writers;

namespace ExpressionTreeToString.Tests {
    public class CustomFormatterTest {
        public CustomFormatterTest() => 
            Register(
                "CustomFormatter",
                (o, language, usePathSpans) => (new ExtensionExpressionFormatter(o).ToString(), null)
            );

        [Fact]
        public void TestCustomFormatter() {
            var expr = new ExtensionExpression();
            const string expected = "42 + 27";
            var actual = expr.ToString("CustomFormatter");
            Assert.Equal(expected, actual);
        }

    }
}
