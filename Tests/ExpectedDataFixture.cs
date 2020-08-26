using System.Collections.Generic;
using static ExpressionTreeToString.Tests.Functions;
using static System.Environment;
using static ZSpitz.Util.Functions;
using System.IO;
using static ExpressionTreeToString.BuiltinRenderer;

namespace ExpressionTreeToString.Tests {
    public class ExpectedDataFixture {
        public static readonly BuiltinRenderer[] RendererKeys = new[] { CSharp, VisualBasic, FactoryMethods, ObjectNotation, TextualTree };

        public readonly Dictionary<(BuiltinRenderer rendererKey, string objectName), string> expectedStrings = new Dictionary<(BuiltinRenderer rendererKey, string objectName), string>();
        public readonly Dictionary<string, HashSet<string>> expectedPaths = new Dictionary<string, HashSet<string>>();
        public ExpectedDataFixture() {
            foreach (var key in RendererKeys) {
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
