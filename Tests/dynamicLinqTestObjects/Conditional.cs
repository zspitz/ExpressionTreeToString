using ExpressionTreeTestObjects;
using System;
using System.Globalization;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeToString.Tests {
    public static partial class DynamicLinqTestObjects {
        [TestObject(Conditionals)]
        internal static readonly Expression Conditional = Expr(p => p.Age >= 13 ? "adult" : "child");

        [TestObject(Conditionals)]
        internal static readonly Expression Np = Expr("np(LastName)");

        [TestObject(Conditionals)]
        internal static readonly Expression NpValue = Expr("np(LastName, \"(unknown)\")");

        [TestObject(Conditionals)]
        internal static readonly Expression NpChain = Expr("np(Father.LastName, \"(unknown)\")");

        [TestObject(Conditionals)]
        internal static readonly Expression NpChainWithMethods = Expr("np(LastName.ToString().ToString())");

        // pending https://github.com/zspitz/ExpressionTreeToString/issues/80
        //[TestObject(Conditionals)]
        //internal static readonly Expression NpChainWithMethodsParameters = Expr(
        //    "np(p.LastName.ToString().ToString(@0))",
        //    CultureInfo.GetCultureInfo("en-US")
        //);
    }
}
