using ExpressionTreeToString.Util;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZSpitz.Util;
using static System.Linq.Expressions.ExpressionType;

namespace ExpressionTreeToString {
    public class DebugViewWriterVisitor : BuiltinsWriterVisitor {
        public static bool FrameworkCompatible = false;

        public DebugViewWriterVisitor(object o, bool hasPathSpans) :
            base(o, (Language?)null, new[] { "", "lambdas" }, hasPathSpans) { }

        [Flags]
        private enum Flow {
            None,
            Space,
            NewLine,

            Break = 0x8000      // newline if column > MaxColumn
        }


        private const int MaxColumn = 120;

        private int _column;

        private const int Tab = 4;
        private int Depth => CurrentIndentationLevel * Tab;

        private void Out(string s, Flow after = Flow.None) => Out(Flow.None, s, after);
        private void Out(Flow before, string s, Flow after = Flow.None) {
            Flow CheckBreak(Flow flow) {
                if ((flow & Flow.Break) != 0) {
                    if (_column > (MaxColumn + Depth)) {
                        flow = Flow.NewLine;
                    } else {
                        flow &= ~Flow.Break;
                    }
                }
                return flow;
            }

            Flow GetFlow(Flow flow) {
                Flow last = CheckBreak(_flow);
                flow = CheckBreak(flow);
                return (Flow)Math.Max((int)last, (int)flow);
            }

            switch (GetFlow(before)) {
                case Flow.None:
                    break;
                case Flow.Space:
                    Write(" ");
                    break;
                case Flow.NewLine:
                    _column = 0;
                    WriteEOL();
                    break;
            }
            _column += s.Length;
            Write(s);
            _flow = after;
        }

        private void NewLine() => _flow = Flow.NewLine;

        private readonly Stack<int> _stack = new Stack<int>();
        private Flow _flow;

        private Dictionary<LambdaExpression, int>? _lambdaIds;
        private Dictionary<ParameterExpression, int>? _paramIds;
        private Dictionary<LabelTarget, int>? _labelIds;

        private static int GetId<T>(T e, ref Dictionary<T, int>? ids, out bool isNew) where T : notnull {
            isNew = false;
            if (ids == null) {
                ids = new Dictionary<T, int>();
            }

            if (!ids.TryGetValue(e, out int id)) {
                isNew = true;
                id = ids.Count + 1;
                ids.Add(e, id);
            }

            return id;
        }

        private int GetLambdaId(LambdaExpression lmbd, out bool isNew) => GetId(lmbd, ref _lambdaIds, out isNew);
        private int GetParamId(ParameterExpression prm) => GetId(prm, ref _paramIds, out var _);
        private int GetLabelTargetId(LabelTarget target) => GetId(target, ref _labelIds, out var _);

        private static string GetDisplayName(string name) {
            static bool ContainsWhitespace(string name) {
                foreach (var c in name) {
                    if (char.IsWhiteSpace(c)) { return true; }
                }
                return false;
            }

            return ContainsWhitespace(name) ?
                $"'{name}'" :
                name;
        }

        bool isRootLambda = true;
        protected override void WriteNodeImpl(object o, bool parameterDeclaration = false, object? metadata = null) {
            if (!(o is LambdaExpression)) { isRootLambda = false; }
            base.WriteNodeImpl(o, parameterDeclaration, metadata);
        }

        private void VisitExpressions<T>(string path, char open, IReadOnlyList<T> expressions, Action<string, T>? visit = null) => VisitExpressions(path, open, ',', expressions);
        private void VisitExpressions<T>(string path, char open, char separator, IReadOnlyList<T> expressions, Action<string, T>? visit = null) {
            visit ??= (path, e) => WriteNode(path, e);

            Out(open.ToString());

            if (expressions is { }) {
                Indent();
                expressions.ForEach((e, index) => {
                    if (index == 0) {
                        if (open == '{' || expressions.Count > 1) { NewLine(); }
                    } else {
                        Out(separator.ToString(), Flow.NewLine);
                    }
                    visit($"{path}[{index}]", e);
                });
                Dedent();
            }

            char close = open switch
            {
                '(' => ')',
                '{' => '}',
                '[' => ']',
                _ => throw new InvalidOperationException()
            };

            if (open == '{') {
                NewLine();
            }
            Out(close.ToString(), Flow.Break);
        }

