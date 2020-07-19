using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Functions;
using static ExpressionTreeTestObjects.Categories;
using System.Linq;

namespace ExpressionTreeTestObjects {
    internal static class Dummy {
        internal static void DummyMethod() { }
        internal static List<T> DummyExtensionMethod1Arguments<T>(this List<T> lst, int n) => lst;
        internal static List<T>? DummyExtensionMethod2Arguments<T>(this List<T>? lst, int n1, int n2) => lst;
    }

    partial class CSCompiler {

        [TestObject(Method)]
        internal static readonly Expression InstanceMethod0Arguments = IIFE(() => {
            var s = "";
            return Expr(() => s.ToString());
        });

        [TestObject(Method)]
        internal static readonly Expression StaticMethod0Arguments = Expr(() => Dummy.DummyMethod());

        [TestObject(Method)]
        internal static readonly Expression ExtensionMethod0Arguments = IIFE(() => {
            var lst = new List<string>();
            return Expr(() => lst.Count());
        });

        [TestObject(Method)]
        internal static readonly Expression InstanceMethod1Argument = IIFE(() => {
            var s = "";
            return Expr(() => s.CompareTo(""));
        });

        [TestObject(Method)]
        internal static readonly Expression StaticMethod1Argument = Expr(() => string.Intern(""));

        [TestObject(Method)]
        internal static readonly Expression ExtensionMethod1Argument = IIFE(() => {
            var lst = new List<string>();
            return Expr(() => lst.DummyExtensionMethod1Arguments(1));
        });

        [TestObject(Method)]
        internal static readonly Expression InstanceMethod2Arguments = IIFE(() => {
            var s = "abcde";
            return Expr(() => s.IndexOf('a', 2));
        });

        [TestObject(Method)]
        internal static readonly Expression StaticMethod2Arguments = Expr(() => string.Join(",", new[] { "a", "b" }));

        [TestObject(Method)]
        internal static readonly Expression ExtensionMethod2Arguments = IIFE(() => {
            var lst = new List<string>();
            return Expr(() => lst.DummyExtensionMethod2Arguments(5, 17));
        });

        [TestObject(Method)]
        internal static readonly Expression StringConcat = Expr((string s1, string s2) => string.Concat(s1, s2));

        [TestObject(Method)]
        internal static readonly Expression MathPow = Expr((double x, double y) => Math.Pow(x, y));
    }
}