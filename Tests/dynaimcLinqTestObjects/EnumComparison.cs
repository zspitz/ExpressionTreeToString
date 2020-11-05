using ExpressionTreeTestObjects;
using System;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
        [TestObject(EnumComparison)]
        internal static readonly Expression LeftEnumNonConstant = Expr(p => p.DOB!.Value.DayOfWeek >= DayOfWeek.Tuesday);

        [TestObject(EnumComparison)]
        internal static readonly Expression RightEnumNonConstant = Expr(p => DayOfWeek.Tuesday >= p.DOB!.Value.DayOfWeek);

        [TestObject(EnumComparison)]
        internal static readonly Expression DualNonConstant = Expr(p => p.RegisteredOn!.Value.DayOfWeek <= p.DOB!.Value.DayOfWeek);

        [TestObject(EnumComparison)]
        internal static readonly Expression InLeftEnumNonConstant = Expr(p => 
            p.DOB!.Value.DayOfWeek == DayOfWeek.Thursday ||
            p.DOB!.Value.DayOfWeek == DayOfWeek.Monday
        );

        [TestObject(EnumComparison)]
        internal static readonly Expression InRightEnumNonConstant = Expr(p => 
            DayOfWeek.Tuesday == p.DOB!.Value.DayOfWeek ||
            DayOfWeek.Tuesday == p.RegisteredOn!.Value.DayOfWeek
        );

        [TestObject(EnumComparison)]
        internal static readonly Expression InConstantOrNonConstant = Expr(p => 
            p.DOB!.Value.DayOfWeek == p.RegisteredOn!.Value.DayOfWeek ||
            p.DOB!.Value.DayOfWeek == DayOfWeek.Thursday
        );
    }
}