        private void VisitDeclarations(string path, IReadOnlyList<ParameterExpression> expressions) =>
            VisitExpressions(path, '(', ',', expressions, (path, variable) => {
                Out(variable.Type.ToString());
                if (variable.IsByRef) { Out("&"); }
                Out(" ");
                WriteParameter(variable);
            });

        private string GetLabelTargetName(LabelTarget target) => 
            target.Name.IsNullOrEmpty() ?
                $"#Label{GetLabelTargetId(target)}" :
                GetDisplayName(target.Name);

        private void DumpLabel(LabelTarget target) =>
            Out(
                $".LabelTarget {GetLabelTargetName(target)}:"
            );

        protected override void WriteBinary(BinaryExpression node) {
            if (node.NodeType == ArrayIndex) {
                ParenthesizedVisit(node, node.Left, "Left");
                Out("[");
                WriteNode("Right", node.Right);
                Out("]");
                return;
            }

            bool parenthesizeLeft = NeedsParentheses(node, node.Left);
            bool parenthesizeRight = NeedsParentheses(node, node.Right);

            string op;
            Flow beforeOp = Flow.Space;
            switch (node.NodeType) {
                case Assign: op = "="; break;
                case Equal: op = "=="; break;
                case NotEqual: op = "!="; break;
                case AndAlso: op = "&&"; beforeOp = Flow.Break | Flow.Space; break;
                case OrElse: op = "||"; beforeOp = Flow.Break | Flow.Space; break;
                case GreaterThan: op = ">"; break;
                case LessThan: op = "<"; break;
                case GreaterThanOrEqual: op = ">="; break;
                case LessThanOrEqual: op = "<="; break;
                case Add: op = "+"; break;
                case AddAssign: op = "+="; break;
                case AddAssignChecked: op = "#+="; break;
                case AddChecked: op = "#+"; break;
                case Subtract: op = "-"; break;
                case SubtractAssign: op = "-="; break;
                case SubtractAssignChecked: op = "#-="; break;
                case SubtractChecked: op = "#-"; break;
                case Divide: op = "/"; break;
                case DivideAssign: op = "/="; break;
                case Modulo: op = "%"; break;
                case ModuloAssign: op = "%="; break;
                case Multiply: op = "*"; break;
                case MultiplyAssign: op = "*="; break;
                case MultiplyAssignChecked: op = "#*="; break;
                case MultiplyChecked: op = "#*"; break;
                case LeftShift: op = "<<"; break;
                case LeftShiftAssign: op = "<<="; break;
                case RightShift: op = ">>"; break;
                case RightShiftAssign: op = ">>="; break;
                case And: op = "&"; break;
                case AndAssign: op = "&="; break;
                case Or: op = "|"; break;
                case OrAssign: op = "|="; break;
                case ExclusiveOr: op = "^"; break;
                case ExclusiveOrAssign: op = "^="; break;
                case Power: op = "**"; break;
                case PowerAssign: op = "**="; break;
                case Coalesce: op = "??"; break;

                default:
                    throw new InvalidOperationException();
            }

            if (parenthesizeLeft) {
                Out("(", Flow.None);
            }

            WriteNode("Left", node.Left);
            if (parenthesizeLeft) {
                Out(Flow.None, ")", Flow.Break);
            }

            Out(beforeOp, op, Flow.Space | Flow.Break);

            if (parenthesizeRight) {
                Out("(", Flow.None);
            }
            WriteNode("Right", node.Right);
            if (parenthesizeRight) {
                Out(Flow.None, ")", Flow.Break);
            }
        }

