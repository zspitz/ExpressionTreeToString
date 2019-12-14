# Expression Trees To Strings

[![NuGet Status](https://img.shields.io/nuget/v/ExpressionTreeToString.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/ExpressionTreeToString/) 

This project provides a `ToString` extension method which returns a string representation of an expression tree (an object inheriting from `System.Linq.Expressions.Expression`, or from another type in `System.Linq.Expressions`).

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

* Type names are rendered using language keywords, instead of just the type name; e.g. `List<string>` or `List(Of Date)` instead of ``List`1``

* Special handling of calls to `String.Concat` and `String.Format`

    ```csharp
    var name = "World";
    Expression<Func<string>> expr = () => string.Format("Hello, {0}!", name);
    Console.WriteLine(expr.ToString("C#"));
    // prints: () => $"Hello, {name}!"
    ```

* Supports the full range of types in `System.Linq.Expressions`, including .NET 4 expression types, and `DynamicExpression`
  
## Feedback

* Star the project
* File an [issue](https://github.com/zspitz/ExpressionToString/issues)
