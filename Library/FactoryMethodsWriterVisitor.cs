using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using static ExpressionTreeToString.Globals;
using static ZSpitz.Util.Functions;
using static System.Linq.Expressions.Expression;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Linq.Enumerable;
using OneOf;
using static ZSpitz.Util.Language;
using static ExpressionTreeToString.Util.Functions;
using ExpressionTreeToString.Util;

namespace ExpressionTreeToString {
    public class FactoryMethodsWriterVisitor : BuiltinsWriterVisitor {
        private static readonly string[] insertionPointKeys = new[] { "usings", "declarations", "" };

        private Dictionary<ParameterExpression, int>? ids;

        private void writeUsings() {
            SetInsertionPoint("usings");
            var @using = language switch
            {
                CSharp => "// using static System.Linq.Expressions.Expression",
                VisualBasic => "' Imports System.Linq.Expressions.Expression",
                _ => throw new NotImplementedException()
            };
            Write(@using);
            WriteEOL();
            SetInsertionPoint("");
        }

        public FactoryMethodsWriterVisitor(object o, OneOf<string, Language?> languageArg, bool hasPathSpans = false)
                : base(o, languageArg.ResolveLanguage() ?? throw new ArgumentException("Invalid language"), insertionPointKeys, hasPathSpans) => 
            writeUsings();

        /// <param name="args">The arguments to write. If a tuple of string and node type, will write as single node. If a tuple of string and property type, will write as multiple nodes.</param>
        private void writeMethodCall(string name, IEnumerable args) {
            Write(name);
            Write("(");

            var wrapPreviousInNewline = false;
            var indented = false;

            args.Cast<object>().ForEach((x, index) => {
                var isTuple = TryTupleValues(x, out var values) && values.Length == 2;
                (var path, var arg) = isTuple ? ((string)values![0]!, values[1]) : ("", x);
                var parameterDeclaration = 
                    (name == "Lambda" && path.StartsWith("Parameters")) || 
                    (name == "Block" && path.StartsWith("Variables"));

                var writeNewline = false;
                var argType = arg?.GetType();
                if (wrapPreviousInNewline) {
                    writeNewline = true;
                    wrapPreviousInNewline = false;
                } else if (arg is { }) {
                    if (
                        (
                            argType!.InheritsFromOrImplementsAny(NodeTypes) ||
                            arg is MemberInfo ||
                            arg is CallSiteBinder
                        ) && !(
                            arg is ParameterExpression
                        )
                    ) {
                        writeNewline = true;
                        wrapPreviousInNewline = true;
                    }
                }

                if (index > 0) {
                    Write(",");
                }
                if (writeNewline) {
                    if (!indented) {
                        Indent();
                        indented = true;
                    }
                    WriteEOL();
                } else if (index > 0) {
                    Write(" ");
                }

                if (argType?.InheritsFromOrImplementsAny(NodeTypes) ?? false) {
                    WriteNode(path, arg!, parameterDeclaration);
                } else if (argType?.InheritsFromOrImplementsAny(PropertyTypes) ?? false) {
                    if (language == CSharp) {
                        Write("new[] {");
                    } else { // language == VisualBasic
                        Write("{");
                    }

                    var argList = arg is IEnumerable enumerable ? enumerable.ToObjectList() : Empty<object>().ToList();
                    if (argList.Any()) {
                        if (argList.Any(y => !(y is ParameterExpression))) {
                            Indent();
                            WriteEOL();
                            WriteNodes(path, argList, true, ", ", parameterDeclaration);
                            WriteEOL(true);
                        } else {
                            Write(" ");
                            WriteNodes(path, argList, false, ", ", parameterDeclaration);
                            Write(" ");
                        }
                    }

                    Write("}");
                } else {
                    Write(RenderLiteral(arg, language));
                }
            });

            if (indented) { Dedent(); }
            if (wrapPreviousInNewline || indented) {
                WriteEOL();
            }

            Write(")");
        }

