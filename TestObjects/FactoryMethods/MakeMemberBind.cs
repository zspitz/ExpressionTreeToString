using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static System.Linq.Expressions.Expression;
using static System.Reflection.BindingFlags;
using System.Linq;
using static ExpressionTreeTestObjects.Functions;
using System.Reflection;

namespace ExpressionTreeTestObjects {
    internal class DummyMember {
        internal string? Foo { get; set; }
    }

    partial class FactoryMethods {

        [TestObject(MemberBindings)]
        internal static readonly MemberBinding MakeMemberBind = Bind(
            typeof(DummyMember).GetMember("Foo", Instance | NonPublic).Single(),
            Constant("abcd")
        );

        [TestObject(MemberBindings)]
        internal static readonly ElementInit MakeElementInit = ElementInit(
            GetMethod(() => ((List<string>?)null)!.Add("")),
            Constant("abcd")
        );

        [TestObject(MemberBindings)]
        internal static readonly ElementInit MakeElementInit2Arguments = ElementInit(
            GetMethod(() => ((Wrapper?)null)!.Add("", "")),
            Constant("abcd"),
            Constant("efgh")
        );

        [TestObject(MemberBindings)]
        internal static readonly MemberBinding MakeMemberMemberBind = MemberBind(
            GetMember(() => ((Node?)null)!.Data),
            Bind(
                GetMember(() => ((NodeData?)null)!.Name),
                Constant("abcd")
            )
        );

        static readonly MethodInfo addMethod = GetMethod(() => ((IList<Node>?)null)!.Add(new Node()));
        static readonly ConstructorInfo nodeConstructor = typeof(Node).GetConstructor(new Type[] { })!;

        [TestObject(MemberBindings)]
        internal static readonly MemberBinding MakeListBinding = ListBind(
            GetMember(() => ((Node?)null)!.Children),
            ElementInit(addMethod, New(nodeConstructor)),
            ElementInit(addMethod, New(nodeConstructor))
        );
    }
}