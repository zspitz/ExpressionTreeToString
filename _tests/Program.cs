using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionTreeTestObjects;
using ExpressionTreeTestObjects.VB;
using ExpressionTreeToString;
using ZSpitz.Util;
using static System.Linq.Expressions.Expression;

namespace _tests {
    class Program {
        static void Main() {
            //Expression<Func<Person, bool>> expr = p => p.DOB.DayOfWeek == DayOfWeek.Tuesday;

            //Console.WriteLine(expr.ToString("C#"));


            //Console.WriteLine(expr.ToString("Factory methods"));


            //Expression<Func<string, bool>> equal = s => s == "test";
            //LambdaExpression lambda = Expression.Lambda(equal.Body, Expression.Parameter(typeof(string), "s"));
            //Console.WriteLine(equal.ToString("Factory methods"));

            //Console.WriteLine(expr.ToString("Textual tree", Language.CSharp));

            //Expression<Func<int, int, int>> expr = (i, j) => i * j;

            //string s = expr.ToString("Factory methods", out Dictionary<string, (int start, int length)> pathSpans, "C#");
            //const int firstColumnAlignment = -45;

            //var s = expr.ToString("C#", out var pathSpans);
            //Console.WriteLine(s);
            //(int start, int length) = pathSpans["Body.Left.Operand"];
            //Console.WriteLine(s.Substring(start, length));

            //Console.WriteLine($"{"Path",firstColumnAlignment}Substring");
            //Console.WriteLine(new string('-', 95));

            //foreach (var kvp in pathSpans) {
            //    var path = kvp.Key;
            //    var (start, length) = kvp.Value;
            //    Console.WriteLine(
            //        $"{path,firstColumnAlignment}{new string(' ', start)}{s.Substring(start, length)}"
            //    );
            //}

            //expr = p => p.LastName.StartsWith("A");
            //Console.WriteLine(expr.ToString("Factory methods", "Visual Basic"));

            //var b = true;
            //Expression<Func<bool>> expr = () => b;
            //Console.WriteLine(expr.ToString("Object notation", "Visual Basic"));

            //Expression<Func<bool>> expr = () => DateTime.Now.DayOfWeek == DayOfWeek.Monday;
            //Console.WriteLine(expr.ToString("C#"));

            //Expression<Func<int, int, string>> expr1 = (i, j) => (i + j + 5).ToString();
            //Console.WriteLine(expr1.ToString("C#"));
            //Console.WriteLine(expr1.ToString("Textual tree", "C#"));

            //double d = 5.2;
            //Expression<Func<string>> expr = () => ((int)d).ToString();
            //Console.WriteLine(expr.ToString("C#"));

            //var b = true;
            //Expression<Func<int>> expr = () => b ? 1 : 0;
            //Console.WriteLine(expr.ToString("ToString"));

            //Console.WriteLine(expr.ToString("DebugView"));

            //Console.WriteLine(expr.ToString("ToString"));

            //Console.WriteLine(expr.ToString("Factory methods", "C#"));

            //var dow = DayOfWeek.Sunday;
            //Expression<Func<bool>> expr = () => DateTime.Today.DayOfWeek == dow;
            //Console.WriteLine(expr.ToString("Textual tree", "C#"));
            //Console.WriteLine(expr.ToString("C#"));

            //Expression<Func<IEnumerable<char>>> expr = () => (IEnumerable<char>)"abcd";
            //Console.WriteLine(expr.ToString("C#"));

            //Loader.Load();
            ////Expression x = (Expression)Objects.Get().Where(x => x.o is Expression expr && expr.CanReduce).Select(x => x.o).FirstOrDefault();
            ////Console.WriteLine(x.ToString("Textual tree", "C#"));

            //Objects.Get()
            //    .Select(x => (o: x.o as Expression, x.name))
            //    .Where(x => x.o is Expression expr && expr.CanReduce)
            //    .Select(x => {
            //        (Expression expr, string name) = (x.o!, x.name);
            //        TextualTreeWriterVisitor.ReducePredicate = _ => false;
            //        var unreduced = expr.ToString("Textual tree", "C#");
            //        TextualTreeWriterVisitor.ReducePredicate = null;
            //        var @default = expr.ToString("Textual tree", "C#");
            //        TextualTreeWriterVisitor.ReducePredicate = _ => true;
            //        var allReduced = expr.ToString("Textual tree", "C#");
            //        return (name, unreduced, @default, allReduced);
            //    }).ForEach(x => {
            //        var (name, unreduced, @default, allReduced) = x;
            //        Console.WriteLine($"======== {name} - {(@default != unreduced ? "default " : "")}{(allReduced != @default ? "allReduced" : "")}");
            //        Console.WriteLine(unreduced);
            //        if (@default != unreduced) { Console.WriteLine(@default); }
            //        if (allReduced != @default) { Console.WriteLine(allReduced); }
            //    });

            //bool? b = true;
            //Expression<Func<object>> expr = () => !b;
            //Console.WriteLine(expr.ToString("C#"));

            //Expression<Func<Foo, Foo?>> expr = f => 
            //    f != null && f.A != null && f.A.B != null && f.A.B.C != null ? 
            //        f.A.B.C.D : 
            //        null;
            //Console.WriteLine(expr.ToString("Dynamic LINQ"));

            ////Expression<Func<Person, string>> expr = p => p.TestExtension();

            //availableRenderersSamples();

            //var s = "123";
            //int i = 5;
            //Expression<Func<bool>> expr = () => int.TryParse(s, out i);
            //var s1 = expr.ToString("Factory methods", "C#");

            //var Bar = Parameter(
            //    typeof(string),
            //    "Bar"
            //);
            //var Baz = Parameter(
            //    typeof(string),
            //    "Baz"
            //);



            //var s = "123";
            //var i = Variable(typeof(int), "i");

            //var expr2 = Lambda(
            //    Call(
            //        typeof(int).GetMethod("TryParse", new[] { typeof(string), typeof(int).MakeByRefType() }),
            //        Constant(s),
            //        i
            //    )
            //);

            //int i = 5;
            //Expression<Func<string>> expr = () => i.ToString();
            //Console.WriteLine(expr.ToString("Factory methods", "C#"));

            int i = 5;
            Expression<Func<long>> expr = () => (long)i;
            Console.WriteLine(expr.ToString("Textual tree", "C#"));
            Console.WriteLine(expr.ToString("C#"));
        }

