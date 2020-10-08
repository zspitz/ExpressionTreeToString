using ZSpitz.Util;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static ZSpitz.Util.Functions;
using static ExpressionTreeToString.Globals;
using System.Collections;
using ExpressionTreeToString.Util;
using OneOf;

namespace ExpressionTreeToString {
    public class TextualTreeWriterVisitor : WriterVisitorBase {
        public static Func<Expression, bool>? ReducePredicate;

        public TextualTreeWriterVisitor(object o, OneOf<string, Language?> languageArg, bool hasPathSpans = false) 
            : base(o, languageArg, null, hasPathSpans) { }

        private readonly ValueExtractor valueExtractor = new ValueExtractor();

        protected override sealed void WriteNodeImpl(object o, bool _ = false, object? _1 = null) {
            var nodeType = "";
            var typename = "";
            var name = "";
            object? value = null;

            switch (o) {
                case Expression expr:
                    nodeType = expr.NodeType.ToString();
                    typename = $"({expr.Type.FriendlyName(language)})";
                    name = expr.Name();
                    if (name is { } && name.ContainsWhitespace()) { name = $"\"{name}\""; }
                    value = valueExtractor.GetValue(expr).value;
                    break;

                case MemberBinding mbind:
                    nodeType = mbind.BindingType.ToString();
                    name = mbind.Member.Name;
                    break;
                case CallSiteBinder binder:
                    nodeType = binder.BinderType().ToString();
                    break;
                default:
                    nodeType = o.GetType().FriendlyName(language);
                    break;
            }

            string stringValue = "";
            if (value != null) { stringValue = "= " + StringValue(value, language); }

            Write((nodeType, typename, name, stringValue).Where(x => !x.IsNullOrWhitespace()).Joined(" "));

            var type = o.GetType();
            var preferredOrder = PreferredPropertyOrders.FirstOrDefault(x => x.type.IsAssignableFrom(o.GetType())).propertyNames;
            var childNodes = type.GetProperties()
                .Where(prp =>
                    prp.PropertyType.InheritsFromOrImplementsAny(PropertyTypes) ||
                    prp.PropertyType.InheritsFromOrImplementsAny(NodeTypes)
                )
                .OrderBy(x => {
                    if (preferredOrder == null) { return -1; }
                    return Array.IndexOf(preferredOrder, x.Name);
                })
                .ThenBy(x => x.Name)
                .SelectMany(prp => {
                    if (prp.PropertyType.InheritsFromOrImplements<IEnumerable>()) {
                        return (prp.GetValue(o) as IEnumerable).Cast<object>().Select((x, index) => (name: $"{prp.Name}[{index}]", value: x));
                    } else {
                        return new[] { (prp.Name, prp.GetValue(o)) };
                    }
                })
                .Where(x => x.value != null)
                .ToList();

            static bool defaultReduceFilter(Expression expr2) => expr2.NodeType == ExpressionType.Extension;

            if (o is Expression expr1 && expr1.CanReduce && (ReducePredicate ?? defaultReduceFilter)(expr1)) {
                Expression reduced = expr1;
                while (reduced.CanReduce) {
                    reduced = reduced.Reduce();
                }
                childNodes.Add("Reduce()", reduced);
            }

            if (childNodes.Any()) {
                Indent();
                WriteEOL();
                childNodes.ForEach((node, index) => {
                    if (index > 0) { WriteEOL(); }
                    Write($"· {node.name} - ");
                    WriteNode(node);
                });
                Dedent();
            }
        }
    }
}
