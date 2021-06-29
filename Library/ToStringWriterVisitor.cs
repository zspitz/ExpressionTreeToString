using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using ZSpitz.Util;
using static System.Linq.Expressions.ExpressionType;
using static System.Linq.Enumerable;
using System.Runtime.CompilerServices;
using ExpressionTreeToString.Util;
using static System.Linq.Expressions.GotoExpressionKind;
using OneOf;
using static ExpressionTreeToString.Util.Functions;

#pragma warning disable IDE1006 // we want to imitate the original source as much as possible
namespace ExpressionTreeToString {
    public class ToStringWriterVisitor : BuiltinsWriterVisitor {
        public static bool FrameworkCompatible = false;

        public ToStringWriterVisitor(object o, bool hasPathSpans)
            : base(o, (Language?)null, null, hasPathSpans) { }

        // Associate every unique label or anonymous parameter in the tree with an integer.
        // Labels are displayed as UnnamedLabel_#; parameters are displayed as Param_#.
        private Dictionary<OneOf<LabelTarget, ParameterExpression>, int>? _ids;

        private int GetLabelId(LabelTarget label) => GetId(label, ref _ids, out var _);
        private int GetParamId(ParameterExpression p) => GetId(p, ref _ids, out var _);

        private static bool IsBool(Expression node) => node.Type.In(typeof(bool), typeof(bool?));

        private void DumpLabel(LabelTarget target) {
            if (!target.Name.IsNullOrEmpty()) {
                Write(target.Name);
            } else {
                Write($"UnnamedLabel_{GetLabelId(target)}");
            }
        }

        protected override void WriteBinary(BinaryExpression expr) {
            if (expr.NodeType == ArrayIndex) {
                WriteNode("Left", expr.Left);
                Write("[");
                WriteNode("Right", expr.Right);
                Write("]");
                return;
            }

            // when we start using C# 9, we can replace the .In calls with the OR pattern
            var op = expr.NodeType switch
            {
                AndAlso => "AndAlso",
                OrElse => "OrElse",
                Assign => "=",
                Equal => "==",
                NotEqual => "!=",
                GreaterThan => ">",
                LessThan => "<",
                GreaterThanOrEqual => ">=",
                LessThanOrEqual => "<=",
                var x when x.In(Add, AddChecked) => "+",
                var x when x.In(AddAssign, AddAssignChecked) => "+=",
                var x when x.In(Subtract, SubtractChecked) => "-",
                var x when x.In(SubtractAssign, SubtractAssignChecked) => "-=",
                Divide => "/",
                DivideAssign => "/=",
                Modulo => "%",
                ModuloAssign => "%=",
                var x when x.In(Multiply, MultiplyChecked) => "*",
                var x when x.In(MultiplyAssign, MultiplyAssignChecked) => "*=",
                LeftShift => "<<",
                LeftShiftAssign => "<<=",
                RightShift => ">>",
                RightShiftAssign => ">>=",
                And => IsBool(expr) ? "And" : "&",
                AndAssign => IsBool(expr) ? "&&=" : "&=",
                Or => IsBool(expr) ? "Or" : "|",
                OrAssign => IsBool(expr) ? "||=" : "|=",
                ExclusiveOr => "^",
                ExclusiveOrAssign => "^=",
                Power => FrameworkCompatible ? "^" : "**",
                PowerAssign => "**=",
                Coalesce => "??",
                _ => throw new InvalidOperationException()
            };

            Write('(');
            WriteNode("Left", expr.Left);
            Write(' ');
            Write(op);
            Write(' ');
            WriteNode("Right", expr.Right);
            Write(')');
        }

        protected override void WriteParameter(ParameterExpression expr) {
            if (expr.IsByRef) { Write("ref "); }
            var name = expr.Name;
            if (name.IsNullOrEmpty()) {
                name = $"Param_{GetParamId(expr)}";
            }
            Write(name);
        }

