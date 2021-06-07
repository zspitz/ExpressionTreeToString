using ExpressionTreeTestObjects;
using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
        [TestObject(Constants)]
        internal static readonly Expression DateTime1 = Constant(new DateTime(2001, 1, 1));

        [TestObject(Constants)]
        internal static readonly Expression DateTime2 = Constant(new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        [TestObject(Constants)]
        internal static readonly Expression Random = Constant(new Random());
    }
}
