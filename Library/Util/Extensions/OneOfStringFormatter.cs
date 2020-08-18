using OneOf;
using System;
using static ExpressionTreeToString.FormatterNames;

namespace ExpressionTreeToString.Util {
    internal static class OneOfStringFormatterExtensions {
        internal static string ResolveFormatter(this OneOf<string, Formatter> formatterArg) =>
            formatterArg.IsT0 ?
                formatterArg.AsT0 :
                formatterArg.AsT1 switch
                {
                    Formatter.CSharp => CSharp,
                    Formatter.VisualBasic => VisualBasic,
                    Formatter.FactoryMethods => FactoryMethods,
                    Formatter.ObjectNotation => ObjectNotation,
                    Formatter.TextualTree => TextualTree,
                    _ => throw new ArgumentException("Unknown formatter")
                };
    }
}