        protected override void WriteLambda(LambdaExpression expr) {
            if (expr.Parameters.Count == 1) {
                WriteNode("Parameters[0]", expr.Parameters[0]);
            } else {
                Write('(');
                WriteNodes("Parameters", expr.Parameters);
                Write(')');
            }
            Write(" => ");
            WriteNode("Body", expr.Body);
        }

        protected override void WriteListInit(ListInitExpression expr) {
            WriteNode("NewExpression", expr.NewExpression);
            Write(" {");
            WriteNodes("Initializers", expr.Initializers);
            Write('}');
        }

        protected override void WriteConditional(ConditionalExpression expr, object? metadata) {
            Write("IIF(");
            WriteNode("Test", expr.Test);
            Write(", ");
            WriteNode("IfTrue", expr.IfTrue);
            Write(", ");
            WriteNode("IfFalse", expr.IfFalse);
            Write(')');
        }

        protected override void WriteConstant(ConstantExpression expr) {
            if (expr.Value is null) {
                Write("null");
                return;
            }

            var sValue = expr.Value.ToString();
            if (expr.Value is string) {
                Write('\"');
                Write(sValue);
                Write('\"');
            } else if (sValue == expr.Value.GetType().ToString()) {
                Write("value(");
                Write(sValue);
                Write(')');
            } else {
                Write(sValue);
            }
        }

        protected override void WriteDebugInfo(DebugInfoExpression expr) {
            var s = string.Format(
                CultureInfo.CurrentCulture,
                "<DebugInfo({0}: {1}, {2}, {3}, {4})>",
                expr.Document.FileName,
                expr.StartLine,
                expr.StartColumn,
                expr.EndLine,
                expr.EndColumn
            );
            Write(s);
        }

        protected override void WriteRuntimeVariables(RuntimeVariablesExpression expr) {
            Write('(');
            WriteNodes("Variables", expr.Variables);
            Write(')');
        }

        protected override void WriteMemberAccess(MemberExpression expr) {
            var (instance, member) = (expr.Expression, expr.Member);
            if (instance is { }) {
                WriteNode("Expression", instance);
            } else {
                Write(member.DeclaringType!.Name);
            }
            Write('.');
            Write(member.Name);
        }

        protected override void WriteMemberInit(MemberInitExpression expr) {
            if (
                expr.NewExpression.Arguments.Count == 0 &&
                expr.NewExpression.Type.Name.Contains("<")
            ) {
                Write("new");
            } else {
                WriteNode("NewExpression", expr.NewExpression);
            }
            Write(" {");
            WriteNodes("Bindings", expr.Bindings);
            Write('}');
        }

        protected override void WriteBinding(MemberBinding binding) {
            Write(binding.Member.Name);
            Write(" = ");

            if (binding is MemberAssignment assignmentBinding) {
                WriteNode("Expression", assignmentBinding.Expression);
                return;
            }

            Write('{');

            (var items, var itemsPath) = binding switch
            {
                MemberListBinding listBinding => (listBinding.Initializers, "Initializers"),
                MemberMemberBinding memberBinding => (memberBinding.Bindings, "Bindings"),
                _ => (Empty<object>(), "")
            };

            WriteNodes(itemsPath, items);

            Write('}');
        }

        protected override void WriteElementInit(ElementInit elementInit) {
            Write(elementInit.AddMethod.ToString());
            Write('(');
            WriteNodes("Arguments", elementInit.Arguments);
            Write(')');
        }

        protected override void WriteInvocation(InvocationExpression expr) {
            Write("Invoke(");
            WriteNode("Expression", expr.Expression);
            if (expr.Arguments.Any()) { Write(", "); }
            WriteNodes("Arguments", expr.Arguments);
            Write(')');
        }