        protected override void WriteUnary(UnaryExpression node) {
            if (node.NodeType == Throw && node.Operand is { }) {
                Out(".Throw", Flow.Space);
            } else {
                var toWrite = node.NodeType switch
                {
                    ExpressionType.Convert => "(" + node.Type.ToString() + ")",
                    ConvertChecked => "#(" + node.Type.ToString() + ")",
                    Not => node.Type == typeof(bool) ? "!" : "~",
                    OnesComplement => "~",
                    Negate => "-",
                    NegateChecked => "#-",
                    UnaryPlus => "+",
                    Quote => "'",
                    Throw => ".Rethrow",
                    IsFalse => ".IsFalse",
                    IsTrue => ".IsTrue",
                    Decrement => ".Decrement",
                    Increment => ".Increment",
                    PreDecrementAssign => "--",
                    PreIncrementAssign => "++",
                    Unbox => ".Unbox",
                    _ => null // TypeAs, ArrayLength
                };
                if (toWrite is { }) { Out(toWrite); }
            }

            if (node.Operand is { }) {
                ParenthesizedVisit(node, node.Operand, "Operand");
            }

            switch (node.NodeType) {
                case TypeAs:
                    Out(Flow.Space, ".As", Flow.Space | Flow.Break);
                    Out(node.Type.ToString());
                    break;

                case ArrayLength:
                    Out(".Length");
                    break;

                case PostDecrementAssign:
                    Out("--");
                    break;

                case PostIncrementAssign:
                    Out("++");
                    break;
            }
        }

        private void WriteLambdaName(LambdaExpression lmbd, out bool isNew) {
            isNew = false;
            var name = lmbd.Name.IsNullOrEmpty() ?
                "#Lambda" + GetLambdaId(lmbd, out isNew) :
                lmbd.Name;
            Out(
                $".Lambda {name}<{lmbd.Type}>"
            );
        }

        private void WriteLambdaFull(LambdaExpression expr) {
            WriteLambdaName(expr, out var _);
            VisitDeclarations("Parameters", expr.Parameters);
            Indent();
            Out(Flow.Space, "{", Flow.NewLine);
            WriteNode("Body", expr.Body);
            Dedent();
            Out(Flow.NewLine, "}");
        }

        private bool isLambdasWritten = false;
        protected override void WriteLambda(LambdaExpression expr) {
            if (isRootLambda) {
                isRootLambda = false;
                WriteLambdaFull(expr);
                return;
            }

            WriteLambdaName(expr, out var isNew);
            if (isNew) {
                SetInsertionPoint("lambdas");
                if (!isLambdasWritten) {
                    WriteEOL();
                    isLambdasWritten = true;
                }
                WriteLambdaFull(expr);
                SetInsertionPoint("");
            }
        }

        protected override void WriteParameter(ParameterExpression expr) {
            var name =
                expr.Name.IsNullOrEmpty() ?
                    $"$var{GetParamId(expr)}" :
                    $"${GetDisplayName(expr.Name)}";
            Out(name);
        }

        private static Dictionary<Type, string> suffixes = new Dictionary<Type, string> {
            [typeof(uint)] = "U",
            [typeof(long)] = "L",
            [typeof(ulong)] = "UL",
            [typeof(double)] = "D",
            [typeof(float)] = "F",
            [typeof(decimal)] = "M"
        };

        protected override void WriteConstant(ConstantExpression expr) {
            object? value = expr.Value;

            Out(
                value switch
                {
                    null => "null",
                    string _ when expr.Type == typeof(string) => $"\"{value}\"",
                    char _ when expr.Type == typeof(char) => $"'{value}'",
                    int _ when expr.Type == typeof(int) => value.ToString(),
                    bool _ when expr.Type == typeof(bool) => value.ToString(),
                    var _ when suffixes.TryGetValue(expr.Type, out var suffix) => $"{value}{suffix}",
                    _ => $".Constant<{expr.Type}>({value})"
                }
            );
        }