        private void writeMethodCall(Expression<Action> expr) {
            if (expr.Body is not MethodCallExpression callExpr) { throw new ArgumentException("Not a MethodCallExpression"); }

            var args = NewArrayInit(
                typeof(object), 
                callExpr.Arguments.Select(x => 
                    !x.Type.IsValueType ? 
                        x : 
                        Convert(x, typeof(object)))
                ).ExtractValue() as object[];
            var names = callExpr.Arguments.Select(x => {
                if (x is MethodCallExpression callExpr1) {
                    if (callExpr1.Method.Name == "ToArray") {
                        x = callExpr1.Arguments[0];
                    } else if (
                        callExpr1.Method.IsIndexerMethod() && 
                        callExpr1.Arguments.Count == 1 && // TODO theoretically we could pass multiple values to the indexer
                        callExpr1.Arguments[0] is ConstantExpression cexpr &&
                        callExpr1.Object is MemberExpression mexpr
                    ) {
                        return $"{mexpr.Member.Name}[{cexpr.Value}]";
                    }
                }
                if (x is UnaryExpression unary && unary.NodeType == ExpressionType.Convert) { x = unary.Operand; }
                return (x as MemberExpression)?.Member.Name ?? "";
            });
            var pairs = names.Zip(args!, (name, arg) => (name, arg)).ToList();
            var lastPair = pairs.LastOrDefault();
            if ((lastPair.arg?.GetType().IsArray ?? false) && callExpr.Method.GetParameters().Last().HasAttribute<ParamArrayAttribute>()) {
                pairs.RemoveLast();
                (lastPair.arg as IEnumerable)!.Cast<object>().Select((innerArg, index) => ($"{lastPair.name}[{index}]", innerArg)).AddRangeTo(pairs);
            }
            writeMethodCall(callExpr.Method.Name, pairs.ToList());
        }

        private static readonly MethodInfo powerMethod = typeof(Math).GetMethod("Pow")!;
        protected override void WriteBinary(BinaryExpression expr) {
            var methodName = FactoryMethodNames[expr.NodeType];
            var args = new List<object?> {
                ("Left", expr.Left),
                ("Right", expr.Right)
            };
            var types = new List<Type> {
                typeof(Expression),
                typeof(Expression)
            };

            // Method property is always filled for Power/PowerAssign, even if the expression is generated usnig Expression.MakeBinary
            // It could, however, be something other than Math.Pow; that's handled further on
            if (expr.NodeType.In(ExpressionType.Power, ExpressionType.PowerAssign) && expr.Method == powerMethod) {
                writeMethodCall(methodName, args);
                return;
            }

            if (expr.Method is { } || expr.Conversion is { }) {
                args.Add(expr.Method);
                types.Add(typeof(MethodInfo));
            }
            if (expr.Conversion is { }) {
                args.Add(expr.Conversion);
                types.Add(typeof(LambdaExpression));
            }

            var mi = typeof(Expression).GetMethod(methodName, types.ToArray());
            if (mi is { }) {
                writeMethodCall(methodName, args);
                return;
            }

            if (expr.Conversion is { }) {
                writeMethodCall(() => MakeBinary(expr.NodeType, expr.Left, expr.Right, false, expr.Method, expr.Conversion));
            } else if (expr.Method is { }) {
                writeMethodCall(() => MakeBinary(expr.NodeType, expr.Left, expr.Right, false, expr.Method));
            } else {
                writeMethodCall(() => MakeBinary(expr.NodeType, expr.Left, expr.Right));
            }
        }

        protected override void WriteUnary(UnaryExpression expr) {
            var methodName = FactoryMethodNames[expr.NodeType];
            var args = new List<object> {
                ("Operand", expr.Operand)
            };
            var types = new List<Type> {
                typeof(Expression)
            };

            if (
                (expr.NodeType.In(ExpressionType.Convert, ExpressionType.ConvertChecked, ExpressionType.TypeAs, ExpressionType.Unbox)) ||
                (expr.NodeType == ExpressionType.Throw && expr.Type != typeof(void))
            ) {
                args.Add(expr.Type);
                types.Add(typeof(Type));
            }

            if (expr.Method is { }) {
                args.Add(expr.Method);
                types.Add(typeof(MethodInfo));
            }

            var mi = typeof(Expression).GetMethod(methodName, types.ToArray());
            if (mi is { }) {
                writeMethodCall(methodName, args);
                return;
            }

            if (expr.Method is { }) {
                writeMethodCall(() => MakeUnary(expr.NodeType, expr.Operand, expr.Type, expr.Method));
            } else {
                writeMethodCall(() => MakeUnary(expr.NodeType, expr.Operand, expr.Type));
            }
        }

