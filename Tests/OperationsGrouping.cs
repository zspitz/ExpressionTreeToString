using Xunit;
using static ExpressionTreeTestObjects.Functions;

namespace ExpressionTreeToString.Tests {
    public class OperationsGrouping {
        [Theory]
        [InlineData("C#", "() => (x + y) * z")]
        [InlineData("Visual Basic", "Function() (x + y) * z")]
        public void HigherBinaryPrecedence(string language, string expected) {
            var expr = IIFE(() => {
                var x = 0;
                var y = 0;
                var z = 0;
                return Expr(() => (x + y) * z);
            });
            Assert.Equal(expected, expr.ToString(language));
        }

        [Theory]
        [InlineData("C#", "() => (x + y).ToString()")]
        [InlineData("Visual Basic", "Function() (x + y).ToString")]
        public void BinaryPrecedence(string language, string expected) {
            var expr = IIFE(() => {
                var x = 0;
                var y = 0;
                return Expr(() => (x + y).ToString());
            });
            Assert.Equal(expected, expr.ToString(language));
        }

        [Theory]
        [InlineData("C#", "() => ((int)d).ToString()")]
        // no need to test VB because conversion operations include parentheses
        public void ConversionPrecedence(string language, string expected) {
            var expr = IIFE(() => {
                var d = 5.2;
                return Expr(() => ((int)d).ToString());
            });
            Assert.Equal(expected, expr.ToString(language));
        }

        [Theory]
        [InlineData("C#", "() => x + (y + z)")]
        [InlineData("Visual Basic", "Function() x + (y + z)")]
        public void LeftAssociativity(string language, string expected) {
            var expr = IIFE(() => {
                var x = 0;
                var y = 0;
                var z = 0;
                return Expr(() => x + (y + z));
            });
            Assert.Equal(expected, expr.ToString(language));
        }

        [Theory]
        [InlineData("C#", "() => (b2 ? b2 : b1) ? b1 : b3")]
        // no need to test VB because "simple" operators are all left associative, except for = assignment
        // the ones that are not, include parentheses as part of the syntax
        public void RightAssociativity(string language, string expected) {
            var expr = IIFE(() => {
                var b1 = false;
                var b2 = false;
                var b3 = false;
                return Expr(() => (b2 ? b2 : b1) ? b1 : b3);
            });
            Assert.Equal(expected, expr.ToString(language));
        }
    }
}
