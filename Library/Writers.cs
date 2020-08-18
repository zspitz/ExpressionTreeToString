using ExpressionTreeToString.Util;
using OneOf;
using System.Collections.Generic;
using ZSpitz.Util;
using static ExpressionTreeToString.FormatterNames;

namespace ExpressionTreeToString {
    // TODO indicate that if usePathSpans == true, the returned pathSpans member of the value tuple is not null
    public delegate (string result, Dictionary<string, (int start, int length)>? pathSpans) Writer(object o, OneOf<string, Language?> languageArg, bool usePathSpans);

    public static class Writers {
        private static readonly Dictionary<string, Writer> writers =
            new Dictionary<string, Writer> {
                {CSharp, (o, languageArg, usePathSpans) =>
                    usePathSpans ?
                        (new CSharpCodeWriter(o, out var pathSpans).ToString(), pathSpans) :
                        (new CSharpCodeWriter(o).ToString(), null)
                },
                {VisualBasic, (o, languageArg, usePathSpans) => 
                    usePathSpans ?
                        (new VBCodeWriter(o, out var pathSpans).ToString(), pathSpans) :
                        (new VBCodeWriter(o).ToString(), null)
                },
                {FactoryMethods, (o, languageArg, usePathSpans) => 
                    usePathSpans ?
                        (new FactoryMethodsFormatter(o, languageArg, out var pathSpans).ToString(), pathSpans) :
                        (new FactoryMethodsFormatter(o, languageArg).ToString(), null)
                },
                {ObjectNotation, (o, languageArg, usePathSpans) =>
                    usePathSpans ?
                        (new ObjectNotationFormatter(o, languageArg, out var pathSpans).ToString(), pathSpans) :
                        (new ObjectNotationFormatter(o, languageArg).ToString(), null)
                },
                {TextualTree, (o, languageArg, usePathSpans) =>
                    usePathSpans ?
                        (new TextualTreeFormatter(o, languageArg, out var pathSpans).ToString(), pathSpans) :
                        (new TextualTreeFormatter(o, languageArg).ToString(), null)
                }
            };

        public static void Register(string key, Writer writer) => writers.Add(key, writer);

        internal static string Invoke(OneOf<string, Formatter> formatterArg, object o, OneOf<string, Language?> language) => 
            writers[formatterArg.ResolveFormatter()].Invoke(o, language, false).result;

        internal static string Invoke(OneOf<string, Formatter> formatterArg, object o, OneOf<string, Language?> language, out Dictionary<string, (int start, int length)> pathSpans) {
            var (ret, pathSpans1) = writers[formatterArg.ResolveFormatter()].Invoke(o, language, true);
            pathSpans = pathSpans1!;
            return ret;
        }
    }
}
