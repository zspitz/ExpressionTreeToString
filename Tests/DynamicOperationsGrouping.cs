using Xunit;
using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using Microsoft.CSharp.RuntimeBinder;
using static Microsoft.CSharp.RuntimeBinder.Binder;
using static ExpressionTreeTestObjects.Globals;
using System.Runtime.CompilerServices;

namespace ExpressionTreeToString.Tests {
    public class DynamicOperationsGrouping {
        private static readonly CSharpBinderFlags flags = CSharpBinderFlags.None;
        private static readonly CSharpArgumentInfo[] argInfos = new[] {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
        };
        private static readonly CSharpArgumentInfo[] argInfos2 = new[] {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
        };
        private static readonly Type context = typeof(OperationsGrouping);

        private static readonly ParameterExpression k = Parameter(typeof(int), "k");

        private static readonly CallSiteBinder addBinder = BinaryOperation(flags, ExpressionType.Add, context, argInfos2);
        private static readonly CallSiteBinder methodInvokeBinder = InvokeMember(flags, "Method", new Type[] { }, context, argInfos);

        [Theory]
        [InlineData("C#", "(i + j) * k")]
        [InlineData("Visual Basic", "(i + j) * k")]
        public void HigherBinaryPrecedence(string language, string expected) {
            var multiplyBinder = BinaryOperation(flags, ExpressionType.Multiply, context, argInfos2);
            var node = Dynamic(
                multiplyBinder, 
                typeof(int),
                Dynamic(addBinder, typeof(int), i, j), 
                k
            );
            string actual = node.ToString(language);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("C#", "(i + j).Method()")]
        [InlineData("Visual Basic", "(i + j).Method")]
        public void BinaryPrecedence(string language, string expected) {
            var node = Dynamic(
                methodInvokeBinder, 
                typeof(string),
                Dynamic(addBinder, typeof(int), i, j)
            );
            var actual = node.ToString(language);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("C#", "((int)x).Method()")]
        // no need to test VB because conversion operations include parentheses
        public void ConversionPrecedence(string language, string expected) {
            var convertBinder = Convert(flags, typeof(int), context);
            var node = Dynamic(
                methodInvokeBinder, 
                typeof(string),
                Dynamic(convertBinder, typeof(int), x)
            );
            var actual = node.ToString(language);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("C#", "i + (j + k)")]
        [InlineData("Visual Basic", "i + (j + k)")]
        public void LeftAssociativity(string language, string expected) {
            var node = Dynamic(
                addBinder, 
                typeof(int), 
                i,
                Dynamic(addBinder, typeof(int), j, k)
            );
            var actual = node.ToString(language);
            Assert.Equal(expected, actual);
        }

        // TODO test right-associativity
        // There's no need to test in VB because operators are all left associative, except for = assignment which cannot be nested
        // But In C#, I can't seem to create a binder for any of the right-associative operators
        // The following fails on binder creation with an exception

        //[Theory(Skip ="Runtime exception on binder creation")]
        //[InlineData("C#", "() => (s ?? s1) ?? s2")]
        //public void RightAssociativity(string language, string expected) {
        //    var coalesceBinder = BinaryOperation(flags, ExpressionType.Coalesce, context, argInfos2);
        //    var node = Dynamic(
        //        coalesceBinder,
        //        typeof(string),
        //        Dynamic(coalesceBinder, typeof(string), s, s1),
        //        s2
        //    );
        //    var actual = node.ToString(language);
        //    Assert.Equal(expected, actual);
        //}
    }
}
