using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static System.Linq.Expressions.Expression;
using static ExpressionTreeTestObjects.Globals;

namespace ExpressionTreeTestObjects {
    partial class FactoryMethods {
        [TestObject(SwitchCases)]
        internal static readonly SwitchCase SingleValueSwitchCase = SwitchCase(
            Block(writeLineTrue, writeLineTrue),
            Constant(5)
        );

        [TestObject(SwitchCases)]
        internal static readonly SwitchCase MultiValueSwitchCase = SwitchCase(
            Block(writeLineTrue, writeLineTrue),
            Constant(5),
            Constant(6)
        );

        [TestObject(SwitchCases)]
        internal static readonly SwitchCase SingleValueSwitchCase1 = SwitchCase(writeLineTrue, Constant(5));

        [TestObject(SwitchCases)]
        internal static readonly SwitchCase MultiValueSwitchCase1 = SwitchCase(writeLineTrue, Constant(5), Constant(6));

        [TestObject(SwitchCases)]
        internal static readonly Expression SwitchOnExpressionWithDefaultSingleStatement = Switch(i, Empty(),
            SwitchCase(
                writeLineTrue,
                Constant(4)
            ), SwitchCase(
                writeLineFalse,
                Constant(5)
            )
        );

        [TestObject(SwitchCases)]
        internal static readonly Expression SwitchOnExpressionWithDefaultMultiStatement = Switch(i,
            Block(
                typeof(void),
                Constant(true),
                Constant(true)
            ), SwitchCase(
                writeLineTrue,
                Constant(4)
            ), SwitchCase(
                writeLineFalse,
                Constant(5)
            )
        );

        [TestObject(SwitchCases)]
        internal static readonly Expression SwitchOnMultipleStatementsWithDefault = Switch(Block(i, j), Block(
                typeof(void),
                Constant(true),
                Constant(true)
            ), SwitchCase(
                writeLineTrue,
                Constant(4)
            ), SwitchCase(
                writeLineFalse,
                Constant(5)
            )
        );

        [TestObject(SwitchCases)]
        internal static readonly Expression SwitchOnExpressionWithoutDefault = Switch(i, SwitchCase(
                writeLineTrue,
                Constant(4)
            ), SwitchCase(
                writeLineFalse,
                Constant(5)
            )
        );

        [TestObject(SwitchCases)]
        internal static readonly Expression SwitchOnMultipleStatementsWithoutDefault = Switch(Block(i, j), SwitchCase(
                writeLineTrue,
                Constant(4)
            ), SwitchCase(
                writeLineFalse,
                Constant(5)
            )
        );

        [TestObject(SwitchCases)]
        internal static readonly Expression NonVoidSwitch = Switch(
            i,
            Constant("Default"),
            SwitchCase(Constant("One"), Constant(1)),
            SwitchCase(Constant("Two"), Constant(2)),
            SwitchCase(Constant("Three"), Constant(3))
        );

        [TestObject(SwitchCases)]
        internal static readonly Expression NonVoidSwitchMultilineBodies = Switch(
            i,
            Block(
                Constant(null, typeof(string)),
                Constant("Default")
            ),
            SwitchCase(
                Block(
                    Constant(null, typeof(string)),
                    Constant("One")
                ),
                Constant(1)
            ),
            SwitchCase(
                Block(
                    Constant(null, typeof(string)),
                    Constant("Two")
                ),
                Constant(2)
            )
        );

        [TestObject(SwitchCases)]
        internal static readonly Expression NonVoidSwitchMultipleTestValues = Switch(
            i,
            Constant("Default"),
            SwitchCase(
                Constant("OneTwo"),
                Constant(1),
                Constant(2)
            ),
            SwitchCase(
                Constant("ThreeFour"),
                Constant(3),
                Constant(4)
            )
        );

    }
}