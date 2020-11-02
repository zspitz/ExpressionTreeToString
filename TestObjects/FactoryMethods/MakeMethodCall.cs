using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Functions;
using static ExpressionTreeTestObjects.Categories;
using static System.Linq.Expressions.Expression;
using static ExpressionTreeTestObjects.Globals;
using System.Linq;

namespace ExpressionTreeTestObjects {
    partial class FactoryMethods {
        [TestObject(Method)]
        internal static readonly Expression InstanceMethod0Arguments = Call(s, GetMethod(() => "".ToString()));

        [TestObject(Method)]
        internal static readonly Expression StaticMethod0Arguments = Call(GetMethod(() => Dummy.DummyMethod()));

        [TestObject(Method)]
        internal static readonly Expression ExtensionMethod0Arguments = Call(GetMethod(() => ((List<string>?)null)!.Count()), lstString);

        [TestObject(Method)]
        internal static readonly Expression InstanceMethod1Argument = Call(s, GetMethod(() => "".CompareTo("")), Constant(""));

        [TestObject(Method)]
        internal static readonly Expression StaticMethod1Argument = Call(GetMethod(() => string.Intern("")), Constant(""));

        [TestObject(Method)]
        internal static readonly Expression ExtensionMethod1Argument = Call(
            GetMethod(() => (null as List<string>)!.ExtensionMethod1Argument(0)), 
            lstString, 
            Constant(1)
        );

        [TestObject(Method)]
        internal static readonly Expression InstanceMethod2Arguments = Call(
            s,
            GetMethod(() => "".IndexOf('a', 2)),
            Constant('a'),
            Constant(2)
        );

        [TestObject(Method)]
        internal static readonly Expression StaticMethod2Arguments = Call(
            GetMethod(() => string.Join(",", new[] { "a", "b" })),
            Constant(","),
            NewArrayInit(typeof(string), Constant("a"), Constant("b"))
        );

        [TestObject(Method)]
        internal static readonly Expression ExtensionMethod2Arguments = IIFE(() => {
            var x = Parameter(typeof(string), "x");
            return Call(
                GetMethod(() => (null as List<string>)!.ExtensionMethod2Arguments(0,0)),
                lstString,
                Constant(5),
                Constant(17)
            );
        });

        [TestObject(Method)]
        internal static readonly Expression StringConcat = Call(
            GetMethod(() => string.Concat("", "")),
            s1,
            s2
        );
    }
}
