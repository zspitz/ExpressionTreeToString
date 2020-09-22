using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static ZSpitz.Util.Functions;
using static System.Linq.Enumerable;
using static System.Linq.Expressions.ExpressionType;
using static System.Linq.Expressions.GotoExpressionKind;
using static ExpressionTreeToString.VBExpressionMetadata;
using ExpressionTreeToString.Util;
using OneOf;

namespace ExpressionTreeToString {
    public class VBWriterVisitor : CodeWriterVisitor {
        public VBWriterVisitor(object o, bool hasPathSpans = false) : base(o, Language.VisualBasic, hasPathSpans) { }

        private static readonly Dictionary<ExpressionType, string> simpleBinaryOperators = new Dictionary<ExpressionType, string>() {
            [Add] = "+",
            [AddChecked] = "+",
            [Divide] = "/",
            [Modulo] = "Mod",
            [Multiply] = "*",
            [MultiplyChecked] = "*",
            [Subtract] = "-",
            [SubtractChecked] = "-",
            [And] = "And",
            [Or] = "Or",
            [ExclusiveOr] = "Xor",
            [AndAlso] = "AndAlso",
            [OrElse] = "OrElse",
            [GreaterThanOrEqual] = ">=",
            [GreaterThan] = ">",
            [LessThan] = "<",
            [LessThanOrEqual] = "<=",
            [LeftShift] = "<<",
            [RightShift] = ">>",
            [Power] = "^",
            [Assign] = "=",
            [AddAssign] = "+=",
            [AddAssignChecked] = "+=",
            [DivideAssign] = "/=",
            [LeftShiftAssign] = "<<=",
            [MultiplyAssign] = "*=",
            [MultiplyAssignChecked] = "*=",
            [PowerAssign] = "^=",
            [RightShiftAssign] = ">>=",
            [SubtractAssign] = "-=",
            [SubtractAssignChecked] = "-="
        };

        protected override void WriteBinary(ExpressionType nodeType, string leftPath, Expression left, string rightPath, Expression right) {
            if (simpleBinaryOperators.TryGetValue(nodeType, out var @operator)) {
                Parens(nodeType, leftPath, left);
                Write($" {@operator} ");
                Parens(nodeType, rightPath, right);
                return;
            }

            switch (nodeType) {
                case ArrayIndex:
                    Parens(nodeType, leftPath, left);
                    Write("(");
                    WriteNode(rightPath, right);
                    Write(")");
                    return;
                case Coalesce:
                    Write("If(");
                    WriteNode(leftPath, left);
                    Write(", ");
                    WriteNode(rightPath, right);
                    Write(")");
                    return;
                case OrAssign:
                case AndAssign:
                case ExclusiveOrAssign:
                case ModuloAssign:
                    // these don't have a dedicated assigment operator
                    var op = (ExpressionType)Enum.Parse(typeof(ExpressionType), nodeType.ToString().Replace("Assign", ""));
                    WriteNode($"{leftPath}_0", left);
                    Write(" = ");
                    WriteNode(leftPath, left);
                    Write($" {simpleBinaryOperators[op]} ");
                    WriteNode(rightPath, right);
                    return;
                case Equal:
                    WriteNode(leftPath, left);
                    Write(" = "); // reference comparisons are handled in the WriterBase.WriteBinary overload
                    WriteNode(rightPath, right);
                    return;
                case NotEqual:
                    WriteNode(leftPath, left);
                    Write(" <> "); // reference comparisons are handled in the WriterBase.WriteBinary overload
                    WriteNode(rightPath, right);
                    return;
            }

            throw new NotImplementedException();
        }

        protected override void WriteBinary(BinaryExpression expr) {
            var isReferenceComparison = IsReferenceComparison(expr.NodeType, expr.Left, expr.Right, expr.Method is { });
            if (isReferenceComparison) {
                var op = expr.NodeType switch {
                    Equal => " Is ",
                    NotEqual => " IsNot ",
                    _ => throw new NotImplementedException()
                };
                WriteNode("Left", expr.Left);
                Write(op);
                Parens(expr, "Right", expr.Right);
                return;
            }

            base.WriteBinary(expr);
        }

