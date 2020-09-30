using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

            Expression<Func<bool>> expr = () => true;
            var s = expr.ToString("DebugView");
            var s1 = debugView.GetValue(expr) as string;
            Console.WriteLine(s);
            Console.WriteLine(s1);
            Console.WriteLine(s == s1);

            Expression<Func<int, bool>> expr1 = i => true;
            s = expr1.ToString("DebugView");
            s1 = debugView.GetValue(expr1) as string;
            Console.WriteLine(s);
            Console.WriteLine(s1);
            Console.WriteLine(s == s1);
        }

        static PropertyInfo debugView = typeof(Expression).GetProperty("DebugView", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    class Person {
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public DateTime DOB { get; set; }
    }
}
