using OneOf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using ZSpitz.Util;
using static System.Linq.Expressions.ExpressionType;
using static ExpressionTreeToString.Globals;

namespace ExpressionTreeToString {
    public abstract class BuiltinsWriterVisitor : WriterVisitorBase {
        protected BuiltinsWriterVisitor(object o, OneOf<string, Language?> languageArg, IEnumerable<string>? insertionPointKeys, bool hasPathSpans) 
            : base(o, languageArg, insertionPointKeys, hasPathSpans) { }

        protected override void WriteNodeImpl(object o, bool parameterDeclaration = false, object? metadata = null) {
            switch (o) {
                case ParameterExpression pexpr when parameterDeclaration:
                    WriteParameterDeclaration(pexpr);
                    break;
                case BlockExpression bexpr when metadata != null:
                    WriteBlock(bexpr, metadata);
                    break;
                case ConditionalExpression cexpr when metadata != null:
                    WriteConditional(cexpr, metadata);
                    break;
                case Expression expr:
                    writeExpression(expr);
                    break;
                case MemberBinding binding:
                    WriteBinding(binding);
                    break;
                case ElementInit init:
                    WriteElementInit(init);
                    break;
                case SwitchCase switchCase:
                    WriteSwitchCase(switchCase);
                    break;
                case CatchBlock catchBlock:
                    WriteCatchBlock(catchBlock);
                    break;
                case LabelTarget labelTarget:
                    WriteLabelTarget(labelTarget);
                    break;

                default:
                    throw new NotImplementedException($"Code generation not implemented for type '{o.GetType().Name}'");
            }
        }

        private void writeExpression(Expression expr) {
            switch (expr.NodeType) {

                case var nodeType when nodeType.In(BinaryExpressionTypes):
                    WriteBinary((BinaryExpression)expr);
                    break;

                case var nodeType when nodeType.In(UnaryExpressionTypes):
                    WriteUnary((UnaryExpression)expr);
                    break;

                case Lambda:
                    WriteLambda((LambdaExpression)expr);
                    break;

                case Parameter:
                    WriteParameter((ParameterExpression)expr);
                    break;

                case Constant:
                    WriteConstant((ConstantExpression)expr);
                    break;

                case MemberAccess:
                    WriteMemberAccess((MemberExpression)expr);
                    break;

                case New:
                    WriteNew((NewExpression)expr);
                    break;

                case Call:
                    WriteCall((MethodCallExpression)expr);
                    break;

                case MemberInit:
                    WriteMemberInit((MemberInitExpression)expr);
                    break;

                case ListInit:
                    WriteListInit((ListInitExpression)expr);
                    break;

                case NewArrayInit:
                case NewArrayBounds:
                    WriteNewArray((NewArrayExpression)expr);
                    break;

                case Conditional:
                    WriteConditional((ConditionalExpression)expr, null);
                    break;

                case Default:
                    WriteDefault((DefaultExpression)expr);
                    break;

                case TypeIs:
                case TypeEqual:
                    WriteTypeBinary((TypeBinaryExpression)expr);
                    break;

                case Invoke:
                    WriteInvocation((InvocationExpression)expr);
                    break;

                case Index:
                    WriteIndex((IndexExpression)expr);
                    break;

                case Block:
                    WriteBlock((BlockExpression)expr, null);
                    break;

                case Switch:
                    WriteSwitch((SwitchExpression)expr);
                    break;

                case Try:
                    WriteTry((TryExpression)expr);
                    break;

                case Label:
                    WriteLabel((LabelExpression)expr);
                    break;

                case Goto:
                    WriteGoto((GotoExpression)expr);
                    break;

                case Loop:
                    WriteLoop((LoopExpression)expr);
                    break;

                case RuntimeVariables:
                    WriteRuntimeVariables((RuntimeVariablesExpression)expr);
                    break;

                case DebugInfo:
                    WriteDebugInfo((DebugInfoExpression)expr);
                    break;

                case Dynamic:
                    WriteDynamic((DynamicExpression)expr);
                    break;

                case Extension:
                    WriteExtension(expr);
                    break;

                default:
                    throw new NotImplementedException($"NodeType: {expr.NodeType}, Expression object type: {expr.GetType().Name}");
            }
        }

