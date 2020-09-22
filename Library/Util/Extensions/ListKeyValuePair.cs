using System.Collections.Generic;

namespace ExpressionTreeToString {
    static internal class ListKeyValuePairExtensions {
        internal static TValue Get<TValue>(this List<KeyValuePair<string, TValue>> lst, string key) {
            foreach (var kvp in lst) {
                if (kvp.Key == key) { return kvp.Value; }
            }
            throw new KeyNotFoundException();
        }
    }
}
