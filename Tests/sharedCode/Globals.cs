using ExpressionTreeTestObjects.VB;
using System;
using System.Linq;
using static ExpressionTreeTestObjects.Objects;

namespace ExpressionTreeToString.Tests {
    public static class Globals {
        public static readonly BuiltinRenderer[] BuiltinRenderers =
            Enum.GetValues(typeof(BuiltinRenderer))
                .Cast<BuiltinRenderer>()
                .ToArray();

        public static readonly (string category, string source, string name, object o)[] Objects;

        static Globals() {
            Loader.Load();
            LoadType(typeof(DynamicLinqTestObjects));
            Objects = Get();
        }
    }
}
