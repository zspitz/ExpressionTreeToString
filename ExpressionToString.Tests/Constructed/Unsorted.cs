﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;
using static ExpressionToString.Tests.Globals;
using static ExpressionToString.Tests.Runners;
using static System.Linq.Expressions.Expression;

namespace ExpressionToString.Tests.Constructed {
    [Trait("Source", "Autogenerated")]
    public class Unsorted {
        readonly bool @bool;
        readonly CallSiteBinder callSiteBinder;
        readonly CatchBlock[] catchBlockArray;
        readonly ConstructorInfo constructorInfo;
        readonly ElementInit[] elementInitArray;
        readonly Expression expression;
        readonly Expression[] expressionArray;
        readonly ExpressionType expressionType;
        readonly FieldInfo fieldInfo;
        readonly GotoExpressionKind gotoExpressionKind;
        readonly Guid guid;
        readonly IEnumerable<CatchBlock> iEnumerableOfCatchBlock;
        readonly IEnumerable<Expression> iEnumerableOfExpression;
        readonly IEnumerable<ParameterExpression> iEnumerableOfParameterExpression;
        readonly int @int;
        readonly LabelTarget labelTarget;
        readonly LambdaExpression lambdaExpression;
        readonly MemberBinding[] memberBindingArray;
        readonly MemberInfo memberInfo;
        readonly MemberInfo[] memberInfoArray;
        readonly MethodInfo methodInfo;
        readonly NewExpression newExpression;
        readonly object @object;
        readonly ParameterExpression parameterExpression;
        readonly ParameterExpression[] parameterExpressionArray;
        readonly PropertyInfo propertyInfo;
        readonly string @string;
        readonly SwitchCase[] switchCaseArray;
        readonly SymbolDocumentInfo symbolDocumentInfo;
        readonly Type type;
        readonly Type[] typeArray;

