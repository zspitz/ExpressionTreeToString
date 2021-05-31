using System;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Categories;
using static ExpressionTreeTestObjects.Functions;

namespace ExpressionTreeTestObjects {
    partial class CSCompiler {
        [TestObject(Lambdas)]
        internal static readonly Expression NoParametersVoidReturn = Expr(() => Console.WriteLine());
        
        [TestObject(Lambdas)]
        internal static readonly Expression OneParameterVoidReturn = Expr((string s) => Console.WriteLine(s));
        
        [TestObject(Lambdas)]
        internal static readonly Expression TwoParametersVoidReturn = Expr((string s1, string s2) => Console.WriteLine(s1 + s2));

        [TestObject(Lambdas)]
        internal static readonly Expression NoParametersNonVoidReturn = Expr(() => "abcd");

        [TestObject(Lambdas)]
        internal static readonly Expression OneParameterNonVoidReturn = Expr((string s) => s);

        [TestObject(Lambdas)]
        internal static readonly Expression TwoParametersNonVoidReturn = Expr((string s1, string s2) => s1 + s2);

        [TestObject(Lambdas)]
        internal static readonly Expression NestedLambda = Expr<Func<string>>(() => () => "");

        [TestObject(Lambdas)]
        internal static readonly Expression NestedLambdaExpression = Expr<Expression<Func<string>>>(() => () => "");

        [TestObject(Lambdas)]
        internal static readonly Expression TripleNestedLambda = Expr<Func<Func<string>>>(() => () => () => "");

        [TestObject(Lambdas)]
        internal static readonly Expression TripleNestedLambdaExpression = Expr<Expression<Func<Expression<Func<string>>>>>(() => () => () => "");
    }
}
