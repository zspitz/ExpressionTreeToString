using System;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Util {
    internal static class ExpressionExtensions {
        internal static void Deconstruct(this Expression expr, out ExpressionType nodeType, out Type type) =>
            (nodeType, type) = (expr.NodeType, expr.Type);
    }
}