        private static int GetOperatorPrecedence(Expression node) {
            switch (node.NodeType) {
                // Assignment
                case Assign:
                case ExclusiveOrAssign:
                case AddAssign:
                case AddAssignChecked:
                case SubtractAssign:
                case SubtractAssignChecked:
                case DivideAssign:
                case ModuloAssign:
                case MultiplyAssign:
                case MultiplyAssignChecked:
                case LeftShiftAssign:
                case RightShiftAssign:
                case AndAssign:
                case OrAssign:
                case PowerAssign:
                case Coalesce:
                    return 1;

                // Conditional (?:) would go here

                // Conditional OR
                case OrElse:
                    return 2;

                // Conditional AND
                case AndAlso:
                    return 3;

                // Logical OR
                case Or:
                    return 4;

                // Logical XOR
                case ExclusiveOr:
                    return 5;

                // Logical AND
                case And:
                    return 6;

                // Equality
                case Equal:
                case NotEqual:
                    return 7;

                // Relational, type testing
                case GreaterThan:
                case LessThan:
                case GreaterThanOrEqual:
                case LessThanOrEqual:
                case TypeAs:
                case TypeIs:
                case TypeEqual:
                    return 8;

                // Shift
                case LeftShift:
                case RightShift:
                    return 9;

                // Additive
                case Add:
                case AddChecked:
                case Subtract:
                case SubtractChecked:
                    return 10;

                // Multiplicative
                case Divide:
                case Modulo:
                case Multiply:
                case MultiplyChecked:
                    return 11;

                // Unary
                case Negate:
                case NegateChecked:
                case UnaryPlus:
                case Not:
                case ExpressionType.Convert:
                case ConvertChecked:
                case PreIncrementAssign:
                case PreDecrementAssign:
                case OnesComplement:
                case Increment:
                case Decrement:
                case IsTrue:
                case IsFalse:
                case Unbox:
                case Throw:
                    return 12;

                // Power, which is not in C#
                // But VB/Python/Ruby put it here, above unary.
                case Power:
                    return 13;

                // Primary, which includes all other node types:
                //   member access, calls, indexing, new.
                case PostIncrementAssign:
                case PostDecrementAssign:
                default:
                    return 14;

                // These aren't expressions, so never need parentheses:
                //   constants, variables
                case Constant:
                case Parameter:
                    return 15;
            }
        }

        private static bool NeedsParentheses(Expression parent, Expression? child) {
            if (child == null) {
                return false;
            }

            // Some nodes always have parentheses because of how they are
            // displayed, for example: ".Unbox(obj.Foo)"
            switch (parent.NodeType) {
                case Increment:
                case Decrement:
                case IsTrue:
                case IsFalse:
                case Unbox:
                    return true;
            }

            int childOpPrec = GetOperatorPrecedence(child);
            int parentOpPrec = GetOperatorPrecedence(parent);

            if (childOpPrec == parentOpPrec) {
                // When parent op and child op has the same precedence,
                // we want to be a little conservative to have more clarity.
                // Parentheses are not needed if
                // 1) Both ops are &&, ||, &, |, or ^, all of them are the only
                // op that has the precedence.
                // 2) Parent op is + or *, e.g. x + (y - z) can be simplified to
                // x + y - z.
                // 3) Parent op is -, / or %, and the child is the left operand.
                // In this case, if left and right operand are the same, we don't
                // remove parenthesis, e.g. (x + y) - (x + y)
                //
                switch (parent.NodeType) {
                    case AndAlso:
                    case OrElse:
                    case And:
                    case Or:
                    case ExclusiveOr:
                        // Since these ops are the only ones on their precedence,
                        // the child op must be the same.
                        // We remove the parenthesis, e.g. x && y && z
                        return false;
                    case Add:
                    case AddChecked:
                    case Multiply:
                    case MultiplyChecked:
                        return false;
                    case Subtract:
                    case SubtractChecked:
                    case Divide:
                    case Modulo:
                        return child == (parent as BinaryExpression)!.Right;
                }
                return true;
            }

            // Special case: negate of a constant needs parentheses, to
            // disambiguate it from a negative constant.
            if (child != null && child.NodeType == Constant &&
                (parent.NodeType == Negate || parent.NodeType == NegateChecked)) {
                return true;
            }

            // If the parent op has higher precedence, need parentheses for the child.
            return childOpPrec < parentOpPrec;
        }

