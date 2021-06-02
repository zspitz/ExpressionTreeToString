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
            if (expr.NodeType.NotIn(RelationalOperators) || expr is not BinaryExpression bexpr) { return false; }

            static (Expression?, string?) enumOperand(Expression operand, Expression other, string pathSegment) {
                if (operand is not UnaryExpression uexpr) { return default; }
                if (uexpr.NodeType.NotIn(ExpressionType.Convert, ConvertChecked)) { return default; }
                var operandType = uexpr.Operand.Type;
                return
                    operandType.IsEnum && operandType.GetEnumUnderlyingType() == other.Type ?
                        (uexpr.Operand, $"{pathSegment}.Operand") :
                        default;
            }

            static Expression? getEnumValue(Expression other, Type enumType) => 
                other is ConstantExpression cexpr && Enum.IsDefined(enumType, cexpr.Value!) ?
                    Expression.Constant(Enum.Parse(enumType, cexpr.Value!.ToString()!)) :
                    null;

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

            // TODO what happens if neither side is null?

            results = (leftOperand!, leftPath!, rightOperand!, rightPath!);
            return true;
        }

        public static bool TryGetCharComparison(Expression expr, out (Expression left, string leftPath, Expression right, string rightPath) results) {
            // In C# compiler-generated expressions, an expression containing a comparison to a const char:
            //      () => "abcd"[0] == 'c'
            // will be generated with a conversion to int and compared against the integer value of the const char:
            //      () => (int)"abcd"[0] == 99 
            // we have to check both sides for such a comparison, and return the normalized expression

            results = default;
            if (expr.NodeType.NotIn(RelationalOperators) || expr is not BinaryExpression bexpr) { return false; }

            static (Expression?, Expression?) charOperand(Expression operand, Expression other) =>
                operand is UnaryExpression uexpr &&
                uexpr.NodeType.In(ExpressionType.Convert, ConvertChecked) &&
                uexpr.Operand.Type == typeof(char) &&
                other is ConstantExpression cexpr &&
                cexpr.Value is int i ?
                    (uexpr.Operand, Expression.Constant((char)i, typeof(char))) :
                    default;

            var (leftOperand, rightOperand) = charOperand(bexpr.Left, bexpr.Right);
            if (leftOperand is { }) {
                results = (leftOperand, "Left.Operand", rightOperand!, "Right");
                return true;
            }
            (rightOperand, leftOperand) = charOperand(bexpr.Right, bexpr.Left);
            if (rightOperand is { }) {
                results = (leftOperand!, "Left", rightOperand!, "Right.Operand");
                return true;
            }
            return false;
        }
    }
}
