using System.Collections.Generic;

namespace ExpressionTreeToString {
    internal static class ListKeyValuePairExtensions {
        internal static TValue Get<TValue>(this List<KeyValuePair<string, TValue>> lst, string key) {
            foreach (var kvp in lst) {
                if (kvp.Key == key) { return kvp.Value; }
            }
            throw new KeyNotFoundException();
        }
    }
}
