using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Functions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeTestObjects {
    partial class CSCompiler {
        [TestObject(Literal)]
        internal static readonly Expression True = Expr(() => true);

        [TestObject(Literal)]
        internal static readonly Expression False = Expr(() => false);

        [TestObject(Literal)]
        internal static readonly Expression Nothing = Expr(() => (string?)null);

        [TestObject(Literal)]
        internal static readonly Expression Integer = Expr(() => 5);

        [TestObject(Literal)]
        internal static readonly Expression NonInteger = Expr(() => 7.32);

        [TestObject(Literal)]
        internal static readonly Expression String = Expr(() => "abcd");

        [TestObject(Literal)]
        internal static readonly Expression InterpolatedString = Expr(() => $"{new DateTime(2001, 3, 25)}");

        [TestObject(Literal)]
        internal static readonly Expression Type = Expr(() => typeof(string));
    }
}