        private static readonly Dictionary<Type, string> conversionFunctions = new Dictionary<Type, string>() {
            {typeof(bool), "CBool"},
            {typeof(byte), "CByte"},
            {typeof(char), "CChar"},
            {typeof(DateTime), "CDate"},
            {typeof(double), "CDbl"},
            {typeof(decimal), "CDec"},
            {typeof(int), "CInt"},
            {typeof(long), "CLng"},
            {typeof(object), "CObj"},
            {typeof(sbyte), "CSByte"},
            {typeof(short), "CShort"},
            {typeof(float), "CSng"},
            {typeof(string), "CStr"},
            {typeof(uint), "CUInt"},
            {typeof(ulong), "CULng"},
            {typeof(ushort), "CUShort" }
        };

        protected override void WriteUnary(ExpressionType nodeType, string operandPath, Expression operand, Type type, string expressionTypename) {
            switch (nodeType) {
                case ArrayLength:
                    Parens(nodeType, operandPath, operand);
                    Write(".Length");
                    break;
                case ExpressionType.Convert:
                case ConvertChecked:
                case Unbox:
                    if (type.IsAssignableFrom(operand.Type)) {
                        WriteNode(operandPath, operand);
                    } else if (conversionFunctions.TryGetValue(type, out var conversionFunction)) {
                        Write(conversionFunction);
                        Write("(");
                        WriteNode(operandPath, operand);
                        Write(")");
                    } else {
                        Write("CType(");
                        WriteNode(operandPath, operand);
                        Write($", {type.FriendlyName(language)})");
                    }
                    break;
                case Negate:
                case NegateChecked:
                    Write("-");
                    WriteNode(operandPath, operand);
                    break;
                case Not:
                    Write("Not ");
                    WriteNode(operandPath, operand);
                    break;
                case TypeAs:
                    Write("TryCast(");
                    WriteNode(operandPath, operand);
                    Write($", {type.FriendlyName(language)})");
                    break;

                case PreIncrementAssign:
                    Write("(");
                    WriteNode($"{operandPath}_0", operand);
                    Write(" += 1 : ");
                    WriteNode(operandPath, operand);
                    Write(")");
                    return;
                case PostIncrementAssign:
                    Write("(");
                    WriteNode($"{operandPath}_0", operand);
                    Write(" += 1 : ");
                    WriteNode(operandPath, operand);
                    Write(" - 1)");
                    return;
                case PreDecrementAssign:
                    Write("(");
                    WriteNode($"{operandPath}_0", operand);
                    Write(" -= 1 : ");
                    WriteNode(operandPath, operand);
                    Write(")");
                    return;
                case PostDecrementAssign:
                    Write("(");
                    WriteNode($"{operandPath}_0", operand);
                    Write(" -= 1 : ");
                    WriteNode(operandPath, operand);
                    Write(" + 1)");
                    return;

                case IsTrue:
                    WriteNode(operandPath, operand);
                    break;
                case IsFalse:
                    Write("Not ");
                    WriteNode(operandPath, operand);
                    break;

                case Increment:
                    WriteNode(operandPath, operand);
                    Write(" += 1");
                    break;
                case Decrement:
                    WriteNode(operandPath, operand);
                    Write(" -= 1");
                    break;

                case Throw:
                    Write("Throw");
                    if (operand != null) {
                        Write(" ");
                        WriteNode(operandPath, operand);
                    }
                    break;

                case Quote:
                    TrimEnd(true);
                    WriteEOL();
                    Write("' --- Quoted - begin");
                    Indent();
                    WriteEOL();
                    WriteNode(operandPath, operand);
                    WriteEOL(true);
                    Write("' --- Quoted - end");
                    break;

                case UnaryPlus:
                    Write("+");
                    WriteNode(operandPath, operand);
                    break;

                default:
                    throw new NotImplementedException($"NodeType: {nodeType}, Expression object type: {expressionTypename}");
            }
        }


