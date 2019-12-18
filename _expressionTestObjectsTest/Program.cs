using System;
using ExpressionTreeTestObjects;
using ExpressionTreeTestObjects.VB;
using ExpressionTreeToString;
using static System.Linq.Expressions.Expression;

namespace _expressionTestObjectsTest {
    class Program {
        static void Main(string[] args) {
            Loader.Load();

            var lst = Objects.Get();

            Console.WriteLine("Hello World!");
        }
    }
}
