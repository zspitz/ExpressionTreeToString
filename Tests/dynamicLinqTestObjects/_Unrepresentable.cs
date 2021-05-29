using ExpressionTreeTestObjects;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    public static partial class DynamicLinqTestObjects {
        // the Dynamic LINQ `as` function can only be used with the ParameterExpression in scope
        // https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/457
        [TestObject("_Unrepresentable")]
        internal static readonly Expression PropertyTypeAs = Expr(p => p.LastName as object);

        // the Dynamic LINQ `is` function can only be used with the ParameterExpression in scope
        // https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/457
        [TestObject("_Unrepresentable")]
        internal static readonly Expression PropertyTypeIs = Expr(p => p.LastName is object);
    }
}
