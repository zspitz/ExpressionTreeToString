using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static System.Linq.Expressions.Expression;

namespace ExpressionTreeTestObjects {
    partial class FactoryMethods {
        private static LabelTarget labelTarget = Label("target");

        [TestObject(Gotos)]
        internal static readonly Expression MakeBreak = Break(labelTarget);

        [TestObject(Gotos)]
        internal static readonly Expression MakeBreakWithValue = Break(labelTarget, Constant(5));

        [TestObject(Gotos)]
        internal static readonly Expression MakeContinue = Continue(labelTarget);

        [TestObject(Gotos)]
        internal static readonly Expression MakeGotoWithoutValue = Goto(labelTarget);

        [TestObject(Gotos)]
        internal static readonly Expression MakeGotoWithValue = Goto(labelTarget, Constant(5));

        [TestObject(Gotos)]
        internal static readonly Expression MakeReturn = Return(labelTarget);

        [TestObject(Gotos)]
        internal static readonly Expression MakeReturnWithValue = Return(labelTarget, Constant(5));
    }
}
