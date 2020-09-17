using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Enumerable;
using static System.Linq.Expressions.ExpressionType;
using static System.Linq.Expressions.GotoExpressionKind;
using static ExpressionTreeToString.CSharpMultilineBlockTypes;
using static ExpressionTreeToString.CSharpBlockMetadata;

namespace ExpressionTreeToString {
    public class CSharpWriterVisitor : CodeWriterVisitor {
        public CSharpWriterVisitor(object o, bool hasPathSpans = false) : base(o, Language.CSharp, hasPathSpans) { }

        private static readonly Dictionary<ExpressionType, string> simpleBinaryOperators = new Dictionary<ExpressionType, string>() {
            [Add] = "+",
            [AddChecked] = "+",
            [Divide] = "/",
            [Modulo] = "%",
            [Multiply] = "*",
            [MultiplyChecked] = "*",
            [Subtract] = "-",
            [SubtractChecked] = "-",
            [And] = "&",
            [Or] = "|",
            [ExclusiveOr] = "^",
            [AndAlso] = "&&",
            [OrElse] = "||",
            [Equal] = "==",
            [NotEqual] = "!=",
            [GreaterThanOrEqual] = ">=",
            [GreaterThan] = ">",
            [LessThan] = "<",
            [LessThanOrEqual] = "<=",
            [Coalesce] = "??",
            [LeftShift] = "<<",
            [RightShift] = ">>",
            [Assign] = "=",
            [AddAssign] = "+=",
            [AddAssignChecked] = "+=",
            [AndAssign] = "&=",
            [DivideAssign] = "/=",
            [ExclusiveOrAssign] = "^=",
            [LeftShiftAssign] = "<<=",
            [ModuloAssign] = "%=",
            [MultiplyAssign] = "*=",
            [MultiplyAssignChecked] = "*=",
            [OrAssign] = "|=",
            [RightShiftAssign] = ">>=",
            [SubtractAssign] = "-=",
            [SubtractAssignChecked] = "-="
        };

        protected override void WriteBinary(ExpressionType nodeType, string leftPath, Expression left, string rightPath, Expression right) {
            if (simpleBinaryOperators.TryGetValue(nodeType, out var @operator)) {
                WriteNode(leftPath, left);
                Write($" {@operator} ");
                WriteNode(rightPath, right);
                return;
            }

            switch (nodeType) {
                case ArrayIndex:
                    WriteNode(leftPath, left);
                    Write("[");
                    WriteNode(rightPath, right);
                    Write("]");
                    return;
                case Power:
                    Write("Math.Pow(");
                    WriteNode(leftPath, left);
                    Write(", ");
                    WriteNode(rightPath, right);
                    Write(")");
                    return;
                case PowerAssign:
                    WriteNode($"{leftPath}_0", left);
                    Write(" = ");
                    Write("Math.Pow(");
                    WriteNode(leftPath, left);
                    Write(", ");
                    WriteNode(rightPath, right);
                    Write(")");
                    return;
            }

            throw new NotImplementedException();
        }

