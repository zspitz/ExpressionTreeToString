using ExpressionTreeTestObjects;
using System;
using System.Globalization;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
        [TestObject(Conditionals)]
        internal static readonly Expression Conditional = Expr(p => p.Age >= 13 ? "adult" : "child");

        [TestObject(Conditionals)] // pending https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/520
        //internal static readonly Expression Np = Expr("np(LastName)");
        internal static readonly Expression Np = Expr(p => p != null ? p.LastName : null);

        [TestObject(Conditionals)]
        internal static readonly Expression NpValue = Lambda("np(LastName, \"(unknown)\")");

        [TestObject(Conditionals)]
        internal static readonly Expression NpChain = Lambda("np(Father.LastName, \"(unknown)\")");

        [TestObject(Conditionals)]
        internal static readonly Expression NpChainWithMethods = Lambda("np(LastName.ToString().ToString())");

        [TestObject(Conditionals)]
        internal static readonly Expression NpChainWithMethodsParameters = Lambda(
            "np(LastName.ToString(@0).ToString(@0))",
            CultureInfo.GetCultureInfo("en-US")
        );

        [TestObject(Conditionals)]
        internal static readonly Expression NpChainWithMethodsParameters1 = Lambda(
            "np(LastName.ToString(@0).ToString(@1))",
            CultureInfo.GetCultureInfo("en-US"),
            CultureInfo.GetCultureInfo("he-IL")
        );
    }
}
