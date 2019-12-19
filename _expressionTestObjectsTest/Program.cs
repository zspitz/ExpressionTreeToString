using System;
using System.Linq;
using ExpressionTreeTestObjects;
using ExpressionTreeTestObjects.VB;
using ExpressionTreeToString;
using static System.Linq.Expressions.Expression;

namespace _expressionTestObjectsTest {
    class Program {
        static void Main(string[] args) {
            Loader.Load();

            var lst = Objects.Get();

            foreach (var category in lst.Select(x => x.category).Distinct().OrderBy(x => x)) {
                Console.WriteLine($"* `{category}`");
            };

            Console.WriteLine("Hello World!");
        }
    }
}
