using ExpressionTreeTestObjects;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    public static partial class DynamicLinqTestObjects {
        [TestObject("Member")]
        internal static readonly Expression StaticMember = Expr(p => string.Empty);

        [TestObject("Member")]
        internal static readonly Expression ParameterMember = Expr(p => p.LastName);

        [TestObject("Member")]
        internal static readonly Expression InstanceMember = Expr(p => p.LastName!.Length);
    }
}
