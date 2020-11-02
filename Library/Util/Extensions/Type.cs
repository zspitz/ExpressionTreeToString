using System;
using ZSpitz.Util;

namespace ExpressionTreeToString.Util {
    public static class TypeExtensions {
        // TODO we need to distinguish between the built-in Action/Func, and one that someone else has defined

        // https://stackoverflow.com/a/5150373/111794
        public static bool IsAction(this Type t) => t.InheritsFromOrImplements<MulticastDelegate>() && t.FullName.StartsWith("System.Action");

        public static bool IsFunc(this Type t) => t.InheritsFromOrImplements<MulticastDelegate>() && t.FullName.StartsWith("System.Func");
    }
}
