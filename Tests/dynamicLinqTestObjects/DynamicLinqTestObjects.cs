using ExpressionTreeTestObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using static System.Linq.Dynamic.Core.DynamicExpressionParser;
using System.Linq;

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

        internal static LambdaExpression Lambda(string selector, params object[] constants) =>
            ParseLambda(new[] { Parameter(typeof(Person)) }, null, selector, constants);

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

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression QueryableWhere =
            Enumerable.Empty<Person>()
                .AsQueryable()
                .Where(x => x.Age <= 20)
                .Expression;

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression QueryableMultiple =
            Enumerable.Empty<Person>()
                .AsQueryable()
                .Where(x => x.Age <= 20)
                .OrderBy(x => x.LastName != null ? x.LastName[0] : ' ')
                .ThenBy(x => x.LastName != null ? x.LastName[0] : ' ')
                .Expression;

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression QueryableTake =
            Enumerable.Empty<Person>().AsQueryable().Take(5).Expression;

        public static readonly Dictionary<string, object[]> Parameters = new() {
            { nameof(Random), new[] { new Random() } },
            { nameof(NpChainWithMethodsParameters), new[] {
                CultureInfo.GetCultureInfo("en-US")
            }},
            { nameof(NpChainWithMethodsParameters1), new[] {
                CultureInfo.GetCultureInfo("en-US"),
                CultureInfo.GetCultureInfo("he-IL")
            }},
            { nameof(Contains1), new object[] { Array.Empty<string>() } },
            { nameof(ClosureValue), new object [] {5}},
            { nameof(LambdaRandom), new[] {new Random()}}
        };
    }
}
