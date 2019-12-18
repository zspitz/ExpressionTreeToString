# Expression Trees To Strings

[![AppVeyor build status](https://img.shields.io/appveyor/ci/zspitz/expressiontreetostring?style=flat&max-age=86400)](https://ci.appveyor.com/project/zspitz/expressiontreetostring) [![Tests](https://img.shields.io/appveyor/tests/zspitz/expressiontreetostring?compact_message&style=flat&max-age=86400)](https://ci.appveyor.com/project/zspitz/expressiontreetostring) [![NuGet Status](https://img.shields.io/nuget/v/ExpressionTreeToString.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/ExpressionTreeToString/) 

**ExpressionTreeTestObjects:** [![NuGet TestObjects Status](https://img.shields.io/nuget/v/ExpressionTreeTestObjects.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/ExpressionTreeTestObjects/) 

Provides a `ToString` extension method which returns a string representation of an expression tree (an object inheriting from `System.Linq.Expressions.Expression`).

```csharp
Expression<Func<bool>> expr = () => true;

Console.WriteLine(expr.ToString("C#"));
// prints: () => true

Console.WriteLine(expr.ToString("Visual Basic"));
// prints: Function() True

Console.WriteLine(expr.ToString("Factory methods"));
// prints:
/*
    // using static System.Linq.Expressions.Expression

    Lambda(
        Constant(true)
    )
*/

Console.WriteLine(expr.ToString("Object notation"));
// prints:
/*
    new Expression<Func<bool>> {
        NodeType = ExpressionType.Lambda,
        Type = typeof(Func<bool>),
        Body = new ConstantExpression {
            Type = typeof(bool),
            Value = true
        },
        ReturnType = typeof(bool)
    }
*/

Console.WriteLine(expr.ToString("Textual tree"));
// prints:
/*
    Lambda (Func<bool>)
        Body - Constant (bool) = True
*/
```

Features:

* Multiple formatters ([with more planned](https://github.com/zspitz/ExpressionTreeToString/issues/38)):

  * Pseudo-code in C# or VB.NET
  * Factory method calls which generate this expression
  * Object notation, using object initializer and collection initializer syntax to describe objects
  * Textual tree, focusing on the properties related to the structure of the tree

* Extension methods are rendered as instance methods

    ```csharp
    Expression<Func<int, int>> expr = x => Enumerable.Range(1, x).Select(y => x * y).Count();
    Console.WriteLine(expr.ToString("C#"));
    // prints: (int x) => Enumerable.Range(1, x).Select((int y) => x * y).Count()
    ```

* Closed-over variables are rendered as simple identifiers (instead of member access on the hidden compiler-generated class)

    ```csharp
    var i = 7;
    var j = 8;
    Expression<Func<int>> expr = () => i + j;
    Console.WriteLine(expr.ToString("C#"));
    // prints: () => i + j
    ```

* Type names are rendered using language syntax and keywords, instead of the [**Type.Name**](https://docs.microsoft.com/en-us/dotnet/api/system.type.name) property; e.g. `List<string>` or `List(Of Date)` instead of ``List`1``

* Calls to `String.Concat` and `String.Format` are mapped to the `+` operator and string interpolation (where possible):

    ```csharp
    var name = "World";
    Expression<Func<string>> expr = () => string.Format("Hello, {0}!", name);
    Console.WriteLine(expr.ToString("C#"));
    // prints: () => $"Hello, {name}!"
    ```

* Supports the full range of types in `System.Linq.Expressions`, including .NET 4 expression types, and `DynamicExpression`

  * [Expression](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression) (and derived types)
  * [ElementInit](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.elementinit)
  * [MemberBinding](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.memberbinding) (and derived types)
  * [SwitchCase](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.switchcase)
  * [CatchBlock](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.catchblock)
  * [LabelTarget](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.labeltarget)
  
For more information, see the [wiki](https://github.com/zspitz/ExpressionToString/wiki/String-rendering-library-overview).

## Feedback

* Star the project
* File an [issue](https://github.com/zspitz/ExpressionToString/issues)
