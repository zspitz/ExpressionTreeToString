using System.Collections.Generic;
using System.Linq.Expressions;
using OneOf;
using ZSpitz.Util;
using static ExpressionTreeToString.Renderers;

namespace ExpressionTreeToString {
    public static class ExpressionExtension {
        public static string ToString(this Expression expr, OneOf<string, BuiltinRenderer> rendererArg, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, expr, language);

        public static string ToString(this Expression expr, OneOf<string, BuiltinRenderer> rendererArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) => 
            Invoke(rendererArg, expr, language, out pathSpans);

        public static string ToString(this ElementInit init, OneOf<string, BuiltinRenderer> rendererArg, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, init, language);

        public static string ToString(this ElementInit init, OneOf<string, BuiltinRenderer> rendererArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) => 
            Invoke(rendererArg, init, language, out pathSpans);

        public static string ToString(this MemberBinding mbind, OneOf<string, BuiltinRenderer> rendererArg, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, mbind, language);

        public static string ToString(this MemberBinding mbind, OneOf<string, BuiltinRenderer> rendererArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, mbind, language, out pathSpans);

        public static string ToString(this SwitchCase switchCase, OneOf<string, BuiltinRenderer> rendererArg, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, switchCase, language);

        public static string ToString(this SwitchCase switchCase, OneOf<string, BuiltinRenderer> rendererArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, switchCase, language, out pathSpans);

        public static string ToString(this CatchBlock catchBlock, OneOf<string, BuiltinRenderer> rendererArg, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, catchBlock, language);

        public static string ToString(this CatchBlock catchBlock, OneOf<string, BuiltinRenderer> rendererArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, catchBlock, language, out pathSpans);

        public static string ToString(this LabelTarget labelTarget, OneOf<string, BuiltinRenderer> rendererArg, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, labelTarget, language);

        public static string ToString(this LabelTarget labelTarget, OneOf<string, BuiltinRenderer> rendererArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            Invoke(rendererArg, labelTarget, language, out pathSpans);
    }
}
