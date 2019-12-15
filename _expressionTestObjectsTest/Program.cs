using System;
using ExpressionTreeTestObjects;
using ExpressionTreeToString;
using static System.Linq.Expressions.Expression;

namespace _expressionTestObjectsTest {
    class Program {
        static void Main(string[] args) {
            //var lst = Objects.Get();

            //Console.WriteLine("Hello World!");

            var expr = Lambda(
                Constant(null, typeof(string))
            );
            Console.WriteLine(expr.ToString("Factory methods"));
        }
    }
}
