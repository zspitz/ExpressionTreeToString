using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeToString.Globals;
using static System.Linq.Expressions.ExpressionType;
using ZSpitz.Util;

namespace ExpressionTreeToString {
    public abstract class CodeWriterBase : WriterBase {
        protected CodeWriterBase(object o, string language) : base(o, language) { }
        protected CodeWriterBase(object o, string language, out Dictionary<string, (int start, int length)> pathSpans) : base(o, language, out pathSpans) { }

        protected abstract void WriteBinary(ExpressionType nodeType, string leftPath, Expression left, string rightPath, Expression right);

        protected override void WriteBinary(BinaryExpression expr) {
            if (expr.NodeType.In(RelationalOperators)) {
                (Expression?, string?) enumOperand(Expression operand, Expression other, string pathSegment) {
                    (Expression?, string?) ret = (null, null);
                    if (!(operand is UnaryExpression uexpr)) { return ret; }
                    if (uexpr.NodeType.NotIn(ExpressionType.Convert, ConvertChecked)) { return ret; }
                    var operandType = uexpr.Operand.Type;
                    if (!operandType.IsEnum) { return ret; }
                    if (operandType.GetEnumUnderlyingType() != other.Type) { return ret; }
                    return (uexpr.Operand, $"{pathSegment}.Operand");
                }

                Expression? getEnumValue(Expression other, Type enumType) {
                    if (!(other is ConstantExpression cexpr)) { return null; }
                    if (!Enum.IsDefined(enumType, cexpr.Value)) { return null; }
                    return Expression.Constant(Enum.Parse(enumType, cexpr.Value.ToString()));
                }

                var (leftOperand, leftPath) = enumOperand(expr.Left, expr.Right, "Left");
                var (rightOperand, rightPath) = enumOperand(expr.Right, expr.Left, "Right");

                if (leftOperand is { } && rightOperand is null) {
                    rightOperand = getEnumValue(expr.Right, leftOperand.Type);
                    rightPath = "Right";
                } else if (leftOperand is null && rightOperand is { }) {
                    leftOperand = getEnumValue(expr.Left, rightOperand.Type);
                    leftPath = "Left";
                }

                if (leftOperand is { } && rightOperand is { }) {
                    WriteBinary(expr.NodeType, leftPath!, leftOperand, rightPath!, rightOperand);
                    return;
                }
            }

            WriteBinary(expr.NodeType, "Left", expr.Left, "Right", expr.Right);
        }
    }
}
