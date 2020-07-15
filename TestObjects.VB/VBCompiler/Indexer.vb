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
                                                     Dim arr = New String() {"1", "2", "3", "4", "5", "6"}
                                                     Return Expr(Function() arr(5))
                                                 End Function)

    <TestObject(Indexer)>
    Friend ArrayMultipleIndex As Expression = IIFE(Function()
                                                       Dim arr = New String(,) {
                                                            {"1", "2", "3", "4", "5", "6", "7"},
                                                            {"11", "12", "13", "14", "15", "16", "17"},
                                                            {"21", "22", "23", "24", "25", "26", "27"},
                                                            {"31", "32", "33", "34", "35", "36", "37"},
                                                            {"41", "42", "43", "44", "45", "46", "47"},
                                                            {"51", "52", "53", "54", "55", "56", "57"}
                                                       }
                                                       Return Expr(Function() arr(5, 6))
                                                   End Function)

    <TestObject(Indexer)>
    Friend TypeIndexer As Expression = IIFE(Function()
                                                Dim lst = New List(Of String) From {"1", "2", "3", "4"}
                                                Return Expr(Function() lst(3))
                                            End Function)

    <TestObject(Indexer)>
    Friend VBDeclaredTypeIndexer As Expression = IIFE(Function()
                                                          Dim x As New DummyWithDefault
                                                          Return Expr(Function() x(5))
                                                      End Function)
End Module