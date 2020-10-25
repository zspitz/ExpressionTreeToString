using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZSpitz.Util;
using static System.Linq.Expressions.ExpressionType;

namespace ExpressionTreeToString.Util {
    internal static class ExpressionExtensions {
        internal static void Deconstruct(this Expression expr, out ExpressionType nodeType, out Type type) =>
            (nodeType, type) = (expr.NodeType, expr.Type);

        private static IEnumerable<(string path, Expression clause)> LogicalCombinedClauses(string path, Expression expr, params ExpressionType[] nodeTypes) {
            // The return type cannot be IEnumerable<BinaryExpression> because it might contain any bool-returning expression
            if (expr.NodeType.NotIn(nodeTypes) || expr.Type != typeof(bool)) {
                yield return (path, expr);
                yield break;
            }

            var bexpr = (BinaryExpression)expr;
            (string path, Expression expr)[] parts = new[] {
                ("Left", bexpr.Left),
                ("Right", bexpr.Right)
            };

            foreach (var (path1, expr1) in parts.SelectMany(x => LogicalCombinedClauses(x.path, x.expr, nodeTypes))) {
                yield return (path1, expr1);
            }
        }

        internal static IEnumerable<Expression> AndClauses(this Expression expr) => LogicalCombinedClauses("", expr, And, AndAlso).Select(x => x.clause);
        internal static IEnumerable<(string path, Expression clause)> OrClauses(this Expression expr) => LogicalCombinedClauses("", expr, Or, OrElse);

        internal static IEnumerable<MemberExpression> MemberClauses(this Expression expr) {
            if (!(expr is MemberExpression mexpr)) {
                yield break;
            }
            foreach (var item in MemberClauses(mexpr.Expression)) {
                yield return item;
            }
            yield return mexpr;
        }
    }
}
