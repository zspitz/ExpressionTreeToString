using ExpressionTreeTestObjects;
using System;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    [ObjectContainer]
    static public partial class DynamicLinqTestObjects {
        static internal Expression ExprBody<T>(Expression<Func<Person, T>> expr) => expr.Body;

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression Np = ExprBody(p => p.LastName == "A" || p.LastName == "B");

        [TestObject("Dynamic LINQ")]
        internal static readonly Expression NpMultipleFields = ExprBody(p => p.LastName == "A" || p.LastName == "B" || p.FirstName == "C" || p.FirstName == "D");

    }
}
