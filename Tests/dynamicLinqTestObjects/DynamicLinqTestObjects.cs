using ExpressionTreeTestObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace ExpressionTreeToString.Tests {
    [ObjectContainer]
    public static partial class DynamicLinqTestObjects {
        internal static Expression Expr<T>(Expression<Func<Person, T>> expr) => expr;
        internal static Expression Expr(Expression<Action<Person>> expr) => expr;

        internal static Expression Expr(string selector, params object[] constants) =>
            new ExpressionParser(
                new[] { Parameter(typeof(Person)) },
                selector,
                constants,
                ParsingConfig.Default
            ).Parse(null);

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression Parameter = Expr(p => p);

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression In = Expr(p => p.LastName == "A" || p.LastName == "B");

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression MultipleFieldsIn = Expr(p => p.LastName == "A" || p.LastName == "B" || p.FirstName == "C" || p.FirstName == "D");

        // tests for multiple fields, some of which have a single value, and some of which have multiple vales
        [TestObject("Dynamic LINQ")]
        internal static readonly Expression MultipleFieldSingleValueIn = Expr(p => p.LastName == "A" || p.LastName == "B" || p.DOB == DateTime.MinValue || p.FirstName == "C" || p.FirstName == "D");

        // pending https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/441
        //[TestObject("Invocation")]
        //internal static readonly Expression Invocation = Expr(p => ((Func<string>)(() => p.LastName + p.FirstName))());

        // pending https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/441
        //[TestObject("Invocation")]
        //internal static readonly Expression InvocationArguments = Expr(p => ((Func<Person, string>)(p1 => p1.LastName + p1.FirstName))(p));

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression ParameterTypeAs = Expr(p => p as object);

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression ParameterTypeIs = Expr(p => p is object);

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression StringEscaping = Expr("\"\\\"\"");

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression CharEscaping = Expr("'\"'");

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression StringEscapingLambda = Expr(p => "\"");

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression CharEscapingLambda = Expr(p => '"');

        public static readonly Dictionary<string, object[]> Parameters = new() {
            { nameof(Random), new[] { new Random() } },
            { nameof(NpChainWithMethodsParameters), new[] {
                CultureInfo.GetCultureInfo("en-US")
            }},
            { nameof(NpChainWithMethodsParameters1), new[] {
                CultureInfo.GetCultureInfo("en-US"),
                CultureInfo.GetCultureInfo("he-IL")
            }},
            { nameof(Contains1), Array.Empty<string>() },
            { nameof(ClosureValue), new object [] {5}}
        };
    }
}
