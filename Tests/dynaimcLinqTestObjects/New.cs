using ExpressionTreeTestObjects;
using System;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    public static partial class DynamicLinqTestObjects {
        [TestObject("New")]
        internal static readonly Expression NewAnonymous = Expr(p => new { p.LastName, p.FirstName, p.Age });

        [TestObject("New")]
        internal static readonly Expression NewNamed = Expr(p => new DateTime(2001, 1, 1, 0,0,0, DateTimeKind.Local));

        [TestObject("New")]
        internal static readonly Expression NewNamed1 = Expr(p => new Uri("https://www.example.com/", UriKind.Absolute));
    }
}
