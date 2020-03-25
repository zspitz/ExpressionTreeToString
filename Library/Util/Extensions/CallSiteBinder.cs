using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace ExpressionTreeToString.Util {
    public static class CallSiteBinderExtensions {
        public static string BinderType(this CallSiteBinder callSiteBinder) =>
            callSiteBinder switch {
                BinaryOperationBinder _ => "BinaryOperation",
                ConvertBinder _ => "Convert",
                CreateInstanceBinder _ => "CreateInstance",
                DeleteIndexBinder _ => "DeleteIndex",
                DeleteMemberBinder _ => "DeleteMember",
                GetIndexBinder _ => "GetIndex",
                GetMemberBinder _ => "GetMember",
                InvokeBinder _ => "Invoke",
                InvokeMemberBinder _ => "InvokeMember",
                SetIndexBinder _ => "SetIndex",
                SetMemberBinder _ => "SetMember",
                UnaryOperationBinder _ => "UnaryOperation",
                DynamicMetaObjectBinder _ => "Dynamic",
                _ => "(Unknown binder)",
            };
    }
}
