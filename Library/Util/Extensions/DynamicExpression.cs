﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Linq.Expressions.ExpressionType;

namespace ExpressionTreeToString.Util {
    internal static class DynamicExpressionExtensions {
        internal static void Deconstruct(this DynamicExpression expr, out ExpressionType nodeType, out Type type, out BinderTypes binderType) {
            (nodeType, type, binderType) = (
                expr.Binder switch
                {
                    BinaryOperationBinder binder => binder.Operation,
                    ConvertBinder _ => ExpressionType.Convert,
                    CreateInstanceBinder _ => New,
                    GetIndexBinder _ => Index,
                    GetMemberBinder _ => MemberAccess,
                    InvokeBinder _ => Invoke,
                    InvokeMemberBinder _ => Call,
                    UnaryOperationBinder binder => binder.Operation,

                    // Some binders have no corresponding node types -- DeleteIndexBinder, DeleteMemberBinder

                    // Other binders mean the node represents more than the simple node type:
                    //  SetIndexBinder and SetMemberBinder are more than just Assign

                    _ => Dynamic
                },
                expr.Type,
                expr.Binder.BinderType1()
            );
        }
    }
}