        private void ParenthesizedVisit(Expression parent, Expression nodeToVisit, string path) {
            if (NeedsParentheses(parent, nodeToVisit)) {
                Out("(");
                WriteNode(path, nodeToVisit!); // nodeToVisit is not null if NeedsParentheses returns true; TODO use attribute
                Out(")");
            } else {
                WriteNode(path, nodeToVisit); // TODO what happens if this is null?
            }
        }

        private void OutMember(Expression node, Expression? instance, MemberInfo member, string path) {
            if (instance is { }) {
                ParenthesizedVisit(node, instance, path);
                Out("." + member.Name);
            } else {
                // For static members, include the type name
                Out(member.DeclaringType!.ToString() + "." + member.Name);
            }
        }

        protected override void WriteMemberAccess(MemberExpression expr) =>
            OutMember(expr, expr.Expression, expr.Member, "Expression");

        protected override void WriteNew(NewExpression node) {
            Out(".New " + node.Type.ToString());
            VisitExpressions("Arguments", '(', node.Arguments);
        }

        protected override void WriteCall(MethodCallExpression node) {
            Out(".Call ");
            if (node.Object is { }) {
                ParenthesizedVisit(node, node.Object, "Object");
            } else if (node.Method.DeclaringType is { }) {
                Out(node.Method.DeclaringType.ToString());
            } else {
                Out("<UnknownType>");
            }
            Out(".");
            Out(node.Method.Name);
            VisitExpressions("Arguments", '(', node.Arguments);
        }

        protected override void WriteMemberInit(MemberInitExpression node) {
            WriteNode("NewExpression", node.NewExpression);
            VisitExpressions("Bindings", '{', ',', node.Bindings, (path, e) => WriteNode(path, e));
        }

        protected override void WriteListInit(ListInitExpression node) {
            WriteNode("NewExpression", node.NewExpression);
            VisitExpressions("Initializers", '{', ',', node.Initializers, (path, e) => WriteNode(path, e));
        }

        protected override void WriteNewArray(NewArrayExpression node) {
            if (node.NodeType == ExpressionType.NewArrayBounds) {
                // .NewArray MyType[expr1, expr2]
                Out(".NewArray " + node.Type.GetElementType()!.ToString());
                VisitExpressions("Expressions", '[', node.Expressions);
            } else {
                // .NewArray MyType {expr1, expr2}
                Out(".NewArray " + node.Type.ToString(), Flow.Space);
                VisitExpressions("Expressions", '{', node.Expressions);
            }
        }

        protected override void WriteConditional(ConditionalExpression expr, object? metadata) {
            if (IsSimpleExpression(expr.Test)) {
                Out(".If (");
                WriteNode("Test", expr.Test);
                Out(") {", Flow.NewLine);
            } else {
                Out(".If (", Flow.NewLine);
                Indent();
                WriteNode("Test", expr.Test);
                Dedent();
                Out(Flow.NewLine, ") {", Flow.NewLine);
            }
            Indent();
            WriteNode("IfTrue", expr.IfTrue);
            Dedent();
            Out(Flow.NewLine, "} .Else {", Flow.NewLine);
            Indent();
            WriteNode("IfFalse", expr.IfFalse);
            Dedent();
            Out(Flow.NewLine, "}");

            static bool IsSimpleExpression(Expression node) {
                if (node is BinaryExpression binary) {
                    return !(binary.Left is BinaryExpression || binary.Right is BinaryExpression);
                }
                return false;
            }
        }

        protected override void WriteDefault(DefaultExpression node) => 
            Out(
                $".Default({node.Type})"
            );

        protected override void WriteTypeBinary(TypeBinaryExpression node) {
            ParenthesizedVisit(node, node.Expression, "Expression");
            switch (node.NodeType) {
                case TypeIs:
                    Out(Flow.Space, ".Is", Flow.Space);
                    break;
                case TypeEqual:
                    Out(Flow.Space, ".TypeEqual", Flow.Space);
                    break;
            }
            Out(node.TypeOperand.ToString());
        }

        protected override void WriteInvocation(InvocationExpression expr) {
            Out(".Invoke ");
            ParenthesizedVisit(expr, expr.Expression, "Expression");
            VisitExpressions("Arguments", '(', expr.Arguments);
        }

