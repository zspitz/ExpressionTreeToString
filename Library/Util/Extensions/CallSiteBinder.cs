using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using static ExpressionTreeToString.Util.BinderTypes;

namespace ExpressionTreeToString.Util {
    public enum BinderTypes {
        Unknown = 0,
        BinaryOperation,
        Convert,
        CreateInstance,
        DeleteIndex,
        DeleteMember,
        GetIndex,
        GetMember,
        Invoke,
        InvokeMember,
        SetIndex,
        SetMember,
        UnaryOperation,
        Dynamic
    }

    public static class CallSiteBinderExtensions {
        public static BinderTypes BinderType(this CallSiteBinder callSiteBinder) =>
            callSiteBinder switch {
                BinaryOperationBinder _ => BinaryOperation,
                ConvertBinder _ => BinderTypes.Convert,
                CreateInstanceBinder _ => CreateInstance,
                DeleteIndexBinder _ => DeleteIndex,
                DeleteMemberBinder _ => DeleteMember,
                GetIndexBinder _ => GetIndex,
                GetMemberBinder _ => GetMember,
                InvokeBinder _ => Invoke,
                InvokeMemberBinder _ => InvokeMember,
                SetIndexBinder _ => SetIndex,
                SetMemberBinder _ => SetMember,
                UnaryOperationBinder _ => UnaryOperation,
                DynamicMetaObjectBinder _ => Dynamic,
                _ => Unknown,
            };
    }
}
