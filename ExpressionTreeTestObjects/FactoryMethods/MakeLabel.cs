using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Functions;
using static ExpressionTreeTestObjects.Categories;
using static System.Linq.Expressions.Expression;
using static ExpressionTreeTestObjects.Globals;

namespace ExpressionTreeTestObjects {
    partial class FactoryMethods {

        [TestObject(Labels)]
        // we're using variables here to force explicit blocks, which have indentation
        // in order to verify that the label is written without indentation
        internal static readonly Expression ConstructLabel = Block(
            new[] { i },
            Block(
                new[] { j },
                Constant(true),
                Label(Label("target")),
                Constant(true)
            )
        );

        [TestObject(Labels)]
        internal static readonly Expression ConstructLabel1 = Block(
            new[] { i },
            Block(
                new[] { j },
                Label(Label("target")),
                Constant(true)
            )
        );

        [TestObject(Labels)]
        internal static readonly LabelTarget ConstructLabelTarget = Label("target");

        [TestObject(Labels)]
        internal static readonly LabelTarget ConstructEmptyLabelTarget = Label("");

    }
}