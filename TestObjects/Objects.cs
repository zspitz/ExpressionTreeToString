using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static System.Reflection.BindingFlags;
using static System.Linq.Enumerable;

namespace ExpressionTreeTestObjects {
    public static class Objects {
        static Objects() {
            var safeTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => (x.FullName ?? "").StartsWith("ExpressionTreeTestObjects"))
                .SelectMany(x => {
                    var ret = Empty<Type>();
                    try {
                        ret = x.GetTypes();
                    } catch { }
                    return ret;
                }).Where(x => x.HasAttribute<ObjectContainerAttribute>());

            foreach (var t in safeTypes) {
                LoadType(t);
            }
        }

        private static readonly HashSet<(string category, string source, string name, object o)> objects = new HashSet<(string category, string source, string name, object o)>();
        private static readonly Dictionary<string, object> byName = new Dictionary<string, object>();

        public static (string category, string source, string name, object o)[] Get() => objects.ToArray();
        public static object ByName(string s) => byName[s];

        public static void LoadType(Type t) {
            LoadType(t, objects);
            foreach (var (_, source, name, o) in objects) {
                byName[$"{source}.{name}"] = o;
            }
        }

        internal static void LoadType(Type t, ISet<(string category, string source, string name, object o)> objects) {
            var source = t.Name;
            t.GetFields(Static | NonPublic | Public)
                .Select(fld => (
                    fld,
                    attr: fld.GetCustomAttribute<TestObjectAttribute>()
                ))
                .WhereT((fld, attr) => attr is { })
                .SelectT((fld, attr) => (
                    attr!.Category,
                    source,
                    fld.Name,
                    fld.GetValue(null)!
                ))
                .AddRangeTo(objects);
        }
    }
}
