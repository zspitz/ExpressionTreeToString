using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ZSpitz.Util;
using static System.Linq.Expressions.ExpressionType;
using static ExpressionTreeToString.Globals;

namespace ExpressionTreeToString.Util {
    public static class Functions {
        // TODO indicate that ids is not going to be null after returning
        public static int GetId<T>(T e, [NotNull] ref Dictionary<T, int>? ids, int offset = 0) where T : notnull =>
            GetId(e, ref ids, out var _, offset);
        public static int GetId<T>(T e, [NotNull] ref Dictionary<T, int>? ids, out bool isNew, int offset = 0) where T : notnull {
            isNew = false;
            if (ids is null) {
                ids = new Dictionary<T, int>();
            }

            if (!ids.TryGetValue(e, out var id)) {
                isNew = true;
                id = ids.Count + offset;
                ids.Add(e, id);
            }

            return id;
        }

        public static string GetVariableName(ParameterExpression prm, ref Dictionary<ParameterExpression, int>? ids, string autonamedPrefix = "var") {
            var name = prm.Name;
            if (name.IsNullOrEmpty()) {
                return $"${autonamedPrefix}{GetId(prm, ref ids)}";
            }
            if (name.ContainsWhitespace()) {
                name = $"${name.ReplaceWhitespace("_")}";
            }
            return name;
        }

        public static bool TryGetEnumComparison(Expression expr, out (Expression left, string leftPath, Expression right, string rightPath) results) {
            // In compiler-generated expressions, a relational comparison used with an enum value:
            //      x => x.DOB.DayOfWeek == DayOfWeek.Tuesday
            // is represented as the comparison between an integer value and the enum converted to Int32:
            //      x => (int)x.DOB.DayOfWeek == 2
            // We check for this pattern in either direction of the BinaryExpression:
            //      x => (int)x.DOB.DayOfWeek == 2
            //      x => 2 == (int)x.DOB.DayOfWeek
            // and return the corresponding normalized parts of the expression

            results = default;
            if (!(expr.NodeType.In(RelationalOperators) && expr is BinaryExpression bexpr)) { return false; }

            static (Expression?, string?) enumOperand(Expression operand, Expression other, string pathSegment) {
                (Expression?, string?) ret = (null, null);
                if (!(operand is UnaryExpression uexpr)) { return ret; }
                if (uexpr.NodeType.NotIn(ExpressionType.Convert, ConvertChecked)) { return ret; }
                var operandType = uexpr.Operand.Type;
                return
                    operandType.IsEnum && operandType.GetEnumUnderlyingType() == other.Type ?
                        (uexpr.Operand, $"{pathSegment}.Operand") :
                        ret;
            }

            static Expression? getEnumValue(Expression other, Type enumType) {
                return
                    other is ConstantExpression cexpr && Enum.IsDefined(enumType, cexpr.Value) ?
                        Expression.Constant(Enum.Parse(enumType, cexpr.Value.ToString())) :
                        null;
            }

            var (leftOperand, leftPath) = enumOperand(bexpr.Left, bexpr.Right, "Left");
            var (rightOperand, rightPath) = enumOperand(bexpr.Right, bexpr.Left, "Right");

            if (leftOperand is null && rightOperand is null) {
                return false;
            } else if (leftOperand is { } && rightOperand is null) {
                rightOperand = getEnumValue(bexpr.Right, leftOperand.Type);
                rightPath = "Right";
            } else if (leftOperand is null && rightOperand is { }) {
                leftOperand = getEnumValue(bexpr.Left, rightOperand.Type);
                leftPath = "Left";
            }

            results = (leftOperand!, leftPath!, rightOperand!, rightPath!);
            return true;
        }
    }
}