        static PropertyInfo debugView = typeof(Expression).GetProperty("DebugView", BindingFlags.NonPublic | BindingFlags.Instance);

        static void availableRenderersSamples() {
            void line() => Console.WriteLine(new string('=', 50));

            Expression<Func<Person, bool>> expr = p => p.DOB.DayOfWeek == DayOfWeek.Tuesday;
            Console.WriteLine(expr.ToString("C#"));

            line();

            Console.WriteLine(expr.ToString("Visual Basic"));

            line();

            Console.WriteLine(expr.ToString("Factory methods", "C#"));

            line();

            Expression<Func<Person, bool>> expr1 = p => p.DOB.DayOfWeek == DayOfWeek.Tuesday || p.DOB.DayOfWeek == DayOfWeek.Thursday;
            Console.WriteLine(expr1.ToString("Dynamic LINQ"));

            line();

            Expression<Func<Person, bool>> expr2 = p => true;
            Console.WriteLine(expr2.ToString("Textual tree", "C#"));

            line();

            Console.WriteLine(expr2.ToString("Object notation", "C#"));

            line();

            Console.WriteLine(expr2.ToString("ToString"));

            line();

            Console.WriteLine(expr2.ToString("DebugView"));
        }
    }

    public class Person {
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string Name => LastName + ", " + FirstName;
        public DateTime DOB { get; set; }
        public Post[] Posts { get; set; } = new Post[] { };
    }

    public class Post {
        public string Text { get; set; } = "";
        public string[] Tags { get; set; }
    }

    public static class Extension {
        public static string TestExtension(this Person p) => "";
    }

    class Foo {
        public Foo? A { get; set; }
        public Foo? B { get; set; }
        public Foo? C { get; set; }
        public Foo? D { get; set; }
    }
}