        protected override void WriteLambda(LambdaExpression expr) {
            var args = new List<object>();
            if (!expr.Type.IsAction() && !expr.Type.IsFunc()) {
                args.Add(expr.Type);
            };
            args.Add(("Body", expr.Body));
            if (!expr.Name.IsNullOrWhitespace()) {
                args.Add(expr.Name);
            }
            if (expr.TailCall) {
                args.Add(expr.TailCall);
            }
            expr.Parameters.Select((x, index) => (object)($"Parameters[{index}]", x)).AddRangeTo(args);
            writeMethodCall("Lambda", args);
        }

        protected override void WriteParameter(ParameterExpression expr) => 
            Write(GetVariableName(expr, ref ids));

        protected override void WriteConstant(ConstantExpression expr) {
            if (
                (expr.Value == null && expr.Type != typeof(object)) ||
                (expr.Value != null && expr.Value.GetType() != expr.Type)
            ) {
                writeMethodCall(() => Constant(expr.Value, expr.Type));
                return;
            }
            writeMethodCall(() => Constant(expr.Value));
        }

        protected override void WriteMemberAccess(MemberExpression expr) {
            // closed over variable from oute scope
            if (expr.Expression?.Type.IsClosureClass() ?? false) {
                if (!(expr.Expression is ConstantExpression)) {
                    throw new NotImplementedException("Closure type returned from an expression other than ConstantExpression.");
                }

                var name = expr.Member.Name.Replace("$VB$Local_", "");
                var variable = Parameter(expr.Type, name);
                WriteNode("Expression", variable, true);
                return;
            }

            writeMethodCall(() => MakeMemberAccess(expr.Expression, expr.Member));
        }

        protected override void WriteNew(NewExpression expr) =>
            writeMethodCall(() => New(expr.Constructor!, expr.Arguments.ToArray()));

        protected override void WriteCall(MethodCallExpression expr) {
            if ((expr.Object?.Type.IsArray ?? false) && expr.Method.Name == "Get") {
                writeMethodCall(() => ArrayIndex(expr.Object, expr.Arguments.ToArray()));
                return;
            } else if (expr.Method.IsIndexerMethod(out var pi)) {
                writeMethodCall(() => Property(expr.Object, pi, expr.Arguments.ToArray()));
                return;
            }

            if (expr.Object == null) {
                writeMethodCall(() => Call(expr.Method, expr.Arguments.ToArray()));
            } else {
                writeMethodCall(() => Call(expr.Object, expr.Method, expr.Arguments.ToArray()));
            }
        }

        protected override void WriteMemberInit(MemberInitExpression expr) =>
            writeMethodCall(() => MemberInit(expr.NewExpression, expr.Bindings.ToArray()));

        protected override void WriteListInit(ListInitExpression expr) =>
            writeMethodCall(() => ListInit(expr.NewExpression, expr.Initializers.ToArray()));

