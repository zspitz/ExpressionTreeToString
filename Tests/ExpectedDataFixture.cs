using System.Collections.Generic;
using static ExpressionTreeToString.Tests.Functions;
using static System.Environment;
using static ZSpitz.Util.Functions;
using System.IO;
using static ExpressionTreeToString.Formatter;

namespace ExpressionTreeToString.Tests {
    public class ExpectedDataFixture {
        public static readonly Formatter[] Formatters = new[] { CSharp, VisualBasic, FactoryMethods, ObjectNotation, TextualTree };

        public readonly Dictionary<(Formatter formatter, string objectName), string> expectedStrings = new Dictionary<(Formatter formatter, string objectName), string>();
        public readonly Dictionary<string, HashSet<string>> expectedPaths = new Dictionary<string, HashSet<string>>();
        public ExpectedDataFixture() {
            foreach (var formatter in Formatters) {
                var filename = formatter.ToString();
                var expectedDataPath = GetFullFilename($"{filename.ToLower()}-testdata.txt");
                string testName = "";
                string expected = "";
                foreach (var line in File.ReadLines(expectedDataPath)) {
                    if (line.StartsWith("----")) {
                        if (testName != "") {
                            if (formatter == FactoryMethods) {
                                expected = FactoryMethodsFormatter.CSharpUsing + NewLines(2) + expected;
                            }
                            expectedStrings.Add((formatter, testName), expected.Trim());
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