        protected override void WriteCall(MethodCallExpression expr) {
            var (argOffset, instance, instancePath) =
                expr.Method.HasAttribute<ExtensionAttribute>() ?
                    (1, expr.Arguments[0], "Arguments[0]") :
                    (0, expr.Object, "Object");
            var arguments = expr.Arguments.Select((x, index) => ($"Arguments[{index}]", x)).Skip(argOffset);

            if (instance is { }) {
                WriteNode(instancePath, instance);
                Write('.');
            }
            Write(expr.Method.Name);
            Write('(');
            WriteNodes(arguments);
            Write(')');
        }

        protected override void WriteNewArray(NewArrayExpression expr) {
            switch (expr.NodeType) {
                case NewArrayBounds:
                    Write("new ");
                    Write(expr.Type.ToString());
                    Write('(');
                    WriteNodes("Expressions", expr.Expressions);
                    Write(')');
                    break;
                case NewArrayInit:
                    Write("new [] {");
                    WriteNodes("Expressions", expr.Expressions);
                    Write('}');
                    break;
            }
        }

        protected override void WriteNew(NewExpression expr) {
            Write("new ");
            Write(expr.Type.Name);
            Write('(');
            var members = expr.Members;
            foreach (var (argument, i) in expr.Arguments.Select((x, i) => (x, i))) {
                if (i > 0) { Write(", "); }
                if (members is { }) {
                    Write(members[i].Name);
                    Write(" = ");
                }
                WriteNode($"Arguments[{i}]", argument);
            }
            Write(')');
        }

        protected override void WriteTypeBinary(TypeBinaryExpression expr) {
            Write('(');
            WriteNode("Expression", expr.Expression);
            Write(
                expr.NodeType switch
                {
                    TypeIs => " Is ",
                    TypeEqual => " TypeEqual ",
                    _ => throw new NotImplementedException()
                }
            );
            Write(expr.TypeOperand.Name);
            Write(')');
        }

        protected override void WriteUnary(UnaryExpression expr) {
            Write(
                expr.NodeType switch
                {
                    var x when x.In(Negate, NegateChecked) => "-",
                    Not => "Not(",
                    IsFalse => "IsFalse(",
                    IsTrue => "IsTrue(",
                    OnesComplement => "~(",
                    ArrayLength => "ArrayLength(",
                    ExpressionType.Convert => "Convert(",
                    ConvertChecked => "ConvertChecked(",
                    Throw => "throw(",
                    TypeAs => "(",
                    UnaryPlus => "+",
                    Unbox => "Unbox(",
                    Increment => "Increment(",
                    Decrement => "Decrement(",
                    PreIncrementAssign => "++",
                    PreDecrementAssign => "--",
                    Quote => "",
                    PostIncrementAssign => FrameworkCompatible ? "PostIncrementAssign(" : "",
                    PostDecrementAssign => FrameworkCompatible ? "PostDecrementAssign(" : "",
                    _ => throw new InvalidOperationException()
                }
            );

            if (expr.Operand is { }) {
                WriteNode("Operand", expr.Operand);
            }

            switch (expr.NodeType) {
                case var x when x.In(Negate, NegateChecked, UnaryPlus, PreDecrementAssign, PreIncrementAssign, Quote):
                    break;
                case TypeAs:
                    Write(" As ");
                    Write(expr.Type.Name);
                    Write(')');
                    break;
                case var x when x.In(ExpressionType.Convert, ConvertChecked):
                    if (!FrameworkCompatible) {
                        Write(", ");
                        Write(expr.Type.Name);
                    }
                    Write(')');
                    break;
                case PostIncrementAssign:
                    Write("++");
                    break;
                case PostDecrementAssign:
                    Write("--");
                    break;
                default:
                    Write(')');
                    break;
            }
        }

        protected override void WriteBlock(BlockExpression expr, object? metadata) {
            Write('{');
            foreach (var (v, index) in expr.Variables.WithIndex()) {
                Write("var ");
                WriteNode($"Variables[{index}]", v);
                Write(';');
            }
            Write(" ... }");
        }

