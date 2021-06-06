using System.Linq.Expressions;
using ExpressionTreeTestObjects;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeToString.Tests {
    partial class DynamicLinqTestObjects {
        [TestObject(CharComparison)]
        internal static readonly Expression CharEquality = Expr(p => p.LastName![0] == 'c');

        [TestObject(CharComparison)]
        internal static readonly Expression CharEquality1 = Expr(p => 'c' == p.LastName![0]);

        [TestObject(CharComparison)]
        internal static readonly Expression InLeftNonConstChar = Expr(p => p.LastName![0] == 'c' || p.LastName[0] == 'd');
    }
}