        protected override void WriteLambda(LambdaExpression expr) {
            var lambdaKeyword = expr.ReturnType == typeof(void) ? "Sub" : "Function";
            Write($"{lambdaKeyword}(");
            expr.Parameters.ForEach((prm, index) => {
                if (index > 0) { Write(", "); }
                WriteNode($"Parameters[{index}]", prm, true);
            });
            Write(")");

            if (CanInline(expr.Body)) {
                Write(" ");
                WriteNode("Body", expr.Body);
                return;
            }

            Indent();
            WriteEOL();

            bool returnBlock = false;
            if (expr.Body.Type != typeof(void)) {
                if (expr.Body is BlockExpression bexpr && bexpr.HasMultipleLines()) {
                    returnBlock = true;
                } else {
                    Write("Return ");
                }
            }
            WriteNode("Body", expr.Body, CreateMetadata(true, Lambda, returnBlock));
            WriteEOL(true);
            Write($"End {lambdaKeyword}");
        }

        protected override void WriteParameterDeclaration(ParameterExpression prm) {
            if (prm.IsByRef) { Write("ByRef "); }
            Write($"{prm.Name} As {prm.Type.FriendlyName(language)}");
        }

        protected override void WriteNew(Type type, string argsPath, IList<Expression> args) {
            Write("New ");
            Write(type.FriendlyName(language));
            if (args.Count > 0) {
                Write("(");
                WriteNodes(argsPath, args);
                Write(")");
            }
        }

        protected override void WriteNew(NewExpression expr) {
            if (expr.Type.IsAnonymous()) {
                Write("New With {");
                Indent();
                WriteEOL();
                expr.Constructor.GetParameters().Select(x => x.Name).Zip(expr.Arguments).ForEachT((name, arg, index) => {
                    if (index > 0) {
                        Write(",");
                        WriteEOL();
                    }
                    // write as `.property = member` only if the source name is different from the target name
                    // otheriwse just write `member`
                    if (!(arg is MemberExpression mexpr && mexpr.Member.Name.Replace("$VB$Local_", "") == name)) {
                        Write($".{name} = ");
                    }
                    WriteNode($"Arguments[{index}]", arg);
                });
                WriteEOL(true);
                Write("}");
                return;
            }
            WriteNew(expr.Type, "Arguments", expr.Arguments);
        }

        static readonly MethodInfo power = typeof(Math).GetMethod("Pow");

        protected override void WriteCall(MethodCallExpression expr) {
            if (expr.Method.IsVBLike()) {
                WriteNode("Arguments[0]", expr.Arguments[0]);
                Write(" Like ");
                WriteNode("Arguments[1]", expr.Arguments[1]);
                return;
            }

            if (expr.Method == power) {
                WriteNode("Arguments[0]", expr.Arguments[0]);
                Write(" ^ ");
                WriteNode("Arguments[1]", expr.Arguments[1]);
                return;
            }

            base.WriteCall(expr);
        }

        protected override void WriteBinding(MemberBinding binding) {
            Write(".");
            Write(binding.Member.Name);
            Write(" = ");
            if (binding is MemberAssignment assignmentBinding) {
                WriteNode("Expression", assignmentBinding.Expression);
                return;
            }

            IEnumerable<object>? items = null;
            string initializerKeyword = "";
            string itemsPath = "";
            switch (binding) {
                case MemberListBinding listBinding when listBinding.Initializers.Count > 0:
                    items = listBinding.Initializers.Cast<object>();
                    initializerKeyword = "From ";
                    itemsPath = "Initializers";
                    break;
                case MemberMemberBinding memberBinding when memberBinding.Bindings.Count > 0:
                    items = memberBinding.Bindings.Cast<object>();
                    initializerKeyword = "With ";
                    itemsPath = "Bindings";
                    break;
            }

            Write($"{initializerKeyword}{{");

            if (items is { }) {
                Indent();
                WriteEOL();
                WriteNodes(itemsPath, items, true);
                WriteEOL(true);
            }

            Write("}");
        }

