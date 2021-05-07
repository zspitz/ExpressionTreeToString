using System;
using System.IO;
using System.Reflection;

namespace ExpressionTreeToString.Tests {
    public static class Functions {
        public static string GetFullFilename(string filename) {

#if NET5_0_OR_GREATER
#pragma warning disable SYSLIB0012 // CodeBase is obsoleted in .NET 5.0
#endif
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
#if NET5_0_OR_GREATER
#pragma warning restore SYSLIB0012
#endif

            if (codeBase == null) { throw new InvalidOperationException(); }
            var executable = new Uri(codeBase).LocalPath;
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(executable)!, "expectedResults", filename));
        }
    }
}