        protected override void WriteNewArray(NewArrayExpression expr) {
            var elementType = expr.Type.GetElementType()!;
            switch (expr.NodeType) {
                case ExpressionType.NewArrayInit:
                    writeMethodCall(() => NewArrayInit(elementType, expr.Expressions.ToArray()));
                    break;
                case ExpressionType.NewArrayBounds:
                    writeMethodCall(() => NewArrayBounds(elementType, expr.Expressions.ToArray()));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void WriteConditional(ConditionalExpression expr, object? metadata) {
            if (expr.Type != typeof(void)) {
                writeMethodCall(() => Condition(expr.Test, expr.IfTrue, expr.IfFalse));
            } else if (expr.IfFalse.IsEmpty()) {
                writeMethodCall(() => IfThen(expr.Test, expr.IfTrue));
            } else {
                writeMethodCall(() => IfThenElse(expr.Test, expr.IfTrue, expr.IfFalse));
            }
        }

        protected override void WriteDefault(DefaultExpression expr) {
            if (expr.Type == typeof(void)) {
                writeMethodCall(() => Empty());
                return;
            }
            writeMethodCall(() => Default(expr.Type));
        }

        protected override void WriteTypeBinary(TypeBinaryExpression expr) {
            switch (expr.NodeType) {
                case ExpressionType.TypeIs:
                    writeMethodCall(() => TypeIs(expr.Expression, expr.TypeOperand));
                    break;
                case ExpressionType.TypeEqual:
                    writeMethodCall(() => TypeEqual(expr.Expression, expr.TypeOperand));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void WriteInvocation(InvocationExpression expr) =>
            writeMethodCall(() => Invoke(expr.Expression, expr.Arguments.ToArray()));

        protected override void WriteIndex(IndexExpression expr) {
            // no such thing as a static indexer, so expr.Object wll never be null

            if (expr.Indexer != null) {
                writeMethodCall(() => MakeIndex(expr.Object!, expr.Indexer, expr.Arguments));
                return;
            }
            writeMethodCall(() => ArrayAccess(expr.Object!, expr.Arguments.ToArray()));
        }

        protected override void WriteBlock(BlockExpression expr, object? metadata) {
            if (expr.Type != expr.Expressions.Last().Type) {
                if (expr.Variables.Any()) {
                    writeMethodCall(() => Block(expr.Type, expr.Variables, expr.Expressions.ToArray()));
                } else {
                    writeMethodCall(() => Block(expr.Type, expr.Expressions.ToArray()));
                }
            } else {
                if (expr.Variables.Any()) {
                    writeMethodCall(() => Block(expr.Variables, expr.Expressions.ToArray()));
                } else {
                    writeMethodCall(() => Block(expr.Expressions.ToArray()));
                }
            }
        }

        protected override void WriteSwitch(SwitchExpression expr) {
            if (expr.DefaultBody == null) {
                writeMethodCall(() => Switch(expr.SwitchValue, expr.Cases.ToArray()));
            } else {
                writeMethodCall(() => Switch(expr.SwitchValue, expr.DefaultBody, expr.Cases.ToArray()));
            }
        }

        protected override void WriteTry(TryExpression expr) {
            if (expr.Fault != null) {
                if (expr.Finally != null || expr.Handlers.Any()) {
                    writeMethodCall(() => MakeTry(expr.Type, expr.Body, expr.Finally, expr.Fault, expr.Handlers));
                } else {
                    writeMethodCall(() => TryFault(expr.Body, expr.Fault));
                }
            } else if (expr.Finally != null) {
                if (expr.Handlers.Any()) {
                    writeMethodCall(() => TryCatchFinally(expr.Body, expr.Finally, expr.Handlers.ToArray()));
                } else {
                    writeMethodCall(() => TryFinally(expr.Body, expr.Finally));
                }
            } else {
                writeMethodCall(() => TryCatch(expr.Body, expr.Handlers.ToArray()));
            }
        }

        protected override void WriteLabel(LabelExpression expr) {
            if (expr.DefaultValue?.IsEmpty() ?? true) {
                writeMethodCall(() => Label(expr.Target));
            } else {
                writeMethodCall(() => Label(expr.Target, expr.DefaultValue));
            }
        }

        protected override void WriteGoto(GotoExpression expr) {
            var methodName = expr.Kind switch
            {
                GotoExpressionKind.Break => "Break",
                GotoExpressionKind.Continue => "Continue",
                GotoExpressionKind.Return => "Return",
                GotoExpressionKind.Goto => "Goto",
                _ => throw new NotImplementedException(),
            };
            var args = new List<(string, object)> { ("Target", expr.Target) };
            if (expr.Value != null) { args.Add(("Value", expr.Value)); }
            writeMethodCall(methodName, args);
        }

        protected override void WriteLoop(LoopExpression expr) {
            if (expr.BreakLabel != null && expr.ContinueLabel != null) {
                writeMethodCall(() => Loop(expr.Body, expr.BreakLabel, expr.ContinueLabel));
            } else if (expr.BreakLabel != null) {
                writeMethodCall(() => Loop(expr.Body, expr.BreakLabel));
            } else {
                writeMethodCall(() => Loop(expr.Body));
            }
        }

        protected override void WriteRuntimeVariables(RuntimeVariablesExpression expr) =>
            writeMethodCall(() => RuntimeVariables(expr.Variables.ToArray()));
        protected override void WriteDebugInfo(DebugInfoExpression expr) {
            if (expr.IsClear) {
                writeMethodCall(() => ClearDebugInfo(expr.Document));
            } else {
                writeMethodCall(() => DebugInfo(expr.Document, expr.StartLine, expr.StartColumn, expr.EndLine, expr.EndColumn));
            }
        }

        protected override void WriteElementInit(ElementInit elementInit) =>
            writeMethodCall(() => ElementInit(elementInit.AddMethod, elementInit.Arguments.ToArray()));

        protected override void WriteBinding(MemberBinding binding) {
            switch (binding) {
                case MemberAssignment assignmentBinding:
                    writeMethodCall(() => Bind(assignmentBinding.Member, assignmentBinding.Expression));
                    break;
                case MemberListBinding listBinding:
                    writeMethodCall(() => ListBind(listBinding.Member, listBinding.Initializers.ToArray()));
                    break;
                case MemberMemberBinding memberMemberBinding:
                    writeMethodCall(() => MemberBind(memberMemberBinding.Member, memberMemberBinding.Bindings.ToArray()));
                    break;
            }
        }

        protected override void WriteSwitchCase(SwitchCase switchCase) =>
            writeMethodCall(() => SwitchCase(switchCase.Body, switchCase.TestValues.ToArray()));

        protected override void WriteCatchBlock(CatchBlock catchBlock) {
            if (catchBlock.Variable != null) {
                if (catchBlock.Variable.Type != catchBlock.Test) {
                    writeMethodCall(() => MakeCatchBlock(catchBlock.Test, catchBlock.Variable, catchBlock.Body, catchBlock.Filter));
                } else if (catchBlock.Filter != null) {
                    writeMethodCall(() => Catch(catchBlock.Variable, catchBlock.Body, catchBlock.Filter));
                } else {
                    writeMethodCall(() => Catch(catchBlock.Variable, catchBlock.Body));
                }
            } else {
                if (catchBlock.Filter != null) {
                    writeMethodCall(() => Catch(catchBlock.Test, catchBlock.Body, catchBlock.Filter));
                } else {
                    writeMethodCall(() => Catch(catchBlock.Test, catchBlock.Body));
                }
            }
        }

        protected override void WriteLabelTarget(LabelTarget labelTarget) {
            if (labelTarget.Type == typeof(void)) {
                if (labelTarget.Name == null) {
                    writeMethodCall(() => Label());
                } else {
                    writeMethodCall(() => Label(labelTarget.Name));
                }
            } else {
                if (labelTarget.Name == null) {
                    writeMethodCall(() => Label(labelTarget.Type));
                } else {
                    writeMethodCall(() => Label(labelTarget.Type, labelTarget.Name));
                }
            }
        }

        protected override void WriteDynamic(DynamicExpression expr) {
            Expression<Action> callExpr = expr.Arguments.Count switch
            {
                1 => () => Dynamic(expr.Binder, expr.Type, expr.Arguments[0]),
                2 => () => Dynamic(expr.Binder, expr.Type, expr.Arguments[0], expr.Arguments[1]),
                3 => () => Dynamic(expr.Binder, expr.Type, expr.Arguments[0], expr.Arguments[1], expr.Arguments[2]),
                4 => () => Dynamic(expr.Binder, expr.Type, expr.Arguments[0], expr.Arguments[1], expr.Arguments[2], expr.Arguments[3]),
                _ => () => Dynamic(expr.Binder, expr.Type, expr.Arguments)
            };
            writeMethodCall(callExpr);
        }

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
        protected override void WriteParameterDeclaration(ParameterExpression prm) {
            SetInsertionPoint("declarations");

            if (language == CSharp) {
                Write($"var {GetVariableName(prm, ref ids)} = ");
            } else { // language == VisualBasic
                Write($"Dim {GetVariableName(prm, ref ids)} = ");
            }
            if (prm.IsByRef) {
                var type = prm.Type.MakeByRefType();
                var args = prm.Name.IsNullOrWhitespace() ?
                    new (string, object)[] { ("Type", type) } :
                    new (string, object)[] { ("Type", type), ("Name", prm.Name) };
                writeMethodCall("Parameter", args);
            } else {
                if (prm.Name.IsNullOrWhitespace()) {
                    writeMethodCall(() => Parameter(prm.Type));
                } else {
                    writeMethodCall(() => Parameter(prm.Type, prm.Name));
                }
            }
            if (language == CSharp) { Write(";"); }
            WriteEOL();

            SetInsertionPoint("");

            Write(GetVariableName(prm, ref ids));
        }
    }
}
