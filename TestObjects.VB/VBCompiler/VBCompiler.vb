<ObjectContainer>
Partial Friend Module VBCompiler
    <TestObject(Conditionals)>
    Friend Conditional As Expression = Expr(Function(i As Integer) If(i > 10, i, i + 10))

    <TestObject(TypeChecks)>
    Friend TypeCheck As Expression = Expr(Function() TypeOf "" Is String)

    <TestObject(Invocation)>
    Friend InvocationNoArguments As Expression = IIFE(Function()
                                                          Dim del = Function() New Date(2001, 1, 1).Day
                                                          Return Expr(Function() del())
                                                      End Function)

    <TestObject(Invocation)>
    Friend InvocationOneArgument As Expression = IIFE(Function()
                                                          Dim del = Function(i As Integer) New Date(2001, 1, 1).Day
                                                          Return Expr(Function() del(5))
                                                      End Function)

    <TestObject(EnumComparison)>
    Friend LeftEnumNonConstant As Expression = IIFE(Function()
                                                        Dim dow = DayOfWeek.Wednesday
                                                        Return Expr(Function() dow >= DayOfWeek.Tuesday)
                                                    End Function)

    <TestObject(EnumComparison)>
    Friend RightEnumNonConstant As Expression = IIFE(Function()
                                                         Dim dow = DayOfWeek.Wednesday
                                                         Return Expr(Function() DayOfWeek.Tuesday <= dow)
                                                     End Function)

    <TestObject(EnumComparison)>
    Friend DualNonConstant As Expression = IIFE(Function()
                                                    Dim dow1 = DayOfWeek.Monday
                                                    Dim dow2 = DayOfWeek.Friday
                                                    Return Expr(Function() dow1 = dow2)
                                                End Function)
End Module