        protected override void WriteUnary(ExpressionType nodeType, string operandPath, Expression operand, Type type, string expressionTypename) {
            switch (nodeType) {
                case ArrayLength:
                    WriteNode(operandPath, operand);
                    Write(".Length");
                    break;
                case ExpressionType.Convert:
                case ConvertChecked:
                case Unbox:
                    if (!type.IsAssignableFrom(operand.Type)) {
                        Write($"({type.FriendlyName(language)})");
                    }
                    WriteNode(operandPath, operand);
                    break;
                case Negate:
                case NegateChecked:
                    Write("-");
                    WriteNode(operandPath, operand);
                    break;
                case Not:
                    if (type == typeof(bool)) {
                        Write("!");
                    } else {
                        Write("~");
                    }
                    WriteNode(operandPath, operand);
                    break;
                case OnesComplement:
                    Write("~");
                    WriteNode(operandPath, operand);
                    break;
                case TypeAs:
                    WriteNode(operandPath, operand);
                    Write($" as {type.FriendlyName(language)}");
                    break;
                case PreIncrementAssign:
                    Write("++");
                    WriteNode(operandPath, operand);
                    break;
                case PostIncrementAssign:
                    WriteNode(operandPath, operand);
                    Write("++");
                    break;
                case PreDecrementAssign:
                    Write("--");
                    WriteNode(operandPath, operand);
                    break;
                case PostDecrementAssign:
                    WriteNode(operandPath, operand);
                    Write("--");
                    break;
                case IsTrue:
                    WriteNode(operandPath, operand);
                    break;
                case IsFalse:
                    Write("!");
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
                    Write("throw");
                    if (operand != null) {
                        Write(" ");
                        WriteNode(operandPath, operand);
                    }
                    break;
                case Quote:
                    TrimEnd(true);
                    WriteEOL();
                    Write("// --- Quoted - begin");
                    Indent();
                    WriteEOL();
                    WriteNode(operandPath, operand);
                    WriteEOL(true);
                    Write("// --- Quoted - end");
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
            Write("(");
            WriteNodes("Parameters", expr.Parameters, false, ", ", true);
            Write(") => ");

            if (CanInline(expr.Body)) {
                WriteNode("Body", expr.Body);
                return;
            }

            Write("{");
            Indent();
            WriteEOL();

            CSharpMultilineBlockTypes blockType = CSharpMultilineBlockTypes.Block;
            if (expr.Body.Type != typeof(void)) {
                if (expr.Body is BlockExpression bexpr && bexpr.HasMultipleLines()) {
                    blockType = CSharpMultilineBlockTypes.Return;
                } else {
                    Write("return ");
                }
            }
            WriteNode("Body", expr.Body, CreateMetadata(blockType));
            WriteStatementEnd(expr.Body);
            WriteEOL(true);
            Write("}");
        }

        protected override void WriteParameterDeclaration(ParameterExpression prm) {
            if (prm.IsByRef) { Write("ref "); }
            Write($"{prm.Type.FriendlyName(language)} {prm.Name}");
        }

        protected override void WriteNew(Type type, string argsPath, IList<Expression> args) {
            Write("new ");
            Write(type.FriendlyName(language));
            Write("(");
            WriteNodes(argsPath, args);
            Write(")");
        }

        protected override void WriteNew(NewExpression expr) {
            if (expr.Type.IsAnonymous()) {
                Write("new {");
                Indent();
                WriteEOL();
                expr.Constructor.GetParameters().Select(x => x.Name).Zip(expr.Arguments).ForEachT((name, arg, index) => {
                    if (index > 0) {
                        Write(",");
                        WriteEOL();
                    }
                    // write as `property = member` only if the source name is different from the target name
                    // otheriwse just write `member`
                    if (!(arg is MemberExpression mexpr && mexpr.Member.Name.Replace("$VB$Local_", "") == name)) {
                        Write($"{name} = ");
                    }
                    WriteNode($"Arguments[{index}]", arg);
                });
                WriteEOL(true);
                Write("}");
                return;
            }
            WriteNew(expr.Type, "Arguments", expr.Arguments);
        }

        protected override void WriteBinding(MemberBinding binding) {
            Write(binding.Member.Name);
            Write(" = ");
            if (binding is MemberAssignment assignmentBinding) {
                WriteNode("Expression", assignmentBinding.Expression);
                return;
            }

            Write("{");

            IEnumerable<object>? items = null;
            string itemsPath = "";
            switch (binding) {
                case MemberListBinding listBinding when listBinding.Initializers.Count > 0:
                    items = listBinding.Initializers.Cast<object>();
                    itemsPath = "Initializers";
                    break;
                case MemberMemberBinding memberBinding when memberBinding.Bindings.Count > 0:
                    items = memberBinding.Bindings.Cast<object>();
                    itemsPath = "Bindings";
                    break;
            }
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
                Write(" {");
                Indent();
                WriteEOL();
                WriteNodes("Bindings", expr.Bindings, true);
                WriteEOL(true);
                Write("}");
            }
        }

