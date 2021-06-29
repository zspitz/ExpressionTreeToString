using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneOf;
using static ZSpitz.Util.Functions;
using ExpressionTreeToString.Util;

namespace ExpressionTreeToString {
    internal class InsertionPoint {
        internal readonly StringBuilder sb = new();
        internal readonly Dictionary<string, (int start, int length)>? pathSpans;
        internal int indentationLevel = 0;
        internal string key;
        internal InsertionPoint(string key, bool hasPathSpans) {
            if (hasPathSpans) { pathSpans = new(); }
            this.key = key;
        }
    }

    public abstract class WriterVisitorBase {
        private readonly List<InsertionPoint> insertionPoints;
        private InsertionPoint ip;
        protected readonly Language? language;
        private readonly List<string> pathSegments = new();
        private readonly bool hasPathSpans;

        protected int CurrentIndentationLevel => ip.indentationLevel;

        public (string result, Dictionary<string, (int start, int length)>? pathSpans) GetResult() {
            var result = insertionPoints
                .Where(x => x.sb.Length > 0)
                .Joined(Environment.NewLine, x => x.sb.ToString())
                .Trim('\0');

            Dictionary<string, (int start, int length)>? pathSpans=null;
            if (insertionPoints.Any(x => x.pathSpans is { })) {
                pathSpans = new Dictionary<string, (int start, int length)>();
                var offset = 0;
                foreach (var ip in insertionPoints.Where(ip => ip.sb.Length > 0)) {
                    ip.pathSpans!.SelectKVP((path, span) => KVP(path, (span.start + offset, span.length))).AddRangeTo(pathSpans);
                    offset += ip.sb.Length += Environment.NewLine.Length;
                }
            }

            return (result, pathSpans);
        }

        protected WriterVisitorBase(object o, OneOf<string, Language?> languageArg, IEnumerable<string>? insertionPointKeys, bool hasPathSpans) {
            language = languageArg.ResolveLanguage();
            this.hasPathSpans = hasPathSpans;
            insertionPoints = (insertionPointKeys ?? new[] { "" }).Select(x => new InsertionPoint(x, hasPathSpans)).ToList();
            ip = insertionPoints.Get("");

            WriteNode("", o);
        }

        protected void Indent() => ip.indentationLevel += 1;
        protected void Dedent() => ip.indentationLevel = Math.Max(ip.indentationLevel - 1, 0); // ensures the indentation level is never < 0

        protected void WriteEOL(bool dedent = false) {
            ip.sb.AppendLine();
            if (dedent) { Dedent(); }
            ip.sb.Append(new string(' ', ip.indentationLevel * 4));
        }

        protected void Write(string? s) => s.AppendTo(ip.sb);
        protected void Write(char c) => c.AppendTo(ip.sb);

        /// <summary>Write a string-rendering of an expression or other type used in expression trees</summary>
        /// <param name="o">Object to be rendered</param>
        /// <param name="parameterDeclaration">For ParameterExpression, this is a parameter declaration</param>
        /// <param name="blockType">For BlockExpression, sets the preferred block type</param>
        /// 
        protected void WriteNode(string pathSegment, object? o, bool parameterDeclaration = false, object? metadata = null) {
            if (!pathSegment.IsNullOrWhitespace()) { pathSegments.Add(pathSegment); }
            var start = ip.sb.Length;
            try {
                WriteNodeImpl(o, parameterDeclaration, metadata);
            } catch (NotImplementedException ex) {
                WriteNotImplemented(ex.Message);
            }

            if (ip.pathSpans is { }) {
                ip.pathSpans.Add(pathSegments.Joined("."), (start, ip.sb.Length - start));
                if (pathSegment.Any()) {
                    if (pathSegments.Last() != pathSegment) { throw new InvalidOperationException(); }
                    pathSegments.RemoveLast();
                }
            }
        }
        protected void WriteNode((string pathSegment, object o) x) => WriteNode(x.pathSegment, x.o);
        protected void WriteNode(string pathSegment, object o, object blockMetadata) => WriteNode(pathSegment, o, false, blockMetadata);

        protected abstract void WriteNodeImpl(object? o, bool parameterDeclaration = false, object? metadata = null);

        protected void WriteNodes<T>(IEnumerable<(string pathSegment, T o)> pathsItems, bool writeEOL, string delimiter = ", ", bool parameterDeclaration = false) {
            if (writeEOL) { delimiter = delimiter.TrimEnd(); }
            foreach (var (pathSegment, arg, index) in pathsItems.WithIndex()) {
                if (index > 0) {
                    delimiter.AppendTo(ip.sb);
                    if (writeEOL) { WriteEOL(); }
                }
                WriteNode(pathSegment, arg!, parameterDeclaration);
            }
        }
        protected void WriteNodes<T>(IEnumerable<(string pathSegment, T o)> pathsItems, string delimiter = ", ") =>
            WriteNodes(pathsItems, false, delimiter);

        protected void WriteNodes<T>(string pathSegment, IEnumerable<T> items, bool writeEOL, string delimiter = ", ", bool parameterDeclaration = false) =>
            WriteNodes(items.Select((arg, index) => ($"{pathSegment}[{index}]", arg)), writeEOL, delimiter, parameterDeclaration);

        protected void WriteNodes<T>(string pathSegment, IEnumerable<T> items, string delimiter = ", ", bool parameterDeclaration = false) =>
            WriteNodes(pathSegment, items, false, delimiter, parameterDeclaration);

        protected void TrimEnd(bool trimEOL = false) => ip.sb.TrimEnd(trimEOL);

        public override string ToString() => insertionPoints
            .Where(x => x.sb.Length > 0)
            .Joined(Environment.NewLine, x => x.sb.ToString());

        protected void SetInsertionPoint(string key) => ip = insertionPoints.Get(key);
        protected void AddInsertionPoint(string key) {
            var ip = new InsertionPoint(key, hasPathSpans);
            insertionPoints.Add(key, ip);
            this.ip = ip;
        }
        protected string CurrentInsertionPoint => ip.key;

        protected void WriteNotImplemented(string message) {
            if (message == "The method or operation is not implemented.") {
                message = "";
            }
            if (!message.IsNullOrEmpty()) { 
                message = $" - {message}";
            }
            $@"--
-- Not implemented{message}
--".AppendTo(ip.sb);
        }
    }
}
