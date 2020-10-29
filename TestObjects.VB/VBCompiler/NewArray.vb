Partial Module VBCompiler

    <TestObject(NewArray)>
    Friend SingleDimensionInit As Expression = Expr(Function() New String() {""})

    <TestObject(NewArray)>
    Friend SingleDimensionInitExplicitType As Expression = Expr(Function() New Object() {""})

    <TestObject(NewArray)>
    Friend SingleDimensionWithBounds As Expression = Expr(Function() New String(4) {})

    <TestObject(NewArray)>
    Friend SingleDimensionWithBoundsExpression As Expression = IIFE(Function()
                                                                        Dim bounds = 5
                                                                        Return Expr(Function() New String(bounds) {})
                                                                    End Function)

    <TestObject(NewArray)>
    Friend MultidimensionWithBounds As Expression = Expr(Function() New String(1, 2) {})

    <TestObject(NewArray)>
    Friend MultidimensionWithBoundsExpression As Expression = IIFE(Function()
                                                                       Dim bound1 = 1
                                                                       Dim bound2 = 2
                                                                       Return Expr(Function() New String(bound1, bound2) {})
                                                                   End Function)


    <TestObject(NewArray)>
    Friend JaggedWithElementsImplicitType As Expression = Expr(Function() {
            ({"ab", "cd"}),
            ({"ef", "gh"})
        })

    ' for jagged array literals, the inner literals need to be wrapped in parentheses
    ' we're checking that this is only done for literals
    <TestObject(NewArray)>
    Friend JaggedWithElementsImplicitTypeInnerNonLiteral As Expression = IIFE(Function()
                                                                                  Dim arr1 = New String() {"ab", "cd"}
                                                                                  Dim arr2 = New String() {"ef", "gh"}
                                                                                  Return Expr(Function() {
                                                                                                      arr1,
                                                                                                      arr2
                                                                                        })
                                                                              End Function)

    <TestObject(NewArray)>
    Friend JaggedWithElementsExplicitType As Expression = Expr(Function() New Object()() {
            ({"ab", "cd"}),
            ({"ef", "gh"})
        })

    <TestObject(NewArray)>
    Friend JaggedWithBounds As Expression = Expr(Function() New String(4)() {})

    <TestObject(NewArray)>
    Friend JaggedWithBoundsExpression As Expression = IIFE(Function()
                                                               Dim bound = 4
                                                               Return Expr(Function() New String(bound)() {})
                                                           End Function)

    <TestObject(NewArray)>
    Friend ArrayOfMultidimensionalArray As Expression = Expr(Function() New String(4)(,) {})

    <TestObject(NewArray)>
    Friend MultidimensionalArrayOfArray As Expression = Expr(Function() New String(2, 1)() {})
End Module