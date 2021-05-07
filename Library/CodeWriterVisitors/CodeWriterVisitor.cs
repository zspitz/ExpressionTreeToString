using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionType;
using ZSpitz.Util;
using OneOf;
using static ZSpitz.Util.Functions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Dynamic;
using static ZSpitz.Util.Methods;
using ExpressionTreeToString.Util;
using static ExpressionTreeToString.Util.Functions;

namespace ExpressionTreeToString {
    public abstract class CodeWriterVisitor : BuiltinsWriterVisitor {
        protected CodeWriterVisitor(object o, OneOf<string, Language?> languageArg, bool hasPathSpans)
            : base(o, languageArg, null, hasPathSpans) { }

        protected Dictionary<ParameterExpression, int>? IDs;

        protected override void WriteUnary(UnaryExpression expr) =>
            WriteUnary(expr.NodeType, "Operand", expr.Operand, expr.Type, expr.GetType().Name);

        protected abstract void WriteBinary(ExpressionType nodeType, string leftPath, Expression left, string rightPath, Expression right);

        protected override void WriteBinary(BinaryExpression expr) {
            if (TryGetEnumComparison(expr, out var parts)) {
                var (leftOperand, leftPath, rightOperand, rightPath) = parts;
                WriteBinary(expr.NodeType, leftPath!, leftOperand, rightPath!, rightOperand);
                return;
            }

            WriteBinary(expr.NodeType, "Left", expr.Left, "Right", expr.Right);
        }

        protected override void WriteParameter(ParameterExpression expr) => Write(GetVariableName(expr, ref IDs));

        protected override void WriteConstant(ConstantExpression expr) =>
            Write(RenderLiteral(expr.Value, language));

        protected override void WriteMemberAccess(MemberExpression expr) {
            switch (expr.Expression) {
                case ConstantExpression cexpr when cexpr.Type.IsClosureClass():
                case MemberExpression mexpr when mexpr.Type.IsClosureClass():
                    // closed over variable from outer scope
                    Write(expr.Member.Name.Replace("$VB$Local_", ""));
                    return;
                case null:
                    // static member
                    Write($"{expr.Member.DeclaringType?.FriendlyName(language)}.{expr.Member.Name}");
                    return;
                default:
                    Parens(expr, "Expression", expr.Expression);
                    Write($".{expr.Member.Name}");
                    return;
            }
        }

        protected abstract (string prefix, string suffix) GenericIndicators { get; }
        protected abstract bool ParensOnEmptyArguments { get; }

        protected override void WriteCall(MethodCallExpression expr) {
            if (expr.Method.IsStringConcat()) {
                var firstArg = expr.Arguments[0];
                IEnumerable<Expression>? argsToWrite = null;
                var argsPath = "";
                if (firstArg is NewArrayExpression newArray && firstArg.NodeType == NewArrayInit) {
                    argsToWrite = newArray.Expressions;
                    argsPath = "Arguments[0].Expressions";
                } else if (expr.Arguments.All(x => x.Type == typeof(string))) {
                    argsToWrite = expr.Arguments;
                    argsPath = "Arguments";
                }
                if (argsToWrite != null) {
                    WriteNodes(argsPath, argsToWrite, " + ");
                    return;
                }
            }

            var isIndexer = 
                (expr.Object?.Type.IsArray ?? false) && expr.Method.Name == "Get" || 
                expr.Method.IsIndexerMethod();
            if (isIndexer) {
                // no such thing as a static indexer; expr.Object will not be null
                writeIndexerAccess("Object", expr.Object!, "Arguments", expr.Arguments);
                return;
            }

            if (expr.Method.IsStringFormat() && expr.Arguments[0] is ConstantExpression cexpr && cexpr.Value is string format) {
                var parts = ParseFormatString(format);
                Write("$\"");
                foreach (var (literal, index, alignment, itemFormat) in parts) {
                    Write(literal.Replace("{", "{{").Replace("}", "}}"));
                    if (index == null) { break; }
                    Write("{");
                    WriteNode($"Arguments[{index.Value + 1}]", expr.Arguments[index.Value + 1]);
                    if (alignment != null) { Write($", {alignment}"); }
                    if (itemFormat != null) { Write($":{itemFormat}"); }
                    Write("}");
                }
                Write("\"");
                return;
            }

            var (path, o) = ("Object", expr.Object);
            var skip = 0;

            if (expr.Object == null && expr.Method.HasAttribute<ExtensionAttribute>()) {
                (path, o) = ("Arguments[0]", expr.Arguments[0]);
                skip = 1;
            }
            var arguments = expr.Arguments.Skip(skip).Select((x, index) => ($"Arguments[{index + skip}]", x));

            if (o is null) {
                // static non-extension method -- write the type name
                Write(expr.Method.ReflectedType.FriendlyName(language));
            } else {
                // instance method, or extension method
                Parens(expr, path, o);
            }

            var typeParameters = "";
            if (expr.Method.IsGenericMethod) {
                var def = expr.Method.GetGenericMethodDefinition();
                var args = def.GetGenericArguments();
                var parameterTypes = def.GetParameters().Select(x => x.ParameterType).ToArray();
                if (args.Any(arg => parameterTypes.None(prm => prm.ContainsType(arg)))) {
                    typeParameters = $"{GenericIndicators.prefix}{expr.Method.GetGenericArguments().Joined(", ", t => t.FriendlyName(language)!)}{GenericIndicators.suffix}";
                }
            }
            Write($".{expr.Method.Name}{typeParameters}");

            if (arguments.Any() || ParensOnEmptyArguments) {
                Write("(");
                WriteNodes(arguments);
                Write(")");
            }
        }

