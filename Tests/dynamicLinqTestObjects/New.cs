using ExpressionTreeTestObjects;
using System;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
        [TestObject(NewObject)]
        internal static readonly Expression NewAnonymous = Expr(p => new { p.LastName, p.FirstName, p.Age });

        [TestObject(NewObject)]
        internal static readonly Expression NewNamed = Expr(p => new DateTime(2001, 1, 1, 0,0,0, DateTimeKind.Local));

        [TestObject(NewObject)]
        internal static readonly Expression NewNamed1 = Expr(p => new Uri("https://www.example.com/", UriKind.Absolute));
    }
}