        protected override void WriteIndex(IndexExpression node) {
            if (node.Indexer is { }) {
                OutMember(node, node.Object, node.Indexer, "Object");
            } else {
                ParenthesizedVisit(node, node.Object, "Object");
            }
            VisitExpressions("Arguments", '[', node.Arguments);
        }

        protected override void WriteBlock(BlockExpression node, object? metadata) {
            Out(".Block");
            if (node.Type != node.Expressions.Last()?.Type) {
                Out(
                    $"<{node.Type}>"
                );
            }

            VisitDeclarations("Variables", node.Variables);
            Out(" ");
            VisitExpressions("Expressions", '{', ';', node.Expressions);
        }

        protected override void WriteSwitch(SwitchExpression node) {
            Out(".Switch ");
            Out("(");
            WriteNode("SwitchValue", node.SwitchValue);
            Out(") {", Flow.NewLine);
            WriteNodes("Cases", node.Cases, "");
            if (node.DefaultBody != null) {
                Out(".Default:", Flow.NewLine);
                Indent(); Indent();
                WriteNode("DefaultBody", node.DefaultBody);
                Dedent(); Dedent();
                NewLine();
            }
            Out("}");
        }

        protected override void WriteTry(TryExpression node) {
            Out(".Try {", Flow.NewLine);
            Indent();
            WriteNode("Body", node.Body);
            Dedent();
            WriteNodes("Handlers", node.Handlers);
            if (node.Finally != null) {
                Out(Flow.NewLine, "} .Finally {", Flow.NewLine);
                Indent();
                WriteNode("Finally", node.Finally);
                Dedent();
            } else if (node.Fault != null) {
                Out(Flow.NewLine, "} .Fault {", Flow.NewLine);
                Indent();
                WriteNode("Fault", node.Fault);
                Dedent();
            }

            Out(Flow.NewLine, "}");
        }

        protected override void WriteLabel(LabelExpression node) {
            Out(".Label", Flow.NewLine);
            Indent();
            if (node.DefaultValue is { }) {
                WriteNode("DefaultValue", node.DefaultValue);
            }
            Dedent();
            NewLine();
            DumpLabel(node.Target);
        }

        protected override void WriteGoto(GotoExpression node) {
            Out("." + node.Kind.ToString(), Flow.Space);
            Out(GetLabelTargetName(node.Target), Flow.Space);
            Out("{", Flow.Space);
            if (node.Value is { }) { WriteNode("Value", node.Value); }
            Out(Flow.Space, "}");
        }

        protected override void WriteLoop(LoopExpression node) {
            Out(".Loop", Flow.Space);
            if (node.ContinueLabel != null) {
                DumpLabel(node.ContinueLabel);
            }
            Out(" {", Flow.NewLine);
            Indent();
            WriteNode("Body", node.Body);
            Dedent();
            Out(Flow.NewLine, "}");
            if (node.BreakLabel != null) {
                Out("", Flow.NewLine);
                DumpLabel(node.BreakLabel);
            }
        }

        protected override void WriteRuntimeVariables(RuntimeVariablesExpression expr) {
            Out(".RuntimeVariables");
            VisitExpressions("Variables", '(', expr.Variables);
        }

        protected override void WriteDebugInfo(DebugInfoExpression node) => 
            Out(
                $".DebugInfo({node.Document.FileName}: {node.StartLine}, {node.StartColumn} - {node.EndLine}, {node.EndColumn})"
            );

        protected override void WriteElementInit(ElementInit node) {
            if (node.Arguments.Count == 1) {
                WriteNode("Arguments[0]", node.Arguments[0]);
            } else {
                VisitExpressions("Arguments", '{', node.Arguments);
            }
        }

        protected override void WriteBinding(MemberBinding binding) {
            switch (binding) {
                case MemberAssignment assignment:
                    Out(assignment.Member.Name);
                    Out(Flow.Space, "=", Flow.Space);
                    WriteNode("Expression", assignment.Expression);
                    break;
                case MemberListBinding listBinding:
                    Out(binding.Member.Name);
                    Out(Flow.Space, "=", Flow.Space);
                    VisitExpressions("Initializers", '{', ',', listBinding.Initializers, (path, e) => WriteNode(path, e));
                    break;
                case MemberMemberBinding memberBinding:
                    Out(binding.Member.Name);
                    Out(Flow.Space, "=", Flow.Space);
                    VisitExpressions("Bindings", '{', ',', memberBinding.Bindings, (path, e) => WriteNode(path, e));
                    break;
            }
        }

