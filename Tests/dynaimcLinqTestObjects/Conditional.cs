using ExpressionTreeTestObjects;
using System;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    public static partial class DynamicLinqTestObjects {
        [TestObject("Conditional")]
        internal static readonly Expression Conditional = Expr(p => p.Age >= 13 ? "adult" : "child");

        [TestObject("Conditional")]
        internal static readonly Expression Np = Expr(p => p != null ? p.LastName : null);

        [TestObject("Conditional")]
        internal static readonly Expression NpValue = Expr(p => p != null ? p.LastName : "(unknown)");

        [TestObject("Conditional")]
        internal static readonly Expression NpChain = Expr(p => p != null && p.Father != null ? p.Father.LastName : "(unknown)");
    }
}
