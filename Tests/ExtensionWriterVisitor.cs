using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTreeToString.Tests {
    public class ExtensionWriterVisitor : CSharpWriterVisitor {
        public ExtensionWriterVisitor(object o) : base(o) { }

        protected override void WriteExtension(Expression expr) {
            if (expr.CanReduce) {
                WriteNode("", expr.Reduce());
                return;
            }
            base.WriteExtension(expr);
        }
    }
}
