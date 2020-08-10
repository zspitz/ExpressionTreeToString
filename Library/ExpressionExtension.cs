using System.Collections.Generic;
using System.Linq.Expressions;
using OneOf;
using ZSpitz.Util;

namespace ExpressionTreeToString {
    public static class ExpressionExtension {
        public static string ToString(this Expression expr, string formatter, OneOf<string, Language?> language = default) =>
            WriterBase.Create(expr, formatter, language).ToString();

        public static string ToString(this Expression expr, string formatter, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(expr, formatter, language, out pathSpans).ToString();

        public static string ToString(this ElementInit init, string formatter, OneOf<string, Language?> language = default) =>
            WriterBase.Create(init, formatter, language).ToString();

        public static string ToString(this ElementInit init, string formatter, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(init, formatter, language, out pathSpans).ToString();

        public static string ToString(this MemberBinding mbind, string formatter, OneOf<string, Language?> language = default) =>
            WriterBase.Create(mbind, formatter, language).ToString();

        public static string ToString(this MemberBinding mbind, string formatter, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(mbind, formatter, language, out pathSpans).ToString();

        public static string ToString(this SwitchCase switchCase, string formatter, OneOf<string, Language?> language = default) =>
            WriterBase.Create(switchCase, formatter, language).ToString();

        public static string ToString(this SwitchCase switchCase, string formatter, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(switchCase, formatter, language, out pathSpans).ToString();

        public static string ToString(this CatchBlock catchBlock, string formatter, OneOf<string, Language?> language = default) =>
            WriterBase.Create(catchBlock, formatter, language).ToString();

        public static string ToString(this CatchBlock catchBlock, string formatter, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(catchBlock, formatter, language, out pathSpans).ToString();

        public static string ToString(this LabelTarget labelTarget, string formatter, OneOf<string, Language?> language = default) =>
            WriterBase.Create(labelTarget, formatter, language).ToString();

        public static string ToString(this LabelTarget labelTarget, string formatter, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(labelTarget, formatter, language, out pathSpans).ToString();
    }
}
