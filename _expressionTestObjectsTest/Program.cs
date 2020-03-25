using System;
using System.Linq;
using System.Linq.Expressions;
using ExpressionTreeTestObjects;
using ExpressionTreeTestObjects.VB;
using ExpressionTreeToString;
using static System.Linq.Expressions.Expression;

namespace _expressionTestObjectsTest {
    class Program {
        static void Main(string[] args) {
            Loader.Load();

            var lst = Objects.Get();

            //foreach (var category in lst.Select(x => x.category).Distinct().OrderBy(x => x)) {
            //    Console.WriteLine($"* `{category}`");
            //};
            Console.WriteLine(lst.Length);

            Expression<Func<int[], int[], int>> expr = (c, d) => c[0] + d[0] + c[1] + d[1] + c[2] + d[2];

            Console.WriteLine(expr.ToString("Factory methods"));

            var a = Parameter(typeof(int[]));
            var b = Parameter(typeof(int[]));
            var expr1 = Lambda(
                Add(
                    Add(
                        Add(
                            Add(
                                Add(
                                    ArrayIndex(a, Constant(0)),
                                    ArrayIndex(b, Constant(0))
                                ),
                                ArrayIndex(a,Constant(1))
                            ),
                            ArrayIndex(b, Constant(1))
                        ),
                        ArrayIndex(a, Constant(2))
                    ),
                    ArrayIndex(b, Constant(2))
                ),
                a,
                b
            );
            var fn = expr1.Compile();
            fn.DynamicInvoke(new[] { 1, 2, 3 }, new[] { 4, 5, 6 });

            Console.WriteLine("Hello World!");
        }
    }
}
