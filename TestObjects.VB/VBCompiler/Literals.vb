Partial Module VBCompiler
    <TestObject(Literal)>
    Friend [True] As Expression = Expr(Function() True)

    <TestObject(Literal)>
    Friend [False] As Expression = Expr(Function() False)

    <TestObject(Literal)>
    Friend NothingString As Expression = Expr(Function() CType(Nothing, String))

    <TestObject(Literal)>
    Friend [Nothing] As Expression = Expr(Function() Nothing)

    <TestObject(Literal)>
    Friend [Integer] As Expression = Expr(Function() 5)

    <TestObject(Literal)>
    Friend NonInteger As Expression = Expr(Function() 7.32)

    <TestObject(Literal)>
    Friend [String] As Expression = Expr(Function() "abcd")

    <TestObject(Literal)>
    Friend EscapedString As Expression = Expr(Function() """")

    <TestObject(Literal)>
    Friend ConstantNothingToObject As Expression = Expr(Function() Nothing)

    <TestObject(Literal)>
    Friend ConstantNothingToReferenceType As Expression = Expr(Of String)(Function() Nothing)

    <TestObject(Literal)>
    Friend ConstantNothingToValueType As Expression = Expr(Of Integer)(Function() Nothing)

    <TestObject(Literal)>
    Friend InterpolatedString As Expression = Expr(Function() $"{#2001-3-25#:yyyy-MM-dd}")
End Module