using OneOf;
using System;
using static ExpressionTreeToString.Formatter;

namespace ExpressionTreeToString.Util {
    internal static class OneOfStringFormatterExtensions {
        internal static Formatter ResolveFormatter(this OneOf<string, Formatter> formatterArg) =>
            formatterArg.IsT1 ?
                formatterArg.AsT1 :
                formatterArg.AsT0 switch {
                    FormatterNames.CSharp => CSharp,
                    FormatterNames.VisualBasic => VisualBasic,
                    FormatterNames.FactoryMethods => FactoryMethods,
                    FormatterNames.ObjectNotation => ObjectNotation,
                    FormatterNames.TextualTree => TextualTree,
                    _ => throw new ArgumentException("Unknown formatter")
                };
    }
}
