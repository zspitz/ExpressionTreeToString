using ExpressionTreeTestObjects;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static ExpressionTreeTestObjects.Functions;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
        [TestObject(Member)]
        internal static readonly Expression StaticMember = Expr(p => string.Empty);

        [TestObject(Member)]
        internal static readonly Expression ParameterMember = Expr(p => p.LastName);

        [TestObject(Member)]
        internal static readonly Expression InstanceMember = Expr(p => p.LastName!.Length);

        [TestObject(Member)]
        internal static readonly Expression ClosureValue = IIFE(() => {
            var x = 5;
            return Expr(p => x);
        });
    }
}
