using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressionTreeTestObjects {
    internal static class Extensions {
        internal static IEnumerable<T> GetAttributes<T>(this Type type, bool inherit) where T : Attribute =>
            type.GetCustomAttributes(typeof(T), inherit).Cast<T>();

        internal static PropertyInfo[] GetIndexers(this Type type, bool inherit) {
            var memberName = type.GetAttributes<DefaultMemberAttribute>(inherit).FirstOrDefault()?.MemberName;
            return memberName == null ? 
                new PropertyInfo[] { } : 
                type.GetProperties().Where(x => x.Name == memberName).ToArray();
        }

        internal static void AddRangeTo<T>(this IEnumerable<T> src, ICollection<T> dest) {
            foreach (var item in src) {
                dest.Add(item);
            }
        }

        internal static bool HasAttribute<TAttribute>(this MemberInfo mi, bool inherit = false) where TAttribute : Attribute =>
            mi.GetCustomAttributes(typeof(TAttribute), inherit).Any();

        internal static IEnumerable<TResult> SelectT<T1, T2, TResult>(this IEnumerable<ValueTuple<T1, T2>> src, Func<T1, T2, TResult> selector) =>
            src.Select(x => selector(x.Item1, x.Item2));

        internal static IEnumerable<(T1, T2)> WhereT<T1, T2>(this IEnumerable<(T1, T2)> src, Func<T1, T2, bool> predicate) => 
            src.Where(x => predicate(x.Item1, x.Item2));
    }
}