        protected abstract (string prefix, string suffix) IndexerIndicators { get; }

        protected void WriteIndexerAccess(string instancePath, Expression instance, string argBasePath, params Expression[] keys) {
            WriteNode(instancePath, instance);
            Write(IndexerIndicators.prefix);
            WriteNodes(argBasePath, keys);
            Write(IndexerIndicators.suffix);
        }
        private void writeIndexerAccess(string instancePath, Expression instance, string argBasePath, IEnumerable<Expression> keys) =>
            WriteIndexerAccess(instancePath, instance, argBasePath, keys.ToArray());

        protected override void WriteIndex(IndexExpression expr) =>
            writeIndexerAccess("Object", expr.Object!, "Arguments", expr.Arguments); // no indexers on static classes

        protected override void WriteElementInit(ElementInit elementInit) {
            var args = elementInit.Arguments;
            switch (args.Count) {
                case 0:
                    throw new NotImplementedException();
                case 1:
                    WriteNode("Arguments[0]", args[0]);
                    break;
                default:
                    Write("{");
                    Indent();
                    WriteEOL();
                    WriteNodes("Arguments", args, true);
                    WriteEOL(true);
                    Write("}");
                    break;
            }
        }

        // VB.NET also requires the parentheses when invoking a delegate
        // that's why we're not checking for arguments
        protected override void WriteInvocation(InvocationExpression expr) {
            if (expr.Expression is LambdaExpression) { Write("("); }
            WriteNode("Expression", expr.Expression);
            if (expr.Expression is LambdaExpression) { Write(")"); }
            Write("(");
            WriteNodes("Arguments", expr.Arguments);
            Write(")");
        }

        protected override void WriteDeleteIndexBinder(DeleteIndexBinder binder, IList<Expression> args) =>
            throw new NotImplementedException();
        protected override void WriteDeleteMemberBinder(DeleteMemberBinder binder, IList<Expression> args) =>
            throw new NotImplementedException();

        protected override void WriteBinaryOperationBinder(BinaryOperationBinder binder, IList<Expression> args) {
            VerifyCount(args, 2);
            WriteBinary(binder.Operation, "Arguments[0]", args[0], "Arguments[1]", args[1]);
        }

        protected abstract void WriteUnary(ExpressionType nodeType, string operandPath, Expression operand, Type type, string expressionTypename);

        protected override void WriteConvertBinder(ConvertBinder binder, IList<Expression> args) {
            VerifyCount(args, 1);
            WriteUnary(ExpressionType.Convert, "Arguments[0]", args[0], binder.Type, typeof(ConvertBinder).Name);
        }

        protected abstract void WriteNew(Type type, string argsPath, IList<Expression> args);

        protected override void WriteCreateInstanceBinder(CreateInstanceBinder binder, IList<Expression> args) =>
            WriteNew(binder.ReturnType, "Arguments", args);

        protected override void WriteGetIndexBinder(GetIndexBinder binder, IList<Expression> args) {
            VerifyCount(args, 2, null);
            WriteNode("Arguments[0]", args[0]);
            Write(IndexerIndicators.prefix);
            WriteNodes(args.Skip(1).Select((arg, index) => ($"Arguments[{index + 1}]", arg)));
            Write(IndexerIndicators.suffix);
        }

        protected override void WriteGetMemberBinder(GetMemberBinder binder, IList<Expression> args) {
            VerifyCount(args, 1);
            WriteNode("Arguments[0]", args[0]);
            Write($".{binder.Name}");
        }

        protected override void WriteInvokeBinder(InvokeBinder binder, IList<Expression> args) {
            VerifyCount(args, 1, null);
            WriteNode("Arguments[0]", args[0]);
            if (ParensOnEmptyArguments || args.Count > 1) {
                Write("(");
                WriteNodes(args.Skip(1).Select((arg, index) => ($"Arguments[{index + 1}]", arg)));
                Write(")");
            }
        }

        protected override void WriteInvokeMemberBinder(InvokeMemberBinder binder, IList<Expression> args) {
            VerifyCount(args, 1, null);
            Parens(Call, "Arguments[0]", args[0]);
            Write($".{binder.Name}");
            if (ParensOnEmptyArguments || args.Count > 1) {
                Write("(");
                WriteNodes(args.Skip(1).Select((arg, index) => ($"Arguments[{index + 1}]", arg)));
                Write(")");
            }
        }

        protected override void WriteSetIndexBinder(SetIndexBinder binder, IList<Expression> args) {
            VerifyCount(args, 3, null);
            WriteNode("Arguments[0]", args[0]);
            Write(IndexerIndicators.prefix);
            WriteNodes(args.Skip(2).Select((arg, index) => ($"Arguments[{index + 2}]", arg)));
            Write($"{IndexerIndicators.suffix} = ");
            WriteNode("Arguments[1]", args[1]);
        }

        protected override void WriteSetMemberBinder(SetMemberBinder binder, IList<Expression> args) {
            VerifyCount(args, 2);
            WriteNode("Arguments[0]", args[0]);
            Write($".{binder.Name} = ");
            WriteNode("Arguments[1]", args[1]);
        }

        protected override void WriteUnaryOperationBinder(UnaryOperationBinder binder, IList<Expression> args) {
            VerifyCount(args, 1);
            WriteUnary(binder.Operation, "Arguments[0]", args[0], binder.ReturnType, binder.GetType().Name);
        }

        protected abstract void Parens(OneOf<Expression, ExpressionType> nodeTypeSource, string path, Expression childNode);
    }
}
