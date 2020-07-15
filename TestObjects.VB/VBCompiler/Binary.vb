Partial Module VBCompiler
    <TestObject(Binary)>
    Friend Add As Expression = IIFE(Function()
                                        Dim x As Double = 0, y As Double = 0
                                        Return Expr(Function() x + y)
                                    End Function)

    <TestObject(Binary)>
    Friend Divide As Expression = IIFE(Function()
                                           Dim x As Double = 0, y As Double = 0
                                           Return Expr(Function() x / y)
                                       End Function)

    <TestObject(Binary)>
    Friend Modulo As Expression = IIFE(Function()
                                           Dim x As Double = 0, y As Double = 0
                                           Return Expr(Function() x Mod y)
                                       End Function)

    <TestObject(Binary)>
    Friend Multiply As Expression = IIFE(Function()
                                             Dim x As Double = 0, y As Double = 0
                                             Return Expr(Function() x * y)
                                         End Function)

    <TestObject(Binary)>
    Friend Subtract As Expression = IIFE(Function()
                                             Dim x As Double = 0, y As Double = 0
                                             Return Expr(Function() x - y)
                                         End Function)

    <TestObject(Binary)>
    Friend AndBitwise As Expression = IIFE(Function()
                                               Dim i As Integer = 0, j As Integer = 0
                                               Return Expr(Function() i And j)
                                           End Function)

    <TestObject(Binary)>
    Friend OrBitwise As Expression = IIFE(Function()
                                              Dim i As Integer = 0, j As Integer = 0
                                              Return Expr(Function() i Or j)
                                          End Function)

    <TestObject(Binary)>
    Friend ExclusiveOrBitwise As Expression = IIFE(Function()
                                                       Dim i As Integer = 0, j As Integer = 0
                                                       Return Expr(Function() i Xor j)
                                                   End Function)

    <TestObject(Binary)>
    Friend AndLogical As Expression = IIFE(Function()
                                               Dim b1 As Boolean = True, b2 As Boolean = True
                                               Return Expr(Function() b1 And b2)
                                           End Function)

    <TestObject(Binary)>
    Friend OrLogical As Expression = IIFE(Function()
                                              Dim b1 As Boolean = True, b2 As Boolean = True
                                              Return Expr(Function() b1 Or b2)
                                          End Function)

    <TestObject(Binary)>
    Friend ExclusiveOrLogical As Expression = IIFE(Function()
                                                       Dim b1 As Boolean = True, b2 As Boolean = True
                                                       Return Expr(Function() b1 Xor b2)
                                                   End Function)

    <TestObject(Binary)>
    Friend [AndAlso] As Expression = IIFE(Function()
                                              Dim b1 As Boolean = True, b2 As Boolean = True
                                              Return Expr(Function() b1 AndAlso b2)
                                          End Function)

    <TestObject(Binary)>
    Friend [OrElse] As Expression = IIFE(Function()
                                             Dim b1 As Boolean = True, b2 As Boolean = True
                                             Return Expr(Function() b1 OrElse b2)
                                         End Function)

    <TestObject(Binary)>
    Friend Equal As Expression = IIFE(Function()
                                          Dim i As Integer = 0, j As Integer = 0
                                          Return Expr(Function() i = j)
                                      End Function)

    <TestObject(Binary)>
    Friend NotEqual As Expression = IIFE(Function()
                                             Dim i As Integer = 0, j As Integer = 0
                                             Return Expr(Function() i <> j)
                                         End Function)

    <TestObject(Binary)>
    Friend GreaterThanOrEqual As Expression = IIFE(Function()
                                                       Dim i As Integer = 0, j As Integer = 0
                                                       Return Expr(Function() i >= j)
                                                   End Function)

    <TestObject(Binary)>
    Friend GreaterThan As Expression = IIFE(Function()
                                                Dim i As Integer = 0, j As Integer = 0
                                                Return Expr(Function() i > j)
                                            End Function)

    <TestObject(Binary)>
    Friend LessThan As Expression = IIFE(Function()
                                             Dim i As Integer = 0, j As Integer = 0
                                             Return Expr(Function() i < j)
                                         End Function)

    <TestObject(Binary)>
    Friend LessThanOrEqual As Expression = IIFE(Function()
                                                    Dim i As Integer = 0, j As Integer = 0
                                                    Return Expr(Function() i <= j)
                                                End Function)

    <TestObject(Binary)>
    Friend Coalesce As Expression = IIFE(Function()
                                             Dim s1 As String = Nothing, s2 As String = Nothing
                                             Return Expr(Function() If(s1, s2))
                                         End Function)

    ' The VB compiler has special behavior around shift operations - https://docs.microsoft.com/dotnet/visual-basic/language-reference/operators/left-shift-operator#remarks
    <TestObject(Binary)>
    Friend LeftShift As Expression = IIFE(Function()
                                              Dim i As Integer = 0, j As Integer = 0
                                              Return Expr(Function() i << j)
                                          End Function)

    <TestObject(Binary)>
    Friend RightShift As Expression = IIFE(Function()
                                               Dim i As Integer = 0, j As Integer = 0
                                               Return Expr(Function() i >> j)
                                           End Function)

    <TestObject(Binary)>
    Friend ArrayIndex As Expression = IIFE(Function()
                                               Dim arr = New String() {17}
                                               Return Expr(Function() arr(0))
                                           End Function)

    <TestObject(Binary)>
    Friend Power As Expression = IIFE(Function()
                                          Dim x As Double = 0, y As Double = 0
                                          Return Expr(Function() x ^ y)
                                      End Function)
End Module