using ExpressionTreeTestObjects;
using System;
using System.Globalization;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
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

        [TestObject(Conditionals)]
        internal static readonly Expression NpChainWithMethodsParameters = Expr(
            "np(LastName.ToString(@0).ToString(@0))",
            CultureInfo.GetCultureInfo("en-US")
        );

        [TestObject(Conditionals)]
        internal static readonly Expression NpChainWithMethodsParameters1 = Expr(
            "np(LastName.ToString(@0).ToString(@1))",
            CultureInfo.GetCultureInfo("en-US"),
            CultureInfo.GetCultureInfo("he-IL")
        );
    }
}
