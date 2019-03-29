﻿using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace ExpressionToString.Util {
    public static class ExpressionExtensions {
        public static object ExtractValue(this Expression expr) {
            if (!(expr is LambdaExpression lexpr)) {
                lexpr = Lambda(expr);
            }
            return lexpr.Compile().DynamicInvoke();
        }

        public static bool TryExtractValue(this Expression expr, out object value) {
            value = null;
            try {
                value = expr.ExtractValue();
                return true;
            } catch {}
            return false;
        }

        public static Expression SansConvert(this Expression expr) =>
            expr is UnaryExpression uexpr && uexpr.NodeType == ExpressionType.Convert ?
                uexpr.Operand :
                expr;

        public static bool IsEmpty(this Expression expr) =>
            expr is DefaultExpression && expr.Type == typeof(void);
    }
}