        #region BinaryExpression
        [Fact(Skip = "Autogenerated tests")]
        public void MakeBinary_Test() =>
            BuildAssert(
                MakeBinary(expressionType, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeBinary_1_Test() =>
            BuildAssert(
                MakeBinary(expressionType, expression, expression, @bool, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeBinary_2_Test() =>
            BuildAssert(
                MakeBinary(expressionType, expression, expression, @bool, methodInfo, lambdaExpression),
                "",
                ""
            );
        #endregion

        #region BlockExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Block_Test() =>
            BuildAssert(
                Block(expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Block_1_Test() =>
            BuildAssert(
                Block(expression, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Block_2_Test() =>
            BuildAssert(
                Block(expression, expression, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Block_3_Test() =>
            BuildAssert(
                Block(expression, expression, expression, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Block_4_Test() =>
            BuildAssert(
                Block(expressionArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Block_5_Test() =>
            BuildAssert(
                Block(type, expressionArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Block_6_Test() =>
            BuildAssert(
                Block(iEnumerableOfParameterExpression, expressionArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Block_7_Test() =>
            BuildAssert(
                Block(type, iEnumerableOfParameterExpression, expressionArray),
                "",
                ""
            );

        #endregion

        #region bool

        [Fact(Skip = "Autogenerated tests")]
        public void TryGetActionType_Test() {
            Type type1;
            BuildAssert(
                TryGetActionType(typeArray, out type1),
                "",
                ""
            );
            //TODO inspect type1
        }

        [Fact(Skip = "Autogenerated tests")]
        public void TryGetFuncType_Test() {
            Type type1;
            BuildAssert(
                TryGetFuncType(typeArray, out type1),
                "",
                ""
            );
            //TODO inspect type1
        }

        #endregion

        #region CatchBlock

        [Fact(Skip = "Autogenerated tests")]
        public void Catch_Test() =>
            BuildAssert(
                Catch(type, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Catch_1_Test() =>
            BuildAssert(
                Catch(parameterExpression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Catch_2_Test() =>
            BuildAssert(
                Catch(type, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Catch_3_Test() =>
            BuildAssert(
                Catch(parameterExpression, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeCatchBlock_Test() =>
            BuildAssert(
                MakeCatchBlock(type, parameterExpression, expression, expression),
                "",
                ""
            );

        #endregion

        #region DebugInfoExpression

        [Fact(Skip = "Autogenerated tests")]
        public void ClearDebugInfo_Test() =>
            BuildAssert(
                ClearDebugInfo(symbolDocumentInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void DebugInfo_Test() =>
            BuildAssert(
                DebugInfo(symbolDocumentInfo, @int, @int, @int, @int),
                "",
                ""
            );

        #endregion

        #region DefaultExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Empty_Test() =>
            BuildAssert(
                Empty(),
                "",
                ""
            );

        #endregion

        #region DynamicExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Dynamic_Test() =>
            BuildAssert(
                Dynamic(callSiteBinder, type, expressionArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Dynamic_1_Test() =>
            BuildAssert(
                Dynamic(callSiteBinder, type, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Dynamic_2_Test() =>
            BuildAssert(
                Dynamic(callSiteBinder, type, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Dynamic_3_Test() =>
            BuildAssert(
                Dynamic(callSiteBinder, type, expression, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Dynamic_4_Test() =>
            BuildAssert(
                Dynamic(callSiteBinder, type, expression, expression, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeDynamic_Test() =>
            BuildAssert(
                MakeDynamic(type, callSiteBinder, expressionArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeDynamic_1_Test() =>
            BuildAssert(
                MakeDynamic(type, callSiteBinder, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeDynamic_2_Test() =>
            BuildAssert(
                MakeDynamic(type, callSiteBinder, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeDynamic_3_Test() =>
            BuildAssert(
                MakeDynamic(type, callSiteBinder, expression, expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeDynamic_4_Test() =>
            BuildAssert(
                MakeDynamic(type, callSiteBinder, expression, expression, expression, expression),
                "",
                ""
            );

        #endregion

        #region GotoExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Break_Test() =>
            BuildAssert(
                Break(labelTarget),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Break_1_Test() =>
            BuildAssert(
                Break(labelTarget, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Break_2_Test() =>
            BuildAssert(
                Break(labelTarget, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Break_3_Test() =>
            BuildAssert(
                Break(labelTarget, expression, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Continue_Test() =>
            BuildAssert(
                Continue(labelTarget),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Continue_1_Test() =>
            BuildAssert(
                Continue(labelTarget, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Goto_Test() =>
            BuildAssert(
                Goto(labelTarget),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Goto_1_Test() =>
            BuildAssert(
                Goto(labelTarget, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Goto_2_Test() =>
            BuildAssert(
                Goto(labelTarget, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Goto_3_Test() =>
            BuildAssert(
                Goto(labelTarget, expression, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeGoto_Test() =>
            BuildAssert(
                MakeGoto(gotoExpressionKind, labelTarget, expression, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Return_Test() =>
            BuildAssert(
                Return(labelTarget),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Return_1_Test() =>
            BuildAssert(
                Return(labelTarget, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Return_2_Test() =>
            BuildAssert(
                Return(labelTarget, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Return_3_Test() =>
            BuildAssert(
                Return(labelTarget, expression, type),
                "",
                ""
            );

        #endregion

        #region IndexExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Property_Test() =>
            BuildAssert(
                Property(expression, @string, expressionArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Property_1_Test() =>
            BuildAssert(
                Property(expression, propertyInfo, expressionArray),
                "",
                ""
            );

        #endregion

        #region LabelExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Label_Test() =>
            BuildAssert(
                Label(labelTarget),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Label_1_Test() =>
            BuildAssert(
                Label(labelTarget, expression),
                "",
                ""
            );

        #endregion

        #region LabelTarget

        [Fact(Skip = "Autogenerated tests")]
        public void Label_2_Test() =>
            BuildAssert(
                Label(),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Label_3_Test() =>
            BuildAssert(
                Label(@string),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Label_4_Test() =>
            BuildAssert(
                Label(type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Label_5_Test() =>
            BuildAssert(
                Label(type, @string),
                "",
                ""
            );

        #endregion

        #region ListInitExpression

        [Fact(Skip = "Autogenerated tests")]
        public void ListInit_Test() =>
            BuildAssert(
                ListInit(newExpression, expressionArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void ListInit_1_Test() =>
            BuildAssert(
                ListInit(newExpression, methodInfo, expressionArray),
                "",
                ""
            );

        #endregion

        #region LoopExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Loop_Test() =>
            BuildAssert(
                Loop(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Loop_1_Test() =>
            BuildAssert(
                Loop(expression, labelTarget),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Loop_2_Test() =>
            BuildAssert(
                Loop(expression, labelTarget, labelTarget),
                "",
                ""
            );

        #endregion

        #region MemberAssignment

        [Fact(Skip = "Autogenerated tests")]
        public void Bind_1_Test() =>
            BuildAssert(
                Bind(methodInfo, expression),
                "",
                ""
            );

        #endregion

        #region MemberExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Field_Test() =>
            BuildAssert(
                Field(expression, fieldInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Field_1_Test() =>
            BuildAssert(
                Field(expression, @string),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Field_2_Test() =>
            BuildAssert(
                Field(expression, type, @string),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Property_2_Test() =>
            BuildAssert(
                Property(expression, @string),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Property_3_Test() =>
            BuildAssert(
                Property(expression, type, @string),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Property_4_Test() =>
            BuildAssert(
                Property(expression, propertyInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Property_5_Test() =>
            BuildAssert(
                Property(expression, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void PropertyOrField_Test() =>
            BuildAssert(
                PropertyOrField(expression, @string),
                "",
                ""
            );

        #endregion

        #region MemberListBinding

        [Fact(Skip = "Autogenerated tests")]
        public void ListBind_Test() =>
            BuildAssert(
                ListBind(memberInfo, elementInitArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void ListBind_1_Test() =>
            BuildAssert(
                ListBind(methodInfo, elementInitArray),
                "",
                ""
            );

        #endregion

        #region MemberMemberBinding

        [Fact(Skip = "Autogenerated tests")]
        public void MemberBind_Test() =>
            BuildAssert(
                MemberBind(memberInfo, memberBindingArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MemberBind_1_Test() =>
            BuildAssert(
                MemberBind(methodInfo, memberBindingArray),
                "",
                ""
            );

        #endregion

        #region MethodCallExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Call_10_Test() =>
            BuildAssert(
                Call(expression, @string, typeArray, expressionArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Call_11_Test() =>
            BuildAssert(
                Call(type, @string, typeArray, expressionArray),
                "",
                ""
            );

        #endregion

        #region NewExpression

        [Fact(Skip = "Autogenerated tests")]
        public void New_Test() =>
            BuildAssert(
                New(constructorInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void New_3_Test() =>
            BuildAssert(
                New(type),
                "",
                ""
            );

        #endregion

        #region ParameterExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Parameter_Test() =>
            BuildAssert(
                Parameter(type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Variable_Test() =>
            BuildAssert(
                Variable(type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Variable_1_Test() =>
            BuildAssert(
                Variable(type, @string),
                "",
                ""
            );

        #endregion

        #region RuntimeVariablesExpression

        [Fact(Skip = "Autogenerated tests")]
        public void RuntimeVariables_Test() =>
            BuildAssert(
                RuntimeVariables(parameterExpressionArray),
                "",
                ""
            );

        #endregion

        #region SwitchCase

        [Fact(Skip = "Autogenerated tests")]
        public void SwitchCase_Test() =>
            BuildAssert(
                SwitchCase(expression, expressionArray),
                "",
                ""
            );

        #endregion

        #region SwitchExpression

        [Fact(Skip = "Autogenerated tests")]
        public void Switch_Test() =>
            BuildAssert(
                Switch(expression, switchCaseArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Switch_1_Test() =>
            BuildAssert(
                Switch(expression, expression, switchCaseArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Switch_2_Test() =>
            BuildAssert(
                Switch(expression, expression, methodInfo, switchCaseArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Switch_3_Test() =>
            BuildAssert(
                Switch(type, expression, expression, methodInfo, switchCaseArray),
                "",
                ""
            );

        #endregion

        #region SymbolDocumentInfo

        [Fact(Skip = "Autogenerated tests")]
        public void SymbolDocument_Test() =>
            BuildAssert(
                SymbolDocument(@string),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void SymbolDocument_1_Test() =>
            BuildAssert(
                SymbolDocument(@string, guid),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void SymbolDocument_2_Test() =>
            BuildAssert(
                SymbolDocument(@string, guid, guid),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void SymbolDocument_3_Test() =>
            BuildAssert(
                SymbolDocument(@string, guid, guid, guid),
                "",
                ""
            );

        #endregion

        #region TryExpression

        [Fact(Skip = "Autogenerated tests")]
        public void MakeTry_Test() =>
            BuildAssert(
                MakeTry(type, expression, expression, expression, iEnumerableOfCatchBlock),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void TryCatch_Test() =>
            BuildAssert(
                TryCatch(expression, catchBlockArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void TryCatchFinally_Test() =>
            BuildAssert(
                TryCatchFinally(expression, expression, catchBlockArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void TryFault_Test() =>
            BuildAssert(
                TryFault(expression, expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void TryFinally_Test() =>
            BuildAssert(
                TryFinally(expression, expression),
                "",
                ""
            );

        #endregion

        #region Type

        [Fact(Skip = "Autogenerated tests")]
        public void GetActionType_Test() =>
            BuildAssert(
                GetActionType(typeArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void GetDelegateType_Test() =>
            BuildAssert(
                GetDelegateType(typeArray),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void GetFuncType_Test() =>
            BuildAssert(
                GetFuncType(typeArray),
                "",
                ""
            );

        #endregion

        #region UnaryExpression

        [Fact(Skip = "Autogenerated tests")]
        public void ConvertChecked_Test() =>
            BuildAssert(
                ConvertChecked(expression, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void ConvertChecked_1_Test() =>
            BuildAssert(
                ConvertChecked(expression, type, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Decrement_Test() =>
            BuildAssert(
                Decrement(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Decrement_1_Test() =>
            BuildAssert(
                Decrement(expression, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Increment_Test() =>
            BuildAssert(
                Increment(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Increment_1_Test() =>
            BuildAssert(
                Increment(expression, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void IsFalse_Test() =>
            BuildAssert(
                IsFalse(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void IsFalse_1_Test() =>
            BuildAssert(
                IsFalse(expression, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void IsTrue_Test() =>
            BuildAssert(
                IsTrue(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void IsTrue_1_Test() =>
            BuildAssert(
                IsTrue(expression, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeUnary_Test() =>
            BuildAssert(
                MakeUnary(expressionType, expression, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void MakeUnary_1_Test() =>
            BuildAssert(
                MakeUnary(expressionType, expression, type, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void NegateChecked_Test() =>
            BuildAssert(
                NegateChecked(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void NegateChecked_1_Test() =>
            BuildAssert(
                NegateChecked(expression, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void OnesComplement_Test() =>
            BuildAssert(
                OnesComplement(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void OnesComplement_1_Test() =>
            BuildAssert(
                OnesComplement(expression, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Quote_Test() =>
            BuildAssert(
                Quote(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Rethrow_Test() =>
            BuildAssert(
                Rethrow(),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Rethrow_1_Test() =>
            BuildAssert(
                Rethrow(type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Throw_Test() =>
            BuildAssert(
                Throw(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Throw_1_Test() =>
            BuildAssert(
                Throw(expression, type),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void UnaryPlus_Test() =>
            BuildAssert(
                UnaryPlus(expression),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void UnaryPlus_1_Test() =>
            BuildAssert(
                UnaryPlus(expression, methodInfo),
                "",
                ""
            );

        [Fact(Skip = "Autogenerated tests")]
        public void Unbox_Test() =>
            BuildAssert(
                Unbox(expression, type),
                "",
                ""
            );

        #endregion

    }
}
