using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Functions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeTestObjects {
    [ObjectContainer]
    static internal partial class CSCompiler {
        [TestObject(Defaults)]
        internal static readonly Expression DefaultRefType = Expr(() => default(string));

        [TestObject(Defaults)]
        internal static readonly Expression DefaultValueType = Expr(() => default(int));

        [TestObject(Conditionals)]
        internal static readonly Expression Conditional = Expr((int i) => i > 10 ? i : i + 10);

        [TestObject(TypeChecks)]
        internal static readonly Expression TypeCheck = IIFE(() => {
            object o = "";
            return Expr(() => o is string);
        });

        [TestObject(Invocation)]
        internal static readonly Expression InvocationNoArguments = IIFE(() => {
            Func<int> del = () => new DateTime(2001,1,1).Day;
            return Expr(() => del());
        });

        [TestObject(Invocation)]
        internal static readonly Expression InvocationOneArgument = IIFE(() => {
            Func<int, int> del = (int i) => new DateTime(2001, 1, 1).Day;
            return Expr(() => del(5));
        });

        [TestObject(Member)]
        internal static readonly Expression InstanceMember = IIFE(() => {
            var s = "";
            return Expr(() => s.Length);
        });

        [TestObject(Member)]
        internal static readonly Expression ClosedVariable = IIFE(() => {
            var s = "";
            return Expr(() => s);
        });

        [TestObject(Member)]
        internal static readonly Expression StaticMember = Expr(() => string.Empty);

        [TestObject(Indexer)]
        internal static readonly Expression ArraySingleIndex = IIFE(() => {
            var arr = new string[] { "0", "1", "2", "3", "4", "5" };
            return Expr(() => arr[5]);
        });

        [TestObject(Indexer)]
        internal static readonly Expression ArrayMultipleIndex = IIFE(() => {
            var arr = new string[,] {
                {"0","1","2","3","4","5" },
                {"10","11","12","13","14","15" },
                {"20","21","22","23","24","25" },
                {"30","31","32","33","34","35" },
                {"40","41","42","43","44","45" },
                {"50","51","52","53","54","55" },
                {"60","61","62","63","64","65" }
            };
            return Expr(() => arr[5, 6]);
        });

        [TestObject(Indexer)]
        internal static readonly Expression TypeIndexer = IIFE(() => {
            var lst = new List<string>() { "1", "2", "3", "4" };
            return Expr(() => lst[3]);
        });
    }
}
