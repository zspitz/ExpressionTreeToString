'Imports ExpressionTreeTestObjects.Dummy

'Partial Module VBCompiler
'    <TestObject(RefOut)>
'    Friend PassRef As Expression = Expr(Sub(i As Integer) DummyMethodWithRef(i))

'    <TestObject(RefOut)>
'    Friend PassOut As Expression = Expr(Sub(i As Integer) DummyMethodWithOut(i))

'    <TestObject(RefOut)>
'    Friend PassRefField = Expr(Sub(d As Dummy2) DummyMethodWithRef(d.Data))

'    <TestObject(RefOut)>
'    Friend PassOutField = Expr(Sub(d As Dummy2) DummyMethodWithOut(d.Data))

'    <TestObject(RefOut)>
'    Friend PassRefClosureVariable = IIFE(Function()
'                                             Dim i As Integer = 0
'                                             Return Expr(Sub() DummyMethodWithRef(i))
'                                         End Function)

'    <TestObject(RefOut)>
'    Friend PassOutClosureVariable = IIFE(Function()
'                                             Dim i As Integer = 0
'                                             Return Expr(Sub() DummyMethodWithOut(i))
'                                         End Function)
'End Module