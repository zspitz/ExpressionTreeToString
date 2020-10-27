using System.Linq;
using System.Linq.Expressions;
using ExpressionTreeToString.Util;
using Xunit;
using static ExpressionTreeTestObjects.Functions;
using ZSpitz.Util;
using System;

namespace ExpressionTreeToString.Tests {
    public class Internals {
        public static readonly TheoryData<Expression, int, string[]> OrClausesData = new (Expression, int, string[])[] {
            (
                Expr((Person p) => p.FirstName == "A" || p.LastName == "D" || p.DOB!.Value.Hour > 10 || true),  
                4,
                new [] {
                    "Left.Left.Left",
                    "Left.Left.Right",
                    "Left.Right",
                    "Right"
                }
            ),
            (
                Expr((Person p) => p.FirstName == "A" || (p.LastName == "D" || p.DOB!.Value.Hour > 10) || true), 
                4,
                new string[] {
                    "Left.Left",
                    "Left.Right.Left",
                    "Left.Right.Right",
                    "Right"
                }
            ),
            (
                Expr((Person p) => true),
                1,
                new string[] {
                    ""
                }
            ),
            (
                Expr((Person p) => p.FirstName == "A" || (p.LastName == "D" || (p.DOB!.Value.Hour > 10 || true))),
                4,
                new string[] {
                    "Left",
                    "Right.Left",
                    "Right.Right.Left",
                    "Right.Right.Right"
                }
            )
        }.SelectT((expr, count, paths) => (((LambdaExpression)expr).Body, count, paths)).ToTheoryData();

        [MemberData(nameof(OrClausesData))]
        [Theory]
        public void TestOrClauses(Expression expr, int count, string[] paths) {
            var orClauses = expr.OrClauses().ToList();
            Assert.Equal(count,  orClauses.Count);

            var orClausesPaths = orClauses.Select(x => x.path).ToHashSet();
            Assert.Equal(paths.ToHashSet(), orClausesPaths);
        }

        public static readonly TheoryData<Expression, int> AndClausesData = new (Expression, int)[] {
            (Expr((Person p) => p.LastName!.CompareTo("A") > 0 && p.FirstName!.Length == 17 && p.DOB!.Value == DateTime.MinValue), 3),
            (Expr((Person p) => p.LastName!.CompareTo("A") > 0 && (p.FirstName!.Length == 17 && (p.DOB!.Value == DateTime.MinValue))), 3)
         }.SelectT((expr, count) => (((LambdaExpression)expr).Body, count)).ToTheoryData();   

        [MemberData(nameof(AndClausesData))]
        [Theory]
        public void TestAndClauses(Expression expr, int count) {
            var andClauses = expr.AndClauses().ToList();
            Assert.Equal(count, andClauses.Count);
        }
    }
}
