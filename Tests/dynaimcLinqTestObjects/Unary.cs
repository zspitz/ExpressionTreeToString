using ExpressionTreeTestObjects;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    static public partial class DynamicLinqTestObjects {
        [TestObject("Unary")]
        internal static readonly Expression ImplicitConversion = Expr(p => (object)p);

        [TestObject("Unary")]
        internal static readonly Expression ImplicitConversion1 = Expr(p => (int)p.Age!);

        [TestObject("Unary")]
        internal static readonly Expression ExplicitConversion = Expr(p => (short)p.Age!);

        [TestObject("Unary")]
        internal static readonly Expression NegateBoolean = Expr(p => !(p.Age > 20));

        [TestObject("Unary")]
        internal static readonly Expression NegateNumeric = Expr(p => ~p.Age);
    }
}
