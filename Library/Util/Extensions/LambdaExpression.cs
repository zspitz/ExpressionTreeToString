using System.Linq.Expressions;

namespace ExpressionTreeToString.Util {
    public static class LambdaExpressionExtensions {
        public static object GetTarget(this LambdaExpression expr) => expr.Compile().Target;
    }
}
