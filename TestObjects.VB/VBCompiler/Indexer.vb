Partial Module VBCompiler
    Private Class DummyWithDefault
        Dim data As Integer
        Default Property Item(index As Integer) As Integer
            Get
                Return data
            End Get
            Set(value As Integer)
                data = index
            End Set
        End Property
    End Class

    <TestObject(Indexer)>
    Friend ArraySingleIndex As Expression = IIFE(Function()
                                                     Dim arr = New String() {}
                                                     Return Expr(Function() arr(5))
                                                 End Function)

    <TestObject(Indexer)>
    Friend ArrayMultipleIndex As Expression = IIFE(Function()
                                                       Dim arr = New String(,) {}
                                                       Return Expr(Function() arr(5, 6))
                                                   End Function)

    <TestObject(Indexer)>
    Friend TypeIndexer As Expression = IIFE(Function()
                                                Dim lst = New List(Of String)()
                                                Return Expr(Function() lst(3))
                                            End Function)

    <TestObject(Indexer)>
    Friend VBDeclaredTypeIndexer As Expression = IIFE(Function()
                                                          Dim x As New DummyWithDefault
                                                          Return Expr(Function() x(5))
                                                      End Function)
End Module