using System.Collections.Generic;
using System.Linq.Expressions;
using OneOf;
using ZSpitz.Util;
using static ExpressionTreeToString.Writers;

namespace ExpressionTreeToString {
    public static class ExpressionExtension {
        public static string ToString(this Expression expr, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, expr, language);

        public static string ToString(this Expression expr, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) => 
            Invoke(formatterArg, expr, language, out pathSpans);

        public static string ToString(this ElementInit init, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, init, language);

        public static string ToString(this ElementInit init, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) => 
            Invoke(formatterArg, init, language, out pathSpans);

        public static string ToString(this MemberBinding mbind, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, mbind, language);

        public static string ToString(this MemberBinding mbind, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, mbind, language, out pathSpans);

        public static string ToString(this SwitchCase switchCase, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, switchCase, language);

        public static string ToString(this SwitchCase switchCase, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, switchCase, language, out pathSpans);

        public static string ToString(this CatchBlock catchBlock, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, catchBlock, language);

        public static string ToString(this CatchBlock catchBlock, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, catchBlock, language, out pathSpans);

        public static string ToString(this LabelTarget labelTarget, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, labelTarget, language);

        public static string ToString(this LabelTarget labelTarget, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            Invoke(formatterArg, labelTarget, language, out pathSpans);
    }
}
