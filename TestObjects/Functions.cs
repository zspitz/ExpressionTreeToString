using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTreeTestObjects {
    public static class Functions {
        internal static MethodInfo GetMethod(Expression<Action> expr, params Type[] typeargs) {
            var ret = ((MethodCallExpression)expr.Body).Method;
            // TODO handle partially open generic methods
            if (typeargs.Any() && ret.IsGenericMethod) {
                ret = ret.GetGenericMethodDefinition().MakeGenericMethod(typeargs);
            }
            return ret;
        }

        internal static MemberInfo GetMember<T>(Expression<Func<T>> expr) =>
            ((MemberExpression)expr.Body).Member;

        public static LambdaExpression Expr<T>(Expression<Func<T>> expr) => expr;
        public static LambdaExpression Expr<T, T1>(Expression<Func<T, T1>> expr) => expr;
        public static LambdaExpression Expr<T, T1, T2>(Expression<Func<T, T1, T2>> expr) => expr;
        public static LambdaExpression Expr(Expression<Action> expr) => expr;
        public static LambdaExpression Expr<T>(Expression<Action<T>> expr) => expr;
        public static LambdaExpression Expr<T, T1>(Expression<Action<T, T1>> expr) => expr;
        public static T IIFE<T>(Func<T> fn) => fn();

    }
}
