using System.Collections.Generic;
using System.Linq;

namespace ExpressionTreeToString.Util {
    public static class IEnumerableTExtensions {
        public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> src) => src.Select((x, index) => (x, index));
    }
}
