using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ZSpitz.Util;

namespace ExpressionTreeToString.Util {
    public static class Functions {
        // TODO indicate that ids is not going to be null after returning
        public static int GetId<T>(T e, [NotNull] ref Dictionary<T, int>? ids, int offset = 0) where T : notnull => 
            GetId(e, ref ids, out var _, offset);
        public static int GetId<T>(T e, [NotNull] ref Dictionary<T, int>? ids, out bool isNew, int offset = 0) where T : notnull {
            isNew = false;
            if (ids is null) {
                ids = new Dictionary<T, int>();
            }

            if (!ids.TryGetValue(e, out int id)) {
                isNew = true;
                id = ids.Count + offset;
                ids.Add(e, id);
            }

            return id;
        }

        public static string GetVariableName(ParameterExpression prm, ref Dictionary<ParameterExpression, int>? ids, string autonamedPrefix = "var") {
            var name = prm.Name;
            if (name.IsNullOrEmpty()) {
                return $"${autonamedPrefix}{GetId(prm, ref ids)}";
            }
            if (name.ContainsWhitespace()) {
                return $"${name.ReplaceWhitespace("_")}";
            }
            return name;
        }
    }
}
