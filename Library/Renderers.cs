using ExpressionTreeToString.Util;
using OneOf;
using System.Collections.Generic;
using ZSpitz.Util;
using static ExpressionTreeToString.RendererNames;

namespace ExpressionTreeToString {
    // TODO indicate that if usePathSpans == true, the returned pathSpans member of the value tuple is not null
    public delegate (string result, Dictionary<string, (int start, int length)>? pathSpans) Renderer(object o, OneOf<string, Language?> languageArg, bool usePathSpans);

    public static class Renderers {
        private static readonly Dictionary<string, Renderer> writers =
            new Dictionary<string, Renderer> {
                {CSharp, (o, languageArg, usePathSpans) =>
                    usePathSpans ?
                        (new CSharpWriterVisitor(o, out var pathSpans).ToString(), pathSpans) :
                        (new CSharpWriterVisitor(o).ToString(), null)
                },
                {VisualBasic, (o, languageArg, usePathSpans) => 
                    usePathSpans ?
                        (new VBWriterVisitor(o, out var pathSpans).ToString(), pathSpans) :
                        (new VBWriterVisitor(o).ToString(), null)
                },
                {FactoryMethods, (o, languageArg, usePathSpans) => 
                    usePathSpans ?
                        (new FactoryMethodsWriterVisitor(o, languageArg, out var pathSpans).ToString(), pathSpans) :
                        (new FactoryMethodsWriterVisitor(o, languageArg).ToString(), null)
                },
                {ObjectNotation, (o, languageArg, usePathSpans) =>
                    usePathSpans ?
                        (new ObjectNotationWriterVisitor(o, languageArg, out var pathSpans).ToString(), pathSpans) :
                        (new ObjectNotationWriterVisitor(o, languageArg).ToString(), null)
                },
                {TextualTree, (o, languageArg, usePathSpans) =>
                    usePathSpans ?
                        (new TextualTreeWriterVisitor(o, languageArg, out var pathSpans).ToString(), pathSpans) :
                        (new TextualTreeWriterVisitor(o, languageArg).ToString(), null)
                }
            };

        public static void Register(string key, Renderer writer) => writers.Add(key, writer);

        public static string Invoke(OneOf<string, BuiltinRenderer> rendererArg, object o, OneOf<string, Language?> language) => 
            writers[rendererArg.ResolveRendererKey()].Invoke(o, language, false).result;

        public static string Invoke(OneOf<string, BuiltinRenderer> rendererArg, object o, OneOf<string, Language?> language, out Dictionary<string, (int start, int length)> pathSpans) {
            var (ret, pathSpans1) = writers[rendererArg.ResolveRendererKey()].Invoke(o, language, true);
            pathSpans = pathSpans1!;
            return ret;
        }
    }
}
