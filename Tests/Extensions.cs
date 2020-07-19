using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;

namespace ExpressionTreeToString.Tests {
    internal static class Extensions {
        internal static TheoryData<T1, T2, T3, T4> ToTheoryData<T1, T2, T3, T4>(this IEnumerable<ValueTuple<T1, T2, T3, T4>> src) {
            var ret = new TheoryData<T1, T2, T3, T4>();
            foreach (var (a, b, c, d) in src) {
                ret.Add(a, b, c, d);
            }
            return ret;
        }

#if NETSTANDARD2_0
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> src) => new HashSet<T>(src);
#endif

    }
}
