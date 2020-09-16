using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneOf;
using static ZSpitz.Util.Functions;

namespace ExpressionTreeToString {
    internal class InsertionPoint {
        internal readonly StringBuilder sb = new StringBuilder();
        internal readonly Dictionary<string, (int start, int length)>? pathSpans;
        internal int indentationLevel = 0;
        internal readonly List<string> pathSegments = new List<string>();
        internal InsertionPoint(bool hasPathSpans) {
            if (hasPathSpans) { pathSpans = new Dictionary<string, (int start, int length)>(); }
        }
    }

    public abstract class WriterVisitorBase {
        private readonly List<KeyValuePair<string, InsertionPoint>> insertionPoints;
        private InsertionPoint ip;
        protected readonly Language? language;

        public (string result, Dictionary<string, (int start, int length)>? pathSpans) GetResult() {
            var result = insertionPoints.Values()
                .Where(x => x.sb.Length > 0)
                .Joined(Environment.NewLine, x => x.sb.ToString());

            Dictionary<string, (int start, int length)>? pathSpans=null;
            if (insertionPoints.Any(x => x.Value.pathSpans is { })) {
                pathSpans = new Dictionary<string, (int start, int length)>();
                var offset = 0;
                foreach (var ip in insertionPoints.Values().Where(ip => ip.sb.Length > 0)) {
                    ip.pathSpans!.SelectKVP((path, span) => KVP(path, (span.start + offset, span.length))).AddRangeTo(pathSpans);
                    offset += ip.sb.Length += Environment.NewLine.Length;
                }
            }

            return (result, pathSpans);
        }

        protected WriterVisitorBase(object o, OneOf<string, Language?> languageArg, IEnumerable<string>? insertionPointKeys, bool hasPathSpans) {
            language = languageArg.ResolveLanguage();
            insertionPoints = (insertionPointKeys ?? new[] { "" }).Select(x => KVP(x, new InsertionPoint(hasPathSpans))).ToList();
            ip = insertionPoints.Get("");

            WriteNode("", o);
        }

        protected void Indent() => ip.indentationLevel += 1;
        protected void Dedent() => ip.indentationLevel -= 1;

        protected void WriteEOL(bool dedent = false) {
            ip.sb.AppendLine();
            if (dedent) { ip.indentationLevel = Math.Max(ip.indentationLevel - 1, 0); } // ensures the indentation level is never < 0
            ip.sb.Append(new string(' ', ip.indentationLevel * 4));
        }

        protected void Write(string s) => s.AppendTo(ip.sb);

        /// <summary>Write a string-rendering of an expression or other type used in expression trees</summary>
        /// <param name="o">Object to be rendered</param>
        /// <param name="parameterDeclaration">For ParameterExpression, this is a parameter declaration</param>
        /// <param name="blockType">For BlockExpression, sets the preferred block type</param>
        /// 
        protected void WriteNode(string pathSegment, object o, bool parameterDeclaration = false, object? metadata = null) {
            if (!pathSegment.IsNullOrWhitespace()) { ip.pathSegments.Add(pathSegment); }
            var start = ip.sb.Length;
            try {
                WriteNodeImpl(o, parameterDeclaration, metadata);
            } catch (NotImplementedException ex) {
                $@"--
-- Not implemented - {ex.Message}
--".AppendTo(ip.sb);
            }

            if (ip.pathSpans != null) {
                ip.pathSpans.Add(ip.pathSegments.Joined("."), (start, ip.sb.Length - start));
                if (ip.pathSegments.Any()) {
                    if (ip.pathSegments.Last() != pathSegment) { throw new InvalidOperationException(); }
                    ip.pathSegments.RemoveLast();
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
                    delimiter.AppendTo(ip.sb);
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

        protected void TrimEnd(bool trimEOL = false) => ip.sb.TrimEnd(trimEOL);

        public override string ToString() => insertionPoints.Values()
            .Where(x => x.sb.Length > 0)
            .Joined(Environment.NewLine, x => x.sb.ToString());

        protected void SetInsertionPoint(string key) => ip = insertionPoints.Get(key);
    }
}