        protected override void WriteExtension(Expression node) {
            Out(
                $".Extension<{node.GetType()}>"
            );

            if (node.CanReduce) {
                Out(Flow.Space, "{", Flow.NewLine);
                Indent();
                WriteNode("(Reduced)", node.Reduce());
                Dedent();
                Out(Flow.NewLine, "}");
            }
        }

        protected override void WriteSwitchCase(SwitchCase node) {
            node.TestValues.ForEach((test, index) => {
                Out(".Case (");
                WriteNode($"TestValues[{index}]", test);
                Out("):", Flow.NewLine);
            });
            Indent(); Indent();
            WriteNode("Body", node.Body);
            Dedent(); 
            Dedent();
            NewLine();
        }

        protected override void WriteCatchBlock(CatchBlock node) {
            Out(Flow.NewLine, $"}} .Catch ({node.Test}");
            if (node.Variable != null) {
                Out(Flow.Space, "");
                WriteNode("Variable", node.Variable);
            }
            if (node.Filter != null) {
                Out(") .If (", Flow.Break);
                WriteNode("Filter", node.Filter);
            }
            Out(") {", Flow.NewLine);
            Indent();
            WriteNode("Body", node.Body);
            Dedent();
        }

        protected override void WriteLabelTarget(LabelTarget labelTarget) {
            throw new NotImplementedException();
        }



        protected override void WriteDynamic(DynamicExpression node) {
            if (!FrameworkCompatible) {
                WriteExtension(node);
                return;
            }

            Out(".Dynamic", Flow.Space);
            Out(
                node.Binder switch
                {
                    ConvertBinder convert => $"Convert {convert.Type}",
                    GetMemberBinder getMember => $"GetMember {getMember.Name}",
                    SetMemberBinder setMember => $"SetMember {setMember.Name}",
                    DeleteMemberBinder deleteMember => $"DeleteMember {deleteMember.Name}",
                    GetIndexBinder _ => "GetIndex",
                    SetIndexBinder _ => "SetIndex",
                    DeleteIndexBinder _ => "DeleteIndex",
                    InvokeMemberBinder call => $"Call {call.Name}",
                    InvokeBinder _ => "Invoke",
                    CreateInstanceBinder _ => "Create",
                    UnaryOperationBinder unary => $"UnaryOperation {unary.Operation}",
                    BinaryOperationBinder binary => $"BinaryOperation {binary.Operation}",
                    _ => node.Binder.ToString()
                }
            );
            VisitExpressions("Arguments", '(', node.Arguments);
        }

        // WriteDynamic forwards to WriteExtension, so all of these are not needed
        protected override void WriteBinaryOperationBinder(BinaryOperationBinder binaryOperationBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteConvertBinder(ConvertBinder convertBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteCreateInstanceBinder(CreateInstanceBinder createInstanceBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteDeleteIndexBinder(DeleteIndexBinder deleteIndexBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteDeleteMemberBinder(DeleteMemberBinder deleteMemberBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteGetIndexBinder(GetIndexBinder getIndexBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteGetMemberBinder(GetMemberBinder getMemberBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteInvokeBinder(InvokeBinder invokeBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteInvokeMemberBinder(InvokeMemberBinder invokeMemberBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteSetIndexBinder(SetIndexBinder setIndexBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteSetMemberBinder(SetMemberBinder setMemberBinder, IList<Expression> args) => throw new NotImplementedException();

        protected override void WriteUnaryOperationBinder(UnaryOperationBinder unaryOperationBinder, IList<Expression> args) => throw new NotImplementedException();

        // Currently not needed; see VisitDeclarations
        protected override void WriteParameterDeclaration(ParameterExpression prm) => throw new NotImplementedException();
    }
}
