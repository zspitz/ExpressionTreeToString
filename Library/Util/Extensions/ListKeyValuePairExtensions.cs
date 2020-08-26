using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
