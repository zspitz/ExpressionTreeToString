Partial Module VBCompiler
    <TestObject(Unary)>
    Friend ArrayLength As Expression = IIFE(Function()
                                                Dim arr = New String() {}
                                                Return Expr(Function() arr.Length)
                                            End Function)

    <TestObject(Unary)>
    Friend Convert As Expression = IIFE(Function()
                                            Dim o As Object = New Random
                                            Return Expr(Function() CType(o, Random))
                                        End Function)

    <TestObject(Unary)>
    Friend CObject As Expression = IIFE(Function()
                                            Dim lst = New List(Of String)()
                                            Return Expr(Function() CObj(lst))
                                        End Function)

    <TestObject(Unary)>
    Friend Negate As Expression = IIFE(Function()
                                           Dim i = 1
                                           Return Expr(Function() -i)
                                       End Function)

    <TestObject(Unary)>
    Friend BitwiseNot As Expression = IIFE(Function()
                                               Dim i = 1
                                               Return Expr(Function() Not i)
                                           End Function)

    <TestObject(Unary)>
    Friend LogicalNot As Expression = IIFE(Function()
                                               Dim b = True
                                               Return Expr(Function() Not b)
                                           End Function)

    <TestObject(Unary)>
    Friend TypeAs As Expression = IIFE(Function()
                                           Dim o As Object = Nothing
                                           Return Expr(Function() TryCast(o, String))
                                       End Function)
End Module