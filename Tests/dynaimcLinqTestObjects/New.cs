using ExpressionTreeTestObjects;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    static public partial class DynamicLinqTestObjects {
        [TestObject("New")]
        internal static readonly Expression NewAnonymous = Expr(p => new { p.LastName, p.FirstName, p.Age });

        [TestObject("New")]
        internal static readonly Expression NewNamed = Expr(p => new Person("abcd", "efgh"));
    }
}
