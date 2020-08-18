using OneOf;
using System;
using static ExpressionTreeToString.FormatterNames;

namespace ExpressionTreeToString {
    public static class Functions {
        public static string ResolveFormatter(OneOf<string, Formatter> formatterArg) =>
            formatterArg.IsT0 ? formatterArg.AsT0 :
            formatterArg.AsT1 switch {
                Formatter.CSharp => CSharp,
                Formatter.VisualBasic => VisualBasic,
                Formatter.FactoryMethods => FactoryMethods,
                Formatter.TextualTree => TextualTree,
                Formatter.ObjectNotation => ObjectNotation,
                _ => throw new ArgumentException("Unknown formatter")
            };
    }
}