        protected override void WriteDefault(DefaultExpression expr) {
            Write("default(");
            Write(expr.Type.Name);
            Write(')');
        }

        protected override void WriteLabel(LabelExpression expr) {
            Write("{ ... } ");
            DumpLabel(expr.Target);
            Write(':');
        }

        protected override void WriteGoto(GotoExpression expr) {
            var op = expr.Kind switch
            {
                GotoExpressionKind.Goto => "goto",
                Break => "break",
                Continue => "continue",
                Return => "return",
                _ => throw new InvalidOperationException(),
            };
            Write(op);
            if (!FrameworkCompatible) { Write(' '); }
            DumpLabel(expr.Target);
            if (expr.Value is { }) {
                Write(" (");
                WriteNode("Value", expr.Value);
                Write(")");
                if (FrameworkCompatible) { Write(' '); }
            }
        }

        protected override void WriteLoop(LoopExpression expr) => Write("loop { ... }");

        protected override void WriteSwitchCase(SwitchCase switchCase) {
            Write("case (");
            WriteNodes("TestValues", switchCase.TestValues);
            Write("): ...");
        }

        protected override void WriteSwitch(SwitchExpression expr) {
            Write("switch (");
            WriteNode("SwitchValue", expr.SwitchValue);
            Write(") { ... }");
        }

        protected override void WriteCatchBlock(CatchBlock catchBlock) {
            Write("catch (");
            Write(catchBlock.Test.Name);
            var name = catchBlock.Variable?.Name;
            if (!name.IsNullOrEmpty()) {
                if (!FrameworkCompatible) { Write(' '); }
                Write(name);
            }
            Write(") { ... }");
        }

        protected override void WriteTry(TryExpression expr) => Write("try { ... }");

        protected override void WriteIndex(IndexExpression expr) {
            if (expr.Object is null) {
                // per https://stackoverflow.com/questions/401232/static-indexers#comment46687777_401381
                // the CLR doesn't support static indexers; so this code should never be reached
                //Write(expr.Indexer.DeclaringType.Name);
                throw new NotImplementedException("Static indexeres are not allowed.");
            }

            WriteNode("Object", expr.Object);
            if (expr.Indexer is { }) {
                Write('.');
                Write(expr.Indexer.Name);
            }
            Write('[');
            WriteNodes("Arguments", expr.Arguments);
            Write(']');
        }

        protected override void WriteExtension(Expression expr) {
            var toString = expr.GetType().GetMethod("ToString", Type.EmptyTypes)!;
            if (toString.DeclaringType != typeof(Expression) && !toString.IsStatic) {
                Write(expr.ToString());
                return;
            }

            Write('[');
            Write(
                expr.NodeType == Extension ?
                    expr.GetType().FullName :
                    expr.NodeType.ToString()
            );
            Write(']');
        }

        protected override void WriteDynamic(DynamicExpression expr) {
            if (!FrameworkCompatible) { 
                WriteExtension(expr);
                return;
            }

            Write(
                expr.Binder switch
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
                    UnaryOperationBinder unary => unary.Operation.ToString(),
                    BinaryOperationBinder binary => binary.Operation.ToString(),
                    _ => "CallSiteBinder"
                }
            );
            Write('(');
            WriteNodes("Arguments", expr.Arguments);
            Write(')');
        }

        protected override void WriteLabelTarget(LabelTarget labelTarget) =>
            Write(
                labelTarget.Name.IsNullOrEmpty() ?
                    "UnamedLabel" : // It's this way in the original -- https://github.com/dotnet/runtime/issues/42681
                    labelTarget.Name
            );

        // writing the parameter for the declaration is the same as writing the parameter for usage
        protected override void WriteParameterDeclaration(ParameterExpression prm) => throw new NotImplementedException();
    }
}
#pragma warning restore IDE1006 // Naming Styles
