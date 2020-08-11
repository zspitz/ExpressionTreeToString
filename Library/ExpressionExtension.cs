using System.Collections.Generic;
using System.Linq.Expressions;
using OneOf;
using ZSpitz.Util;

namespace ExpressionTreeToString {
    public static class ExpressionExtension {
        public static string ToString(this Expression expr, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            WriterBase.Create(expr, formatterArg, language).ToString();

        public static string ToString(this Expression expr, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(expr, formatterArg, language, out pathSpans).ToString();

        public static string ToString(this ElementInit init, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            WriterBase.Create(init, formatterArg, language).ToString();

        public static string ToString(this ElementInit init, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(init, formatterArg, language, out pathSpans).ToString();

        public static string ToString(this MemberBinding mbind, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            WriterBase.Create(mbind, formatterArg, language).ToString();

        public static string ToString(this MemberBinding mbind, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(mbind, formatterArg, language, out pathSpans).ToString();

        public static string ToString(this SwitchCase switchCase, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            WriterBase.Create(switchCase, formatterArg, language).ToString();

        public static string ToString(this SwitchCase switchCase, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(switchCase, formatterArg, language, out pathSpans).ToString();

        public static string ToString(this CatchBlock catchBlock, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            WriterBase.Create(catchBlock, formatterArg, language).ToString();

        public static string ToString(this CatchBlock catchBlock, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(catchBlock, formatterArg, language, out pathSpans).ToString();

        public static string ToString(this LabelTarget labelTarget, OneOf<string, Formatter> formatterArg, OneOf<string, Language?> language = default) =>
            WriterBase.Create(labelTarget, formatterArg, language).ToString();

        public static string ToString(this LabelTarget labelTarget, OneOf<string, Formatter> formatterArg, out Dictionary<string, (int start, int length)> pathSpans, OneOf<string, Language?> language = default) =>
            WriterBase.Create(labelTarget, formatterArg, language, out pathSpans).ToString();
    }
}
