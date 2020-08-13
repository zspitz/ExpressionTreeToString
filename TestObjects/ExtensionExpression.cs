using System;
using System.Linq.Expressions;

namespace TestObjects {
    public class ExtensionExpression : Expression {
        public override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type => typeof(int);
        public override bool CanReduce => true;
        public override Expression Reduce() => Constant(42);
    }
}
