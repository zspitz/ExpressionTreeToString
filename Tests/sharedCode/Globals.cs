using System;
using System.Linq;

namespace ExpressionTreeToString.Tests {
    public static class Globals {
        public static readonly BuiltinRenderer[] BuiltinRenderers =
            Enum.GetValues(typeof(BuiltinRenderer))
                .Cast<BuiltinRenderer>()
                .ToArray();
    }
}
