using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionTreeToString;
using ZSpitz.Util;

namespace _tests {
    class Program {
        static void Main() {
            Expression<Func<Person, bool>> expr = p => p.DOB.DayOfWeek == DayOfWeek.Tuesday;

            //Console.WriteLine(expr.ToString("C#"));


            //Console.WriteLine(expr.ToString("Factory methods"));


            //Expression<Func<string, bool>> equal = s => s == "test";
            //LambdaExpression lambda = Expression.Lambda(equal.Body, Expression.Parameter(typeof(string), "s"));
            //Console.WriteLine(equal.ToString("Factory methods"));

            Console.WriteLine(expr.ToString("Textual tree", Language.CSharp));


            string s = expr.ToString("C#", out Dictionary<string, (int start, int length)> pathSpans);
            const int firstColumnAlignment = -45;

            Console.WriteLine($"{"Path",firstColumnAlignment}Substring");
            Console.WriteLine(new string('-', 95));

            foreach (var kvp in pathSpans) {
                var path = kvp.Key;
                var (start, length) = kvp.Value;
                Console.WriteLine(
                    $"{path,firstColumnAlignment}{new string(' ', start)}{s.Substring(start, length)}"
                );
            }

            expr = p => p.LastName.StartsWith("A");
            Console.WriteLine(expr.ToString("Factory methods", "Visual Basic"));

            //var b = true;
            //Expression<Func<bool>> expr = () => b;
            //Console.WriteLine(expr.ToString("Object notation", "Visual Basic"));

            //Expression<Func<bool>> expr = () => DateTime.Now.DayOfWeek == DayOfWeek.Monday;
            //Console.WriteLine(expr.ToString("C#"));

            Expression<Func<int, int, string>> expr1 = (i, j) => (i + j + 5).ToString();
            Console.WriteLine(expr1.ToString("C#"));
            Console.WriteLine(expr1.ToString("Textual tree", "C#"));
        }
    }

    class Person {
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public DateTime DOB { get; set; }
    }
}
