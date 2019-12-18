using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static System.Linq.Expressions.Expression;
using static ExpressionTreeTestObjects.Globals;
using System.Linq;
using System.Collections;

namespace ExpressionTreeTestObjects {
    [ObjectContainer]
    internal static partial class FactoryMethods {
        [TestObject(Quoted)]
        internal static readonly Expression MakeQuoted = Block(
            new[] { x },
            Quote(
                Lambda(writeLineTrue)
            )
        );

        [TestObject(Quoted)]
        internal static readonly Expression MakeQuoted1 = Lambda(
            Quote(
                Lambda(writeLineTrue)
            )
        );

        [TestObject(DebugInfos)]
        internal static readonly Expression MakeDebugInfo = DebugInfo(document, 1, 2, 3, 4);

        [TestObject(DebugInfos)]
        internal static readonly Expression MakeClearDebugInfo = ClearDebugInfo(document);

        [TestObject(Loops)]
        internal static readonly Expression EmptyLoop = Loop(Constant(true));

        [TestObject(Loops)]
        internal static readonly Expression EmptyLoop1 = Loop(
             Block(
                 Constant(true),
                 Constant(true)
             )
         );

        [TestObject(Member)]
        internal static readonly Expression InstanceMember = MakeMemberAccess(
            Constant(""),
            typeof(string).GetMember("Length").Single()
        );

        [TestObject(Member)]
        internal static readonly Expression StaticMember = MakeMemberAccess(null, typeof(string).GetMember("Empty").Single());

        [TestObject(RuntimeVars)]
        internal static readonly Expression ConstructRuntimeVariables = RuntimeVariables(x, s1);

        [TestObject(RuntimeVars)]
        internal static readonly Expression RuntimeVariablesWithinBlock = Block(
            new[] { s2 }, //forces an explicit block
            Constant(true),
            RuntimeVariables(x, s1)
        );

        [TestObject(Defaults)]
        internal static readonly Expression MakeDefaultRefType = Default(typeof(string));

        [TestObject(Defaults)]
        internal static readonly Expression MakeDefaultValueType = Default(typeof(int));

        [TestObject(TypeChecks)]
        internal static readonly Expression MakeTypeCheck = TypeIs(
            Constant(""),
            typeof(string)
        );

        [TestObject(TypeChecks)]
        internal static readonly Expression MakeTypeEqual = TypeEqual(
            Constant(""),
            typeof(IEnumerable)
        );

        [TestObject(Invocation)]
        internal static readonly Expression MakeInvocation = Invoke(
            Lambda(Constant(5))
        );
    }
}
