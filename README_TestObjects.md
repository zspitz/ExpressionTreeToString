[![NuGet TestObjects Status](https://img.shields.io/nuget/v/ExpressionTreeTestObjects.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/ExpressionTreeTestObjects/)

Expression trees (and related objects) can be generated in three ways:

* Calling the factory methods at **System.Linq.Expressions.Expression**
* In C#, assigning a lambda expression to an `Expression<T>` variable, parameter or property; the C# compiler generates an expression tree corresponding to the code of the lambda expression
* The VB compiler also generates expression trees from VB lambda expressions assigned to `Expression<T>`.

As part of the development of [ExpressionTreeToString](https://github.com/zspitz/ExpressionTreeToString), I needed a comprehensive set of expression trees to test the renderers against. This set of objects is available as a [Nuget package](https://www.nuget.org/packages/ExpressionTreeTestObjects/).

Each expression object is a field with the [`TestObject` attribute](https://github.com/zspitz/ExpressionTreeToString/blob/master/TestObjects/TestObjectAttribute.cs) applied to it.

The objects are available by referencing the NuGet package, and calling the static `Get` method:

    // using ExpressionTreeTestObjects;

    (string category, string source, string name, object o)[] lst = Objects.Get();

where `source` is the type where the object is defined -- one of the following:

| Source | Description |
| --- | --- |
| `Factory methods` | Factory methods |
| `CSCompiler` | C# compiler |
| `VBCompiler` | VB compiler |

and `category` is one of [these](https://github.com/zspitz/ExpressionTreeToString/blob/master/TestObjects/Categories.cs) values:

* `Binary`
* `Blocks`
* `Conditionals`
* `Constants`
* `DebugInfos`
* `Defaults`
* `Dynamics`
* `Gotos`
* `Indexer`
* `Invocation`
* `Labels`
* `Lambda`
* `Literal`
* `Loops`
* `Member access (+ closed variables)`
* `Member bindings`
* `Method call`
* `New array`
* `Object creation and initialization`
* `Quoted`
* `Runtime variables`
* `Switch, CatchBlock`
* `Try, Catch, Finally`
* `Type check`
* `Unary`

The static constructor automatically loads types in assemblies that start with `ExpressionTreeTestObjects` -- ExpressionTreeTestObjects.CSharp.dll, and ExpressionTreeTestObjects.VB.dll.


2. Call the static `LoadType` method on the containing type:

        Objects.LoadType(typeof(MyCustomType));

    and it will add the object. The type name becomes the `source`, and the field name becomes the `objectName`.

---

The C# and VB compiler test expressions were added as I was working on the corresponding renderers. For the factory methods, I used reflection to build a list of all the methods and overloads at `System.Linq.Expressions.Expression` and created expressions by calling each one.

The result is pretty comprehensive, with one limitation: I didn't write test expressions for overloads which take an additional `MethodInfo`, like [this one](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression.add?view=netframework-4.8#System_Linq_Expressions_Expression_Add_System_Linq_Expressions_Expression_System_Linq_Expressions_Expression_System_Reflection_MethodInfo_). This is being tracked at [#20](https://github.com/zspitz/ExpressionTreeToString/issues/20).
