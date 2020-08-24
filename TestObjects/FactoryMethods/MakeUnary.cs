using System;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static System.Linq.Expressions.Expression;
using static ExpressionTreeTestObjects.Globals;

namespace ExpressionTreeTestObjects {
    internal static partial class FactoryMethods {
        [TestObject(Unary)]
        internal static readonly Expression ConstructArrayLength = ArrayLength(arr);

        [TestObject(Unary)]
        internal static readonly Expression ConstructConvert = Convert(arr, typeof(object));

        [TestObject(Unary)]
        internal static readonly Expression ConstructConvertChecked = ConvertChecked(Constant(5), typeof(float));

        [TestObject(Unary)]
        internal static readonly Expression ConstructConvertCheckedForReferenceType = ConvertChecked(arr, typeof(object));

        [TestObject(Unary)]
        internal static readonly Expression ConstructNegate = Negate(i);

        [TestObject(Unary)]
        internal static readonly Expression ConstructBitwiseNot = Not(i);

        [TestObject(Unary)]
        internal static readonly Expression ConstructLogicalNot = Not(b1);

        [TestObject(Unary)]
        internal static readonly Expression ConstructTypeAs = TypeAs(arr, typeof(object));

        [TestObject(Unary)]
        internal static readonly Expression ConstructPostDecrementAssign = PostDecrementAssign(i);

        [TestObject(Unary)]
        internal static readonly Expression ConstructPostIncrementAssign = PostIncrementAssign(i);

        [TestObject(Unary)]
        internal static readonly Expression ConstructPreDecrementAssign = PreDecrementAssign(i);

        [TestObject(Unary)]
        internal static readonly Expression ConstructPreIncrementAssign = PreIncrementAssign(i);

        [TestObject(Unary)]
        internal static readonly Expression ConstructIsTrue = IsTrue(b1);

        [TestObject(Unary)]
        internal static readonly Expression ConstructIsFalse = IsFalse(b1);

        [TestObject(Unary)]
        internal static readonly Expression ConstructIncrement = Increment(i);

        [TestObject(Unary)]
        internal static readonly Expression ConstructDecrement = Decrement(i);

        [TestObject(Unary)]
        internal static readonly Expression ConstructThrow = Throw(Constant(new Random()));

        [TestObject(Unary)]
        internal static readonly Expression ConstructRethrow = Rethrow();

        [TestObject(Unary)]
        internal static readonly Expression ConstructUnbox = Unbox(obj, typeof(int));
      }
}
