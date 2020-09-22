using Xunit;
using static ExpressionTreeTestObjects.Functions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static System.Linq.Expressions.Expression;
using Microsoft.CSharp.RuntimeBinder;
using static Microsoft.CSharp.RuntimeBinder.Binder;

namespace ExpressionTreeToString.Tests {
    public class DynamicOperationsGrouping {
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
                double d = 5.2;
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
        // no need to test VB because the ternary If includes parentheses
        public void RightAssociativity(string language, string expected) {
            var expr = IIFE(() => {
                bool b1 = false;
                bool b2 = false;
                bool b3 = false;
                return Expr(() => (b2 ? b2 : b1) ? b1 : b3);
            });
            Assert.Equal(expected, expr.ToString(language));
        }
    }
}
