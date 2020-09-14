using System;
using System.IO;
using System.Reflection;

namespace ExpressionTreeToString.Tests {
    public static class Functions {
        public static string GetFullFilename(string filename) {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            if (codeBase == null) { throw new InvalidOperationException(); }
            string executable = new Uri(codeBase).LocalPath;
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(executable)!, "expectedResults", filename));
        }
    }
}
