using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneOf;

namespace ExpressionTreeToString {
    public abstract class WriterVisitorBase {
        private readonly StringBuilder sb = new StringBuilder();
        private readonly Dictionary<string, (int start, int length)>? pathSpans;

        /// <summary>Determines how to render literals and types</summary>
        protected readonly Language? language;

        protected WriterVisitorBase(object o, OneOf<string, Language?> languageArg) {
            language = languageArg.ResolveLanguage();
            PreWrite();
            WriteNode("", o);
        }

        protected WriterVisitorBase(object o, OneOf<string, Language?> languageArg, out Dictionary<string, (int start, int length)> pathSpans) {
            language = languageArg.ResolveLanguage();
            this.pathSpans = new Dictionary<string, (int start, int length)>();
            PreWrite();
            WriteNode("", o);
            pathSpans = this.pathSpans;
        }

        private int indentationLevel = 0;
        protected void Indent() => indentationLevel += 1;
        protected void Dedent() => indentationLevel -= 1;

        protected void WriteEOL(bool dedent = false) {
            sb.AppendLine();
            if (dedent) { indentationLevel = Math.Max(indentationLevel - 1, 0); } // ensures the indentation level is never < 0
            sb.Append(new string(' ', indentationLevel * 4));
        }

        protected void Write(string s) => s.AppendTo(sb);

        protected virtual void PreWrite() { }

        private readonly List<string> pathSegments = new List<string>();

        /// <summary>Write a string-rendering of an expression or other type used in expression trees</summary>
        /// <param name="o">Object to be rendered</param>
        /// <param name="parameterDeclaration">For ParameterExpression, this is a parameter declaration</param>
        /// <param name="blockType">For BlockExpression, sets the preferred block type</param>
        /// 
        protected void WriteNode(string pathSegment, object o, bool parameterDeclaration = false, object? metadata = null) {
            if (!pathSegment.IsNullOrWhitespace()) { pathSegments.Add(pathSegment); }
            var start = sb.Length;
            try {
                WriteNodeImpl(o, parameterDeclaration, metadata);
            } catch (NotImplementedException ex) {
                $@"--
-- Not implemented - {ex.Message}
--".AppendTo(sb);
            }

            if (pathSpans != null) {
                pathSpans.Add(pathSegments.Joined("."), (start, sb.Length - start));
                if (pathSegments.Any()) {
                    if (pathSegments.Last() != pathSegment) { throw new InvalidOperationException(); }
                    pathSegments.RemoveLast();
                }
            }
        }
        protected void WriteNode((string pathSegment, object o) x) => WriteNode(x.pathSegment, x.o);
        protected void WriteNode(string pathSegment, object o, object blockMetadata) => WriteNode(pathSegment, o, false, blockMetadata);

        protected abstract void WriteNodeImpl(object o, bool parameterDeclaration = false, object? metadata = null);

        protected void WriteNodes<T>(IEnumerable<(string pathSegment, T o)> pathsItems, bool writeEOL, string delimiter = ", ", bool parameterDeclaration = false) {
            if (writeEOL) { delimiter = delimiter.TrimEnd(); }
            pathsItems.ForEachT((pathSegment, arg, index) => {
                if (index > 0) {
                    delimiter.AppendTo(sb);
                    if (writeEOL) { WriteEOL(); }
                }
                WriteNode(pathSegment, arg!, parameterDeclaration);
            });
        }
        protected void WriteNodes<T>(IEnumerable<(string pathSegment, T o)> pathsItems, string delimiter = ", ") =>
            WriteNodes(pathsItems, false, delimiter);

        protected void WriteNodes<T>(string pathSegment, IEnumerable<T> items, bool writeEOL, string delimiter = ", ", bool parameterDeclaration = false) =>
            WriteNodes(items.Select((arg, index) => ($"{pathSegment}[{index}]", arg)), writeEOL, delimiter, parameterDeclaration);

        protected void WriteNodes<T>(string pathSegment, IEnumerable<T> items, string delimiter = ", ", bool parameterDeclaration = false) =>
            WriteNodes(pathSegment, items, false, delimiter, parameterDeclaration);

        protected void TrimEnd(bool trimEOL = false) => sb.TrimEnd(trimEOL);

        public override string ToString() => sb.ToString();
    }
}
