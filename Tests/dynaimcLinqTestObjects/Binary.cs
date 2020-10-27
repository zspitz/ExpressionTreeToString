using ExpressionTreeTestObjects;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    static public partial class DynamicLinqTestObjects {
        [TestObject("Binary")]
        internal static readonly Expression Equal = ExprBody(p => p.LastName == "A");

    }
}