        protected override void WriteListInit(ListInitExpression expr) {
            WriteNode("NewExpression", expr.NewExpression);
            Write(" {");
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
                    Write("new");
                    if (elementType.IsArray || expr.Expressions.None() || expr.Expressions.Any(x => x.Type != elementType)) {
                        Write(" ");
                        Write(expr.Type.FriendlyName(language));
                    } else {
                        Write("[]");
                    }
                    Write(" { ");
                    WriteNodes("Expressions", expr.Expressions);
                    Write(" }");
                    break;
                case NewArrayBounds:
                    (string left, string right) = ("[", "]");
                    var nestedArrayTypes = expr.Type.NestedArrayTypes().ToList();
                    Write($"new {nestedArrayTypes.Last().root!.FriendlyName(language)}");
                    nestedArrayTypes.ForEachT((current, _, index) => {
                        Write(left);
                        if (index == 0) {
                            WriteNodes("Expressions", expr.Expressions);
                        } else {
                            Write(Repeat("", current.GetArrayRank()).Joined());
                        }
                        Write(right);
                    });
                    break;
                default:
                    throw new NotImplementedException();
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

        protected override void WriteConditional(ConditionalExpression expr, object? metadata) {
            if (expr.Type != typeof(void)) {
                WriteNode("Test", expr.Test);
                Write(" ? ");
                WriteNode("IfTrue", expr.IfTrue);
                Write(" : ");
                WriteNode("IfFalse", expr.IfFalse);
                return;
            }

            Write("if (");
            WriteNode("Test", expr.Test, CreateMetadata(Test));
            Write(") {");
            Indent();
            WriteEOL();
            WriteNode("IfTrue", expr.IfTrue, CreateMetadata(CSharpMultilineBlockTypes.Block));
            WriteStatementEnd(expr.IfTrue);
            WriteEOL(true);
            Write("}");
            if (!expr.IfFalse.IsEmpty()) {
                Write(" else ");
                if (!(expr.IfFalse is ConditionalExpression)) {
                    Write("{");
                    Indent();
                    WriteEOL();
                }
                WriteNode("IfFalse", expr.IfFalse, CreateMetadata(CSharpMultilineBlockTypes.Block));
                WriteStatementEnd(expr.IfFalse);
                if (!(expr.IfFalse is ConditionalExpression)) {
                    WriteEOL(true);
                    Write("}");
                }
            }
        }

        protected override void WriteDefault(DefaultExpression expr) =>
            Write($"default({expr.Type.FriendlyName(language)})");

        protected override void WriteTypeBinary(TypeBinaryExpression expr) {
            WriteNode("Expression", expr.Expression);
            var typeName = expr.TypeOperand.FriendlyName(language);
            switch (expr.NodeType) {
                case TypeIs:
                    Write($" is {typeName}");
                    break;
                case TypeEqual:
                    Write($".GetType() == typeof({typeName})");
                    break;
            }
        }

        protected override void WriteBlock(BlockExpression expr, object? metadata) {
            var (blockType, parentIsBlock) = metadata as CSharpBlockMetadata ?? CreateMetadata(Inline, false);
            if (blockType == ForceInline) {
                WriteNodes("Expressions", expr.Expressions);
                return;
            }

            bool introduceNewBlock;
            if (blockType.In(CSharpMultilineBlockTypes.Block, CSharpMultilineBlockTypes.Return)) {
                introduceNewBlock = expr.Variables.Any() && parentIsBlock;
                if (introduceNewBlock) {
                    Write("{");
                    Indent();
                    WriteEOL();
                }
                expr.Variables.ForEach((subexpr, index) => {
                    WriteNode($"Variables[{index}]", subexpr, true);
                    Write(";");
                    WriteEOL();
                });
                expr.Expressions.ForEach((subexpr, index) => {
                    if (index > 0) { WriteEOL(); }
                    if (subexpr is LabelExpression) { TrimEnd(); }
                    if (blockType == CSharpMultilineBlockTypes.Return && index == expr.Expressions.Count - 1) {
                        if (subexpr is BlockExpression bexpr && bexpr.HasMultipleLines()) {
                            WriteNode($"Expressions[{index}]", subexpr, CreateMetadata(CSharpMultilineBlockTypes.Return, true));
                        } else {
                            Write("return ");
                            WriteNode($"Expressions[{index}]", subexpr, CreateMetadata(CSharpMultilineBlockTypes.Block, true));
                        }
                    } else {
                        WriteNode($"Expressions[{index}]", subexpr, CreateMetadata(CSharpMultilineBlockTypes.Block, true));
                    }
                    WriteStatementEnd(subexpr);
                });
                if (introduceNewBlock) {
                    WriteEOL(true);
                    Write("}");
                }
                return;
            }

            introduceNewBlock =
                (expr.Expressions.Count > 1 && !parentIsBlock) ||
                expr.Variables.Any();
            if (introduceNewBlock) {
                if (blockType == Inline || parentIsBlock) { Write("("); }
                Indent();
                WriteEOL();
            }
            WriteNodes("Variables", expr.Variables, true, ",", true);
            expr.Expressions.ForEach((subexpr, index) => {
                if (index > 0 || expr.Variables.Count > 0) {
                    var previousExpr = index > 0 ? expr.Expressions[index - 1] : null;
                    if (previousExpr is null || !(previousExpr is LabelExpression || subexpr is RuntimeVariablesExpression)) { Write(","); }
                    WriteEOL();
                }
                if (subexpr is LabelExpression) { TrimEnd(); }
                WriteNode($"Expressions[{index}]", subexpr, CreateMetadata(Test, true));
            });
            if (introduceNewBlock) {
                WriteEOL(true);
                if (blockType == Inline || parentIsBlock) { Write(")"); }
            }
            return;
        }

        private void WriteStatementEnd(Expression expr) {
            switch (expr) {
                case ConditionalExpression cexpr when cexpr.Type == typeof(void):
                case BlockExpression _:
                case SwitchExpression _:
                case LabelExpression _:
                case TryExpression _:
                case RuntimeVariablesExpression _:
                case UnaryExpression bexpr when bexpr.NodeType == Quote:
                    return;
            }
            Write(";");
        }

        protected override void WriteSwitchCase(SwitchCase switchCase) {
            if (switchCase.Body.Type != typeof(void)) {
                WriteNodes("TestValues", switchCase.TestValues, " or ");
                Write(" => ");
                WriteNode("Body", switchCase.Body, CreateMetadata(ForceInline));
                return;
            }

            switchCase.TestValues.ForEach((testValue, index) => {
                if (index > 0) { WriteEOL(); }
                Write("case ");
                WriteNode($"TestValues[{index}]", testValue);
                Write(":");
            });
            Indent();
            WriteEOL();
            WriteNode("Body", switchCase.Body, CreateMetadata(CSharpMultilineBlockTypes.Block));
            WriteStatementEnd(switchCase.Body);
            WriteEOL();
            Write("break;");
        }

        protected override void WriteSwitch(SwitchExpression expr) {
            if (expr.Type != typeof(void)) {
                WriteNode("SwitchValue", expr.SwitchValue);
                Write(" switch {");
                Indent();
                WriteEOL();
                WriteNodes("Cases", expr.Cases, true, ",");
                if (expr.DefaultBody != null) {
                    Write(",");
                    WriteEOL();
                    Write("_ => ");
                    WriteNode("DefaultBody", expr.DefaultBody, CreateMetadata(ForceInline));
                }
                WriteEOL(true);
                Write("}");
                return;
            }

            Write("switch (");
            WriteNode("SwitchValue", expr.SwitchValue, CreateMetadata(Test));
            Write(") {");
            Indent();
            WriteEOL();
            expr.Cases.ForEach((switchCase, index) => {
                if (index > 0) { WriteEOL(); }
                WriteNode($"Cases[{index}]", switchCase);
                Dedent();
            });
            if (expr.DefaultBody != null) {
                WriteEOL();
                Write("default:");
                Indent();
                WriteEOL();
                WriteNode("DefaultBody", expr.DefaultBody, CreateMetadata(CSharpMultilineBlockTypes.Block));
                WriteStatementEnd(expr.DefaultBody);
                WriteEOL();
                Write("break;");
                Dedent();
            }
            WriteEOL(true);
            Write("}");
        }

        protected override void WriteCatchBlock(CatchBlock catchBlock) {
            Write("catch ");
            if (catchBlock.Variable != null || catchBlock.Test != typeof(Exception)) {
                Write("(");
                if (catchBlock.Variable != null) {
                    WriteNode("Variable", catchBlock.Variable, true);
                } else {
                    Write(catchBlock.Test.FriendlyName(language));
                }
                Write(") ");
                if (catchBlock.Filter != null) {
                    Write("when (");
                    WriteNode("Filter", catchBlock.Filter, CreateMetadata(Test));
                    Write(") ");
                }
            }
            Write("{");
            Indent();
            WriteEOL();
            WriteNode("Body", catchBlock.Body, CreateMetadata(CSharpMultilineBlockTypes.Block));
            WriteStatementEnd(catchBlock.Body);
            WriteEOL(true);
            Write("}");
        }

        protected override void WriteTry(TryExpression expr) {
            Write("try {");
            Indent();
            WriteEOL();
            WriteNode("Body", expr.Body);
            WriteStatementEnd(expr.Body);
            WriteEOL(true);
            Write("}");
            expr.Handlers.ForEach((catchBlock, index) => {
                Write(" ");
                WriteNode($"Handlers[{index}]", catchBlock);
            });
            if (expr.Fault != null) {
                Write(" fault {");
                Indent();
                WriteEOL();
                WriteNode("Fault", expr.Fault);
                WriteStatementEnd(expr.Fault);
                WriteEOL(true);
                Write("}");
            }
            if (expr.Finally != null) {
                Write(" finally {");
                Indent();
                WriteEOL();
                WriteNode("Finally", expr.Finally);
                WriteStatementEnd(expr.Finally);
                WriteEOL(true);
                Write("}");
            }
        }

        protected override void WriteLabel(LabelExpression expr) {
            WriteNode("Target", expr.Target);
            Write(":");
        }

        protected override void WriteGoto(GotoExpression expr) {
            var gotoKeyword = expr.Kind switch
            {
                Break => "break",
                Continue => "continue",
                GotoExpressionKind.Goto => "goto",
                GotoExpressionKind.Return => "return",
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
            Write("while (true) {");
            Indent();
            WriteEOL();
            WriteNode("Body", expr.Body, CreateMetadata(CSharpMultilineBlockTypes.Block));
            WriteStatementEnd(expr.Body);
            WriteEOL(true);
            Write("}");
        }

        protected override void WriteRuntimeVariables(RuntimeVariablesExpression expr) {
            Write("// variables -- ");
            expr.Variables.ForEach((x, index) => {
                if (index > 0) { Write(", "); }
                WriteNode($"Variables[{index}]", x, true);
            });
        }

        protected override void WriteDebugInfo(DebugInfoExpression expr) {
            var filename = expr.Document.FileName;
            Write("// ");
            var comment =
                expr.IsClear ?
                $"Clear debug info from {filename}" :
                $"Debug to {filename} -- L{expr.StartLine}C{expr.StartColumn} : L{expr.EndLine}C{expr.EndColumn}";
            Write(comment);
        }

        protected override (string prefix, string suffix) GenericIndicators => ("<", ">");

        protected override bool ParensOnEmptyArguments => true;

        protected override (string prefix, string suffix) IndexerIndicators => ("[", "]");
    }
}