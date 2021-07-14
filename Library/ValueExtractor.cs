using System.Collections.Generic;
using System.Linq.Expressions;
using ZSpitz.Util;

namespace ExpressionTreeToString {
    public class ValueExtractor : ExpressionVisitor {
        private readonly Stack<Expression> expressionStack = new();
        private readonly Dictionary<Expression, bool> evaluables = new();

        public override Expression? Visit(Expression? node) {
            if (node is null) { return null; }

            expressionStack.Push(node);

            switch (node) {
                // AFAICT the only way to get a void-returning expression within an expression tree is as part of a BlockExpression
                // other expression types cannot contain a void-returning expression
                case Expression expr when expr.Type == typeof(void):
                    foreach (var x in expressionStack) {
                        // BlockExpressions may have a value even though one of the statements is a void-returning expression
                        if (x is BlockExpression && expr != x) { break; }
                        evaluables[x] = false;
                    }
                    break;

                // Nodes which contain the following expression types cannot be evaluated
                // ParameterExpression because it has no value
                // LoopExpression because it might be a never-ending loop
                case ParameterExpression _:
                case LoopExpression _:
                case Expression expr when expr.NodeType == ExpressionType.Extension && !expr.CanReduce:
                    foreach (var x in expressionStack) {
                        evaluables[x] = false;
                    }
                    break;

                case DefaultExpression _:
                case ConstantExpression _:
                case MethodCallExpression mcexpr when mcexpr.Arguments.None() && mcexpr.Object is null:
                case MemberExpression mexpr when mexpr.Expression is null:
                case NewExpression nexpr when nexpr.Arguments.None() && nexpr.Members!.None():
                case DebugInfoExpression _:
                case GotoExpression _:
                    foreach (var x in expressionStack) {
                        if (evaluables.ContainsKey(x)) { break; }

                        // LambdaExpression's value is the same as LambdaExpression.Body
                        // BlockExpression's value is the same as the last expression in the block
                        // in either case we will have gotten the value for the underlying expression; there's no need to do so again
                        evaluables[x] = !(x is LambdaExpression) && !(x is BlockExpression);
                    }
                    break;
            }

            var ret = node switch {
                null => null,
                var _ when node.NodeType == ExpressionType.Extension && !node.CanReduce => node,
                _ => base.Visit(node)
            };
            expressionStack.Pop();
            return ret;
        }

        public (bool evaluated, object? value) GetValue(Expression node) {
            if (!evaluables.TryGetValue(node, out var canEvaluate)) {
                Visit(node);
                evaluables.TryGetValue(node, out canEvaluate);
            }
            (var evaluated, object? value) = (false, null);
            if (canEvaluate) {
                evaluated = node.TryExtractValue(out value);
            }
            return (evaluated, value);
        }
    }
}
