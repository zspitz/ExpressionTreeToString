Partial Module VBCompiler
    <TestObject(NewObject)>
    Friend NamedType As Expression = Expr(Function() New Random())

    <TestObject(NewObject)>
    Friend NamedTypeWithInitializer As Expression = Expr(Function() New Foo With {.Bar = "abcd"})

    <TestObject(NewObject)>
    Friend NamedTypeWithInitializers As Expression = Expr(Function() New Foo With {.Bar = "abcd", .Baz = "efgh"})

    <TestObject(NewObject)>
    Friend NamedTypeConstructorParameters As Expression = Expr(Function() New Foo("ijkl"))

    <TestObject(NewObject)>
    Friend NamedTypeConstructorParametersWithInitializers As Expression = Expr(Function() New Foo("ijkl") With {.Bar = "abcd", .Baz = "efgh"})

    <TestObject(NewObject)>
    Friend AnonymousType As Expression = Expr(Function() New With {.Bar = "abcd", .Baz = "efgh"})

    <TestObject(NewObject)>
    Friend AnonymousTypeFromVariables As Expression = IIFE(Function()
                                                               Dim Bar = "abcd"
                                                               Dim Baz = "efgh"
                                                               Return Expr(Function() New With {Bar, Baz})
                                                           End Function)

    <TestObject(NewObject)>
    Friend CollectionTypeWithInitializer As Expression = Expr(Function() New List(Of String) From {"abcd", "efgh"})

    <TestObject(NewObject)>
    Friend CollectionTypeWithMultipleElementsInitializers As Expression = Expr(Function() New Wrapper From {{"ab", "cd"}, {"ef", "gh"}})

    <TestObject(NewObject)>
    Friend CollectionTypeWithSingleOrMultipleElementsInitializers As Expression = Expr(Function() New Wrapper From {{"ab", "cd"}, "ef"})
End Module