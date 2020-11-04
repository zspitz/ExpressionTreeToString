using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static ExpressionTreeTestObjects.Functions;

namespace ExpressionTreeTestObjects {
    partial class CSCompiler {
        [TestObject(Unary)]
        internal static readonly Expression Negate = IIFE(() => {
            var i = 1;
            return Expr(() => -i);
        });

        [TestObject(Unary)]
        internal static readonly Expression BitwiseNot = IIFE(() => {
            var i = 1;
            return Expr(() => ~i);
        });

        [TestObject(Unary)]
        internal static readonly Expression LogicalNot = IIFE(() => {
            var b = true;
            return Expr(() => !b);
        });

        [TestObject(Unary)]
        internal static readonly Expression TypeAs = IIFE(() => {
            object? o = null;
            return Expr(() => o as string);
        })
            ;
        [TestObject(Unary)]
        internal static readonly Expression ArrayLength = IIFE(() => {
            var arr = new string[] { };
            return Expr(() => arr.Length);
        });

        [TestObject(Unary)]
        internal static readonly Expression Convert = IIFE(() => {
            var lst = new List<string>();
            return Expr(() => (object)lst);
        });
    }
}
