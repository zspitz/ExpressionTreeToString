using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTreeToString.Util {
    public static class StringExtensions {
        public static bool IsNullOrEmpty(this string? s) => string.IsNullOrEmpty(s);
    }
}
