Partial Module VBCompiler
    <TestObject(Lambdas)>
    Friend NoParametersVoidReturn As Expression = Expr(Sub() Console.WriteLine())

    <TestObject(Lambdas)>
    Friend OneParameterVoidReturn As Expression = Expr(Sub(s As String) Console.WriteLine(s))

    <TestObject(Lambdas)>
    Friend TwoParametersVoidReturn As Expression = Expr(Sub(s1 As String, s2 As String) Console.WriteLine(s1 + s2))

    <TestObject(Lambdas)>
    Friend NoParametersNonVoidReturn As Expression = Expr(Function() "abcd")

    <TestObject(Lambdas)>
    Friend OneParameterNonVoidReturn As Expression = Expr(Function(s As String) s)

    <TestObject(Lambdas)>
    Friend TwoParametersNonVoidReturn As Expression = Expr(Function(s1 As String, s2 As String) s1 + s2)

    <TestObject(Lambdas)>
    Friend NestedLambda As Expression = Expr(Function() Function() "")

    <TestObject(Lambdas)>
    Friend NestedLambdaExpression As Expression = Expr(Of Expression(Of Func(Of String)))(Function() Function() "")

    <TestObject(Lambdas)>
    Friend TripleNestedLambda As Expression = Expr(Function() Function() Function() "")

    <TestObject(Lambdas)>
    Friend TripleNestedLambdaExpression As Expression = Expr(Of Expression(Of Func(Of Expression(Of Func(Of String)))))(Function() Function() Function() "")
End Module
