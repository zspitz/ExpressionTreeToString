using System.Collections.Generic;
using static ExpressionTreeToString.Tests.Globals;
using static ExpressionTreeToString.Tests.Functions;
using static System.Environment;
using static ZSpitz.Util.Functions;
using System.IO;
using static ExpressionTreeToString.BuiltinRenderer;
using ZSpitz.Util;

namespace ExpressionTreeToString.Tests {
    public class FileDataFixture {
        public readonly Dictionary<(BuiltinRenderer rendererKey, string objectName), string> expectedStrings = new Dictionary<(BuiltinRenderer rendererKey, string objectName), string>();
        public FileDataFixture() {
            foreach (var key in BuiltinRenderers) {
                if (key.In(DebugView, BuiltinRenderer.ToString)) { continue; }
                var expectedDataPath = GetFullFilename($"{key.ToString().ToLower()}-testdata.txt");
                string testName = "";
                string expected = "";
                foreach (var line in File.ReadLines(expectedDataPath)) {
                    if (line.StartsWith("----")) {
                        if (testName != "") {
                            if (key == FactoryMethods) {
                                expected = "// using static System.Linq.Expressions.Expression" + NewLines(2) + expected;
                            }
                            expectedStrings.Add((key, testName), expected.Trim());
                        }
                        testName = line.Substring(5); // ---- typename.testMethod
                        expected = "";
                    } else {
                        expected += line + NewLine;
                    }
                }
            }
        }
    }
}
