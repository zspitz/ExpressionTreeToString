using ExpressionTreeTestObjects;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
        [TestObject(Binary)]
        internal static readonly Expression Add = Expr(p => p.Age + 100);

        [TestObject(Binary)]
        internal static readonly Expression Divide = Expr(p => p.Age / 2);

        [TestObject(Binary)]
        internal static readonly Expression Modulo = Expr(p => p.Age % 10);

        [TestObject(Binary)]
        internal static readonly Expression Multiply = Expr(p => p.Age * 10);

        [TestObject(Binary)]
        internal static readonly Expression Subtract = Expr(p => p.Age - 10);

        [TestObject(Binary)]
        internal static readonly Expression AndAlso = Expr(p => p.Age >= 20 && p.Age <= 60);

        [TestObject(Binary)]
        internal static readonly Expression OrElse = Expr(p => p.Age < 20 || p.Age > 60);

        [TestObject(Binary)]
        internal static readonly Expression Equal = Expr(p => p.LastName == "A");

        [TestObject(Binary)]
        internal static readonly Expression NotEqual = Expr(p => p.LastName != "A");

        [TestObject(Binary)]
        internal static readonly Expression GreaterThanOrEqual = Expr(p => p.Age >= 13);

        [TestObject(Binary)]
        internal static readonly Expression GreaterThan = Expr(p => p.Age > 65);

        [TestObject(Binary)]
        internal static readonly Expression LessThan = Expr(p => p.Age < 20);

        [TestObject(Binary)]
        internal static readonly Expression LessThanOrEqual = Expr(p => p.Age <= 20);

        [TestObject(Binary)]
        internal static readonly Expression Coalesce = Expr(p => p.LastName ?? "");

        [TestObject(Binary)]
        internal static readonly Expression ArrayIndex= Expr(p => p.Relatives[4]);

        [TestObject(Binary)]
        internal static readonly Expression LogicalAnd = Expr(p => p.Age >= 20 & p.Age <= 60);

        [TestObject(Binary)]
        internal static readonly Expression LogicalOr = Expr(p => p.Age >= 20 | p.Age <= 60);
    }
}
