Partial Module VBCompiler
    <TestObject(Method)>
    Friend InstanceMethod0Arguments As Expression = IIFE(Function()
                                                             Dim s = ""
                                                             Return Expr(Function() s.ToString())
                                                         End Function)

    <TestObject(Method)>
    Friend StaticMethod0Arguments As Expression = Expr(Sub() DummyMethod())

    <TestObject(Method)>
    Friend ExtensionMethod0Arguments As Expression = IIFE(Function()
                                                              Dim lst = New List(Of String)()
                                                              Return Expr(Function() lst.Distinct)
                                                          End Function)

    <TestObject(Method)>
    Friend ExtensionMethod0ArgumentsWithoutConversion As Expression = IIFE(Function()
                                                                               Dim lst As IEnumerable(Of String) = New List(Of String)()
                                                                               Return Expr(Function() lst.Distinct)
                                                                           End Function)

    <TestObject(Method)>
    Friend InstanceMethod1Argument As Expression = IIFE(Function()
                                                            Dim s = ""
                                                            Return Expr(Function() s.CompareTo(""))
                                                        End Function)

    <TestObject(Method)>
    Friend StaticMethod1Argument As Expression = Expr(Function() String.Intern(""))

    <TestObject(Method)>
    Friend ExtensionMethod1Argument As Expression = IIFE(Function()
                                                             Dim lst = New List(Of String)()
                                                             Return Expr(Function() lst.Take(1))
                                                         End Function)

    <TestObject(Method)>
    Friend ExtensionMethod1ArgumentWithoutConversion As Expression = IIFE(Function()
                                                                              Dim lst As IEnumerable(Of String) = New List(Of String)()
                                                                              Return Expr(Function() lst.Take(1))
                                                                          End Function)

    <TestObject(Method)>
    Friend InstanceMethod2Arguments As Expression = IIFE(Function()
                                                             Dim s = ""
                                                             Return Expr(Function() s.IndexOf("a"c, 2))
                                                         End Function)

    <TestObject(Method)>
    Friend StaticMethod2Arguments As Expression = IIFE(Function()
                                                           Dim arr = New Char() {"a"c, "b"c}
                                                           Return Expr(Function() String.Join(","c, arr))
                                                       End Function)

    <TestObject(Method)>
    Friend StaticMethod2ArgumentsWithoutConversion As Expression = IIFE(Function()
                                                                            Dim arr As IEnumerable(Of Char) = New Char() {"a"c, "b"c}
                                                                            Return Expr(Function() String.Join(","c, arr))
                                                                        End Function)

    <TestObject(Method)>
    Friend ExtensionMethod2Arguments As Expression = IIFE(Function()
                                                              Dim lst = New List(Of String)()
                                                              Return Expr(Function() lst.OrderBy(Function(x) x, StringComparer.OrdinalIgnoreCase))
                                                          End Function)

    <TestObject(Method)>
    Friend ExtensionMethod2ArgumentsWithoutConversion As Expression = IIFE(Function()
                                                                               Dim lst As IEnumerable(Of String) = New List(Of String)()
                                                                               Dim comparer As IComparer(Of String) = StringComparer.OrdinalIgnoreCase
                                                                               Return Expr(Function() lst.OrderBy(Function(x) x, comparer))
                                                                           End Function)

    <TestObject(Method)>
    Friend StringConcat As Expression = Expr(Function(s1 As String, s2 As String) String.Concat(s1, s2))

    <TestObject(Method)>
    Friend StringConcatOperator As Expression = Expr(Function(s1 As String, s2 As String) s1 + s2)

    <TestObject(Method)>
    Friend StringConcatOperatorParamArray As Expression = Expr(Function(s1 As String, s2 As String) s1 + s2 + s1 + s2 + s1 + s2)

    'This will not compile -- Like operator is currently not supported in .NET Core
    '<Category(Method)>
    'Friend LikeOperatorStrings As Expression = Expr(Function(s1 As String, s2 As String) s1 Like s2)

    '<Category(Method)>
    'Friend LikeOperatorObjects As Expression = Expr(Function(o1 As Object, o2 As Object) o1 Like o2)
End Module
