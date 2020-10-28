using ExpressionTreeTestObjects;
using System;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    [ObjectContainer]
    static public partial class DynamicLinqTestObjects {
        static internal Expression Expr<T>(Expression<Func<Person, T>> expr) => expr;
        static internal Expression Expr(Expression<Action<Person>> expr) => expr;

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression Parameter = Expr(p => p);

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression In = Expr(p => p.LastName == "A" || p.LastName == "B");

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression MultipleFieldsIn = Expr(p => p.LastName == "A" || p.LastName == "B" || p.FirstName == "C" || p.FirstName == "D");

        // tests for multiple fields, some of which have a single value, and some of which have multiple vales
        [TestObject("Dynamic LINQ")]
        internal static readonly Expression MultipleFieldSingleValueIn = Expr(p => p.LastName == "A" || p.LastName == "B" || p.DOB == DateTime.MinValue || p.FirstName == "C" || p.FirstName == "D");

        [TestObject("Invocation")]
        internal static readonly Expression Invocation = Expr(p => p.Notify());

        [TestObject("Invocation")]
        internal static readonly Expression InvocationArguments = Expr(p => p.Notify1(true));
    }
}
