using ExpressionTreeTestObjects;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
        [TestObject("OperationGrouping")]
        internal static readonly Expression HigherBinaryPrecedence = Expr("(@0 + @1) * @2", 1, 2, 3);

        [TestObject("OperationGrouping")]
        internal static readonly Expression BinaryPrecedence = Expr("(@0 + @1).ToString()", 1, 2);
    }
}