        protected override void WriteMemberInit(MemberInitExpression expr) {
            WriteNode("NewExpression", expr.NewExpression);
            if (expr.Bindings.Count > 0) {
                Write(" With {");
                Indent();
                WriteEOL();
                WriteNodes("Bindings", expr.Bindings, true);
                WriteEOL(true);
                Write("}");
            }
        }

        protected override void WriteListInit(ListInitExpression expr) {
            WriteNode("NewExpression", expr.NewExpression);
            Write(" From {");
            Indent();
            WriteEOL();
            WriteNodes("Initializers", expr.Initializers, true);
            WriteEOL(true);
            Write("}");
        }

        protected override void WriteNewArray(NewArrayExpression expr) {
            switch (expr.NodeType) {
                case NewArrayInit:
                    var elementType = expr.Type.GetElementType();
                    if (expr.Expressions.None() || expr.Expressions.Any(x => x.Type != elementType)) {
                        Write($"New {expr.Type.FriendlyName(language)} ");
                    }
                    Write("{ ");
                    expr.Expressions.ForEach((arg, index) => {
                        if (index > 0) { Write(", "); }
                        if (arg.NodeType == NewArrayInit) { Write("("); }
                        WriteNode($"Expressions[{index}]", arg);
                        if (arg.NodeType == NewArrayInit) { Write(")"); }
                    });
                    Write(" }");
                    break;
                case NewArrayBounds:
                    var nestedArrayTypes = expr.Type.NestedArrayTypes().ToList();
                    Write($"New {nestedArrayTypes.Last().root!.FriendlyName(language)}");
                    nestedArrayTypes.ForEachT((current, _, arrayTypeIndex) => {
                        Write("(");
                        if (arrayTypeIndex == 0) {
                            expr.Expressions.ForEach((x, index) => {
                                if (index > 0) { Write(", "); }

                                // The value(s) for an array-bounds initialization expression refer to the number of elements in the array, for the specified dimension.
                                // But in VB.NET, the number in an array-bounds initialization is the upper bound, not the number of elements.
                                // For example:
                                //
                                //    Dim arr = New String(5) {}
                                //
                                // produces an array with 6 elements; and the expression tree will be as follows:
                                //
                                //    NewArrayBounds(GetType(String), Constant(6))
                                //
                                // In order to get back the corresponding VB code, if the bounds is a constant, we can replace the constant.
                                // If the bounds is another expression, we can wrap in a subtraction expression.
                                // See also https://github.com/zspitz/ExpressionTreeToString/issues/32

                                Expression newExpr;
                                if (x is ConstantExpression cexpr) {
                                    object newValue = ((dynamic)cexpr.Value) - 1;
                                    newExpr = Expression.Constant(newValue);
                                } else {
                                    newExpr = Expression.SubtractChecked(x, Expression.Constant(1));
                                }

                                WriteNode($"Expressions[{index}]", newExpr);
                            });
                        } else {
                            Write(Repeat("", current.GetArrayRank()).Joined());
                        }
                        Write(")");
                    });
                    Write(" {}");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void WriteConditional(ConditionalExpression expr, object? metadata) {
            var parentIsConditional = ((VBExpressionMetadata?)metadata ?? CreateMetadata(false, null)).ExpressionType == Conditional;

            if (expr.Type != typeof(void)) {
                Write("If(");
                WriteNode("Test", expr.Test);
                Write(", ");
                WriteNode("IfTrue", expr.IfTrue);
                Write(", ");
                WriteNode("IfFalse", expr.IfFalse);
                Write(")");
                return;
            }

            var outgoingMetadata = CreateMetadata(true);

            if (CanInline(expr.Test)) {
                Write("If ");
                WriteNode("Test", expr.Test);
                Write(" Then");
            } else {
                Write("If");
                Indent();
                WriteEOL();
                WriteNode("Test", expr.Test, outgoingMetadata);
                WriteEOL(true);
                Write("Then");
            }

            var canInline = new[] { expr.IfTrue, expr.IfFalse }.All(x => CanInline(x)) && !parentIsConditional;
            if (canInline) {
                Write(" ");
                WriteNode("IfTrue", expr.IfTrue);
                if (!expr.IfFalse.IsEmpty()) {
                    Write(" Else ");
                    WriteNode("IfFalse", expr.IfFalse);
                }
                return;
            }

            Indent();
            WriteEOL();
            WriteNode("IfTrue", expr.IfTrue, outgoingMetadata);
            WriteEOL(true);
            if (expr.IfFalse.IsEmpty()) {
                Write("End If");
                return;
            }

            Write("Else");
            if (expr.IfFalse is ConditionalExpression) {
                Write(" ");
                WriteNode("IfFalse", expr.IfFalse, CreateMetadata(true, Conditional));
            } else {
                Indent();
                WriteEOL();
                WriteNode("IfFalse", expr.IfFalse, outgoingMetadata);
                WriteEOL(true);
                Write("End If");
            }
        }

        protected override void WriteDefault(DefaultExpression expr) =>
            Write($"CType(Nothing, {expr.Type.FriendlyName(language)})");

        protected override void WriteTypeBinary(TypeBinaryExpression expr) {
            switch (expr.NodeType) {
                case TypeIs:
                    Write("TypeOf ");
                    WriteNode("Expression", expr.Expression);
                    Write($" Is {expr.TypeOperand.FriendlyName(language)}");
                    break;
                case TypeEqual:
                    WriteNode("Expression", expr.Expression);
                    Write($".GetType = GetType({expr.TypeOperand.FriendlyName(language)})");
                    break;
            }
        }

        protected override void WriteBlock(BlockExpression expr, object? metadata) {
            var (isInMultiline, parentType, returnBlock) = (VBExpressionMetadata?)metadata ?? CreateMetadata(false, null, false);
            var useBlockConstruct = !isInMultiline ||
                (expr.Variables.Any() && parentType == Block);
            if (useBlockConstruct) {
                Write("Block");
                Indent();
                WriteEOL();
            }
            expr.Variables.ForEach((v, index) => {
                if (index > 0) { WriteEOL(); }
                Write("Dim ");
                WriteNode($"Variables[{index}]", v, true);
            });
            expr.Expressions.ForEach((subexpr, index) => {
                if (index > 0 || expr.Variables.Count > 0) { WriteEOL(); }
                if (subexpr is LabelExpression) { TrimEnd(); }

                var outgoingMetadata = CreateMetadata(true, Block);
                if (returnBlock && index == expr.Expressions.Count - 1) {
                    if (subexpr is BlockExpression bexpr && bexpr.HasMultipleLines()) {
                        outgoingMetadata = CreateMetadata(true, Block, true);
                    } else {
                        Write("Return ");
                    }
                }
                WriteNode($"Expressions[{index}]", subexpr, outgoingMetadata);
            });
            if (useBlockConstruct) {
                WriteEOL(true);
                Write("End Block");
            }
        }

        private bool CanInline(Expression expr) {
            switch (expr) {
                case ConditionalExpression cexpr when cexpr.Type == typeof(void):
                case BlockExpression bexpr when
                    bexpr.Expressions.Count > 1 ||
                    bexpr.Variables.Any() ||
                    (bexpr.Expressions.Count == 1 && CanInline(bexpr.Expressions.First())):
                case SwitchExpression _:
                case LambdaExpression _:
                case TryExpression _:
                case Expression _ when expr.NodeType == Quote:
                    return false;
                case RuntimeVariablesExpression _:
                    throw new NotImplementedException();
            }
            return true;
        }

        protected override void WriteSwitchCase(SwitchCase switchCase) {
            Write("Case ");
            WriteNodes("TestValues", switchCase.TestValues);
            Indent();
            WriteEOL();
            WriteNode("Body", switchCase.Body, CreateMetadata(true, null));
            Dedent();
        }

        protected override void WriteSwitch(SwitchExpression expr) {
            Write("Select Case ");
            Indent();
            WriteNode("SwitchValue", expr.SwitchValue);
            WriteEOL();
            WriteNodes("Cases", expr.Cases, true, "");
            if (expr.DefaultBody != null) {
                if (expr.Cases.Count > 0) { WriteEOL(); }
                Write("Case Else");
                Indent();
                WriteEOL();
                WriteNode("DefaultBody", expr.DefaultBody, CreateMetadata(true, Switch));
                Dedent();
            }
            WriteEOL(true);
            Write("End Select");
        }

        protected override void WriteCatchBlock(CatchBlock catchBlock) {
            Write("Catch");
            if (catchBlock.Variable != null) {
                Write(" ");
                WriteNode("Variable", catchBlock.Variable, true);
            } else if (catchBlock.Test != null && catchBlock.Test != typeof(Exception)) {
                Write($" _ As {catchBlock.Test.FriendlyName(language)}");
            }
            if (catchBlock.Filter != null) {
                Write(" When ");
                WriteNode("Filter", catchBlock.Filter);
            }
            Indent();
            WriteEOL();
            WriteNode("Body", catchBlock.Body, CreateMetadata(true, null));
        }

        protected override void WriteTry(TryExpression expr) {
            Write("Try");
            Indent();
            WriteEOL();
            WriteNode("Body", expr.Body, CreateMetadata(true, Try));
            WriteEOL(true);
            expr.Handlers.ForEach((catchBlock, index) => {
                WriteNode($"Handlers[{index}]", catchBlock);
                WriteEOL(true);
            });
            if (expr.Fault != null) {
                Write("Fault");
                Indent();
                WriteEOL();
                WriteNode("Fault", expr.Fault, CreateMetadata(true, Try));
                WriteEOL(true);
            }
            if (expr.Finally != null) {
                Write("Finally");
                Indent();
                WriteEOL();
                WriteNode("Finally", expr.Finally, CreateMetadata(true, Try));
                WriteEOL(true);
            }
            Write("End Try");
        }

        protected override void WriteLabel(LabelExpression expr) {
            WriteNode("Target", expr.Target);
            Write(":");
        }

        protected override void WriteGoto(GotoExpression expr) {
            var gotoKeyword = expr.Kind switch {
                Break => "Exit",
                Continue => "Continue",
                GotoExpressionKind.Goto => "Goto",
                Return => "Return",
                _ => throw new NotImplementedException(),
            };
            Write(gotoKeyword);
            if (!(expr.Target?.Name).IsNullOrWhitespace()) {
                Write(" ");
                WriteNode("Target", expr.Target);
            }
            if (expr.Value != null) {
                Write(" ");
                WriteNode("Value", expr.Value);
            }
        }

        protected override void WriteLabelTarget(LabelTarget labelTarget) => Write(labelTarget.Name);

        protected override void WriteLoop(LoopExpression expr) {
            Write("Do");
            Indent();
            WriteEOL();
            WriteNode("Body", expr.Body, CreateMetadata(true, Loop));
            WriteEOL(true);
            Write("Loop");
        }

        protected override void WriteRuntimeVariables(RuntimeVariablesExpression expr) {
            Write("' Variables -- ");
            expr.Variables.ForEach((x, index) => {
                if (index > 0) { Write(", "); }
                WriteNode($"Variables[{index}]", x, true);
            });
        }

        protected override void WriteDebugInfo(DebugInfoExpression expr) {
            var filename = expr.Document.FileName;
            Write("' ");
            var comment =
                expr.IsClear ?
                $"Clear debug info from {filename}" :
                $"Debug to {filename} -- L{expr.StartLine}C{expr.StartColumn} : L{expr.EndLine}C{expr.EndColumn}";
            Write(comment);
        }

        protected override (string prefix, string suffix) GenericIndicators => ("(Of ", ")");

        protected override bool ParensOnEmptyArguments => false;

        protected override (string prefix, string suffix) IndexerIndicators => ("(", ")");

        // to verify precendence levels by level against https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/operators/operator-precedence#precedence-order
        // use:
        //    precedence.GroupBy(kvp => kvp.Value, kvp => kvp.Key, (key, grp) => new {key, values = grp.OrderBy(x => x.ToString()).Joined(", ")}).OrderBy(x => x.key);
        private Dictionary<ExpressionType, int> precedence = new Dictionary<ExpressionType, int>() {
            [Add] =6,
            [AddAssign] =-1,
            [AddAssignChecked] =-1,
            [AddChecked] =6,
            [And] =11,
            [AndAlso] =11,
            [AndAssign] =-1,
            [ArrayIndex] =-1,
            [ArrayLength] =-1, // member access in VB.NET
            [Assign] =-1,
            [Block] = -1,
            [Call] = 0, // precedence of the . operator, not the arguments, which are in any case wrapped in parentheses
            [Coalesce] =-1,
            [Conditional] = -1,
            [Constant] = -1,
            [ExpressionType.Convert] =-1,
            [ConvertChecked] =-1,
            [DebugInfo] = -1,
            [Decrement] =-1,
            [Default] =-1,
            [Divide] =3,
            [DivideAssign] =-1,
            [Dynamic] = -1,
            [Equal] =9,
            [ExclusiveOr] =13,
            [ExclusiveOrAssign] =-1,
            [Extension] = -1,
            [ExpressionType.Goto] = -1,
            [GreaterThan] =9,
            [GreaterThanOrEqual] =9,
            [Increment] =-1,
            [Index] =-1,
            [Invoke] =-1,
            [IsFalse] =-1,
            [IsTrue] = -1,
            [Label] = -1,
            [Lambda] =-1,
            [LeftShift] =8,
            [LeftShiftAssign] =-1,
            [LessThan] =9,
            [LessThanOrEqual] =9,
            [ListInit] = -1,
            [Loop] = -1,
            [MemberAccess] =-1,
            [MemberInit] = -1,
            [Modulo] =5,
            [ModuloAssign] =-1,
            [Multiply] =3,
            [MultiplyAssign] =-1,
            [MultiplyAssignChecked] =-1,
            [MultiplyChecked] =-1,
            [Negate] =2,
            [NegateChecked] =2,
            [New] =-1,
            [NewArrayBounds] =-1, // same as New
            [NewArrayInit] =-1, // same as New
            [Not] =10,
            [NotEqual] =9,
            [OnesComplement] =2,
            [Or] =12,
            [OrAssign] =-1,
            [OrElse] =12,
            [Parameter] = -1,
            [PostDecrementAssign] =-1,
            [PostIncrementAssign] =-1,
            [Power] =1, 
            [PowerAssign] =-1,
            [PreDecrementAssign] =-1,
            [PreIncrementAssign] =-1,
            [Quote] = -1,
            [RightShift] =8,
            [RightShiftAssign] =-1,
            [RuntimeVariables] = -1,
            [Subtract] =6,
            [SubtractAssign] =-1,
            [SubtractAssignChecked] =-1,
            [SubtractChecked] =6,
            [Switch] = -1,
            [Throw] = -1,
            [Try] = -1,
            [TypeAs] =-1,
            [TypeEqual] = 9, // like Equal
            [TypeIs] =9,
            [UnaryPlus] =2,
            [Unbox] = -1,
        };

        private int GetPrecedence(Expression node) => node switch
        {
            MethodCallExpression mexpr when mexpr.Method.IsStringConcat() => 7,
            MethodCallExpression mexpr when mexpr.Method.IsVBLike() => 9,
            _ => precedence[node.NodeType]
        };

        protected override void Parens(OneOf<Expression, ExpressionType> arg, string path, Expression childNode) {
            var parentPrecedence = arg.Match(
                node => GetPrecedence(node),
                nodeType => precedence[nodeType]
            );
            var childPrecedence = GetPrecedence(childNode);

            // Operators in VB are left-associative, except for assignment which cannot be nested
            // We thus consider everything left-associative

            bool writeParens = false;
            if (parentPrecedence != -1 && childPrecedence != -1) {
                writeParens =
                    (childPrecedence > parentPrecedence) ||
                    (parentPrecedence == childPrecedence && childNode is BinaryExpression && path == "Right");
            }

            if (writeParens) { Write("("); }
            WriteNode(path, childNode);
            if (writeParens) { Write(")"); }
        }
    }
}