        protected virtual void WriteDynamic(DynamicExpression expr) {
            switch (expr.Binder) {
                case BinaryOperationBinder binaryOperationBinder:
                    WriteBinaryOperationBinder(binaryOperationBinder, expr.Arguments);
                    break;
                case ConvertBinder convertBinder:
                    WriteConvertBinder(convertBinder, expr.Arguments);
                    break;
                case CreateInstanceBinder createInstanceBinder:
                    WriteCreateInstanceBinder(createInstanceBinder, expr.Arguments);
                    break;
                case DeleteIndexBinder deleteIndexBinder:
                    WriteDeleteIndexBinder(deleteIndexBinder, expr.Arguments);
                    break;
                case DeleteMemberBinder deleteMemberBinder:
                    WriteDeleteMemberBinder(deleteMemberBinder, expr.Arguments);
                    break;
                case GetIndexBinder getIndexBinder:
                    WriteGetIndexBinder(getIndexBinder, expr.Arguments);
                    break;
                case GetMemberBinder getMemberBinder:
                    WriteGetMemberBinder(getMemberBinder, expr.Arguments);
                    break;
                case InvokeBinder invokeBinder:
                    WriteInvokeBinder(invokeBinder, expr.Arguments);
                    break;
                case InvokeMemberBinder invokeMemberBinder:
                    WriteInvokeMemberBinder(invokeMemberBinder, expr.Arguments);
                    break;
                case SetIndexBinder setIndexBinder:
                    WriteSetIndexBinder(setIndexBinder, expr.Arguments);
                    break;
                case SetMemberBinder setMemberBinder:
                    WriteSetMemberBinder(setMemberBinder, expr.Arguments);
                    break;
                case UnaryOperationBinder unaryOperationBinder:
                    WriteUnaryOperationBinder(unaryOperationBinder, expr.Arguments);
                    break;

                default:
                    throw new NotImplementedException($"Dynamic expression with binder type {expr.Binder} not implemented");
            }
        }

        protected virtual void WriteExtension(Expression expr) => throw new NotImplementedException("NodeType: Exension not implemented.");

        // .NET 3.5 expression types
        protected abstract void WriteBinary(BinaryExpression expr);
        protected abstract void WriteUnary(UnaryExpression expr);
        protected abstract void WriteLambda(LambdaExpression expr);
        protected abstract void WriteParameter(ParameterExpression expr);
        protected abstract void WriteConstant(ConstantExpression expr);
        protected abstract void WriteMemberAccess(MemberExpression expr);
        protected abstract void WriteNew(NewExpression expr);
        protected abstract void WriteCall(MethodCallExpression expr);
        protected abstract void WriteMemberInit(MemberInitExpression expr);
        protected abstract void WriteListInit(ListInitExpression expr);
        protected abstract void WriteNewArray(NewArrayExpression expr);
        protected abstract void WriteConditional(ConditionalExpression expr, object? metadata);
        protected abstract void WriteDefault(DefaultExpression expr);
        protected abstract void WriteTypeBinary(TypeBinaryExpression expr);
        protected abstract void WriteInvocation(InvocationExpression expr);
        protected abstract void WriteIndex(IndexExpression expr);

        // .NET 4 expression types
        protected abstract void WriteBlock(BlockExpression expr, object? metadata);
        protected abstract void WriteSwitch(SwitchExpression expr);
        protected abstract void WriteTry(TryExpression expr);
        protected abstract void WriteLabel(LabelExpression expr);
        protected abstract void WriteGoto(GotoExpression expr);
        protected abstract void WriteLoop(LoopExpression expr);
        protected abstract void WriteRuntimeVariables(RuntimeVariablesExpression expr);
        protected abstract void WriteDebugInfo(DebugInfoExpression expr);

        // other types
        protected abstract void WriteElementInit(ElementInit elementInit);
        protected abstract void WriteBinding(MemberBinding binding);
        protected abstract void WriteSwitchCase(SwitchCase switchCase);
        protected abstract void WriteCatchBlock(CatchBlock catchBlock);
        protected abstract void WriteLabelTarget(LabelTarget labelTarget);

        // binders
        protected abstract void WriteBinaryOperationBinder(BinaryOperationBinder binaryOperationBinder, IList<Expression> args);
        protected abstract void WriteConvertBinder(ConvertBinder convertBinder, IList<Expression> args);
        protected abstract void WriteCreateInstanceBinder(CreateInstanceBinder createInstanceBinder, IList<Expression> args);
        protected abstract void WriteDeleteIndexBinder(DeleteIndexBinder deleteIndexBinder, IList<Expression> args);
        protected abstract void WriteDeleteMemberBinder(DeleteMemberBinder deleteMemberBinder, IList<Expression> args);
        protected abstract void WriteGetIndexBinder(GetIndexBinder getIndexBinder, IList<Expression> args);
        protected abstract void WriteGetMemberBinder(GetMemberBinder getMemberBinder, IList<Expression> args);
        protected abstract void WriteInvokeBinder(InvokeBinder invokeBinder, IList<Expression> args);
        protected abstract void WriteInvokeMemberBinder(InvokeMemberBinder invokeMemberBinder, IList<Expression> args);
        protected abstract void WriteSetIndexBinder(SetIndexBinder setIndexBinder, IList<Expression> args);
        protected abstract void WriteSetMemberBinder(SetMemberBinder setMemberBinder, IList<Expression> args);
        protected abstract void WriteUnaryOperationBinder(UnaryOperationBinder unaryOperationBinder, IList<Expression> args);

        protected abstract void WriteParameterDeclaration(ParameterExpression prm);
    }
}
