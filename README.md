# Expression Tree To String

[![AppVeyor build status](https://img.shields.io/appveyor/ci/zspitz/expressiontreetostring?style=flat&max-age=86400)](https://ci.appveyor.com/project/zspitz/expressiontreetostring) [![Tests](https://img.shields.io/appveyor/tests/zspitz/expressiontreetostring?compact_message&style=flat&max-age=86400)](https://ci.appveyor.com/project/zspitz/expressiontreetostring) [![NuGet Status](https://img.shields.io/nuget/v/ExpressionTreeToString.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/ExpressionTreeToString/) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [**Test objects:**](https://github.com/zspitz/ExpressionTreeToString/wiki/ExpressionTreeTestObjects) [![NuGet TestObjects Status](https://img.shields.io/nuget/v/ExpressionTreeTestObjects.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/ExpressionTreeTestObjects/) 

![Targets .NET Standard 2.0 and above / Framework 4.5.2 and above](https://img.shields.io/badge/targets%20.NET-Standard%202.0%20%7C%20Framework%204.5.2-informational "Targets .NET Standard 2.0 | Framework 4.5.2") 

Provides a `ToString` extension method which returns a string representation of an expression tree (an object inheriting from `System.Linq.Expressions.Expression`).

```csharp
Expression<Func<bool>> expr = () => true;

Console.WriteLine(expr.ToString("C#"));
// prints: () => true

Console.WriteLine(expr.ToString("Visual Basic"));
// prints: Function() True

Console.WriteLine(expr.ToString("Factory methods", "C#"));
// prints:
/*
    // using static System.Linq.Expressions.Expression

    Lambda(
        Constant(true)
    )
*/

Console.WriteLine(expr.ToString("Object notation", "C#"));
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

Console.WriteLine(expr.ToString("Object notation", "Visual Basic"));
// prints:
/*
    New Expression(Of Func(Of Boolean)) With {
        .NodeType = ExpressionType.Lambda,
        .Type = GetType(Func(Of Boolean)),
        .Body = New ConstantExpression With {
            .Type = GetType(Boolean),
            .Value = True
        },
        .ReturnType = GetType(Boolean)
    }
*/

Console.WriteLine(expr.ToString("Textual tree"));
// prints:
/*
    Lambda (Func<bool>)
        Body - Constant (bool) = True
*/

var b = true;
Expression<Func<int>> expr1 = () => b ? 1 : 0;
Console.WriteLine(expr1.ToString("ToString"));
// prints:
/*
    () => IIF(value(_tests.Program+<>c__DisplayClass0_0).b, 1, 0)
*/

Console.WriteLine(expr1.ToString("DebugView"));
// prints:
/*
    .Lambda #Lambda1<System.Func`1[System.Int32]>() {
        .If (
            .Constant<_tests.Program+<>c__DisplayClass0_0>(_tests.Program+<>c__DisplayClass0_0).b
        ) {
            1
        } .Else {
            0
        }
    }
*/

Expression<Func<Person, bool>> filter = p => p => p.LastName == "A" || p.FirstName == "B" || p.DOB == DateTime.MinValue || p.LastName == "C" || p.FirstName == "D";
Console.WriteLine(filter.ToString("Dynamic LINQ"));
// prints:
/*
    LastName in ("A", "C") || DOB == DateTime.MinValue || FirstName in ("B", "D")
*/
```

Features:

* Multiple writers:

  * Pseudo-code in C# or VB.NET
  * [Factory method](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression) calls which generate this expression
  * Object notation, using object initializer and collection initializer syntax to describe objects
  * Textual tree, focusing on the properties related to the structure of the tree
  * [ToString](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression.tostring) and [DebugView](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/debugview-syntax) reimplementation
  * [Dynamic LINQ](https://dynamic-linq.net/overview) equivalent to the expression

* For C# and VB pseudo-code representations:

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
  * Calls to `String.Concat` and `String.Format` are mapped to the `+` operator and string interpolation, respectively (where possible):

      ```csharp
      var name = "World";
      Expression<Func<string>> expr = () => string.Format("Hello, {0}!", name);
      Console.WriteLine(expr.ToString("C#"));
      // prints: () => $"Hello, {name}!"
      ```

  * Unnecessary conversions are not rendered:
  
      ```csharp
      Expression<Func<IEnumerable<char>>> expr = () => (IEnumerable<char>)"abcd";
      Console.WriteLine(expr.ToString("C#"));
      // prints: () => "abcd"
      ```
  
  * Comparisons against an enum are rendered properly, not as comparison to `int`-converted value:
  
      ```csharp
      var dow = DayOfWeek.Sunday;
      Expression<Func<bool>> expr = () => DateTime.Today.DayOfWeek == dow;
      
      Console.WriteLine(expr.ToString("Textual tree", "C#"));
      // prints:
      /*
        Lambda (Func<bool>)
            · Body - Equal (bool) = false
                · Left - Convert (int) = 3
                    · Operand - MemberAccess (DayOfWeek) DayOfWeek = DayOfWeek.Wednesday
                        · Expression - MemberAccess (DateTime) DateTime.Today = 30/09/2020 12:00:00 am
                · Right - Convert (int) = 0
                    · Operand - MemberAccess (DayOfWeek) dow = DayOfWeek.Sunday
                        · Expression - Constant (<closure>) = #<closure>      
      */
      
      Console.WriteLine(expr.ToString("C#"));
      // prints: () => DateTime.Today.DayOfWeek == dow
      ```

* Each representation (including the ToString and DebugView renderers) can return the start and length of the substring corresponding to any of the paths of the tree's nodes, which can be used to find the substring corresponding to a given node in the tree:

  ```csharp
  var s = expr.ToString("C#", out var pathSpans);
  Console.WriteLine(s);
  // prints: (Person p) => p.DOB.DayOfWeek == DayOfWeek.Tuesday
  
  (int start, int length) = pathSpans["Body.Left.Operand"];
  Console.WriteLine(s.Substring(start, length));
  // prints: p.DOB.DayOfWeek
  ```

* Type names are rendered using language syntax and keywords, instead of the [**Type.Name**](https://docs.microsoft.com/en-us/dotnet/api/system.type.name) property; e.g. `List<string>` or `List(Of Date)` instead of ``List`1``

* Supports the full range of types in `System.Linq.Expressions`, including .NET 4 expression types, and `DynamicExpression`

  * [Expression](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression) (and derived types)
  * [ElementInit](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.elementinit)
  * [MemberBinding](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.memberbinding) (and derived types)
  * [SwitchCase](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.switchcase)
  * [CatchBlock](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.catchblock)
  * [LabelTarget](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.labeltarget)
  
* Extensibility -- allows creating custom renderers, or inheriting from existing renderers, to handle your own Expression-derived types
  
For more information, see the [wiki](https://github.com/zspitz/ExpressionTreeToString/wiki).

## Feedback

* Star the project
* File an [issue](https://github.com/zspitz/ExpressionTreeToString/issues)
