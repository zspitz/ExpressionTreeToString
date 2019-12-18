Partial Module VBCompiler

    <TestObject(Member)>
    Friend InstanceMember As Expression = IIFE(Function()
                                                   Dim s = ""
                                                   Return Expr(Function() s.Length)
                                               End Function)

    <TestObject(Member)>
    Friend ClosedVariable As Expression = IIFE(Function()
                                                   Dim s = ""
                                                   Return Expr(Function() s)
                                               End Function)

    <TestObject(Member)>
    Friend StaticMember As Expression = Expr(Function() String.Empty)
End Module