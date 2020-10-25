using ExpressionTreeToString.Util;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using ZSpitz.Util;
using static ExpressionTreeToString.RendererNames;

namespace ExpressionTreeToString {
    // TODO indicate that if usePathSpans == true, the returned pathSpans member of the value tuple is not null
    public delegate (string result, Dictionary<string, (int start, int length)>? pathSpans) Renderer(object o, OneOf<string, Language?> languageArg, bool usePathSpans);

    public static class Renderers {
        private static readonly Dictionary<string, Renderer> writers =
            new Dictionary<string, Renderer>(StringComparer.InvariantCultureIgnoreCase) {
                {CSharp, (o, languageArg, usePathSpans) => new CSharpWriterVisitor(o, usePathSpans).GetResult() },
                {VisualBasic, (o, languageArg, usePathSpans) => new VBWriterVisitor(o, usePathSpans).GetResult() },
                {FactoryMethods, (o, languageArg, usePathSpans) => new FactoryMethodsWriterVisitor(o, languageArg, usePathSpans).GetResult() },
                {ObjectNotation, (o, languageArg, usePathSpans) =>new ObjectNotationWriterVisitor(o, languageArg, usePathSpans).GetResult() },
                {TextualTree, (o, languageArg, usePathSpans) => new TextualTreeWriterVisitor(o, languageArg, usePathSpans).GetResult() },
                {ToStringRenderer, (o, languageArg, usePathSpans) => new ToStringWriterVisitor(o, usePathSpans).GetResult() },
                {DebugView, (o, languageArg, usePathSpans) => new DebugViewWriterVisitor(o, usePathSpans).GetResult() },
                {DynamicLinq, (o, languageArg, usePathSpans) => new DynamicLinqWriterVisitor(o, languageArg, usePathSpans).GetResult() }
            };

        public static void Register(string key, Renderer writer) => writers.Add(key, writer);

        public static string Invoke(OneOf<string, BuiltinRenderer> rendererArg, object o, OneOf<string, Language?> language) => 
            writers[rendererArg.ResolveRendererKey()].Invoke(o, language, false).result;

        public static string Invoke(OneOf<string, BuiltinRenderer> rendererArg, object o, OneOf<string, Language?> language, out Dictionary<string, (int start, int length)> pathSpans) {
            var (ret, pathSpans1) = writers[rendererArg.ResolveRendererKey()].Invoke(o, language, true);
            pathSpans = pathSpans1!;
            return ret;
        }

        public static string[] RendererKeys => writers.Keys.Ordered().ToArray();
    }
}
