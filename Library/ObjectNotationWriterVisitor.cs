using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static ZSpitz.Util.Functions;
using static ExpressionTreeToString.Globals;
using System.Collections;
using OneOf;
using static ZSpitz.Util.Language;
using static ExpressionTreeToString.Util.Functions;

namespace ExpressionTreeToString {
    public class ObjectNotationWriterVisitor : WriterVisitorBase {
        private static string[] insertionPointKeys = new[] { "declarations", "" };

        public ObjectNotationWriterVisitor(object o, OneOf<string, Language?> languageArg, bool hasPathSpans = false)
            : base(o, languageArg.ResolveLanguage() ?? throw new ArgumentException("Invalid language"), insertionPointKeys, hasPathSpans) { }

        private Dictionary<ParameterExpression, int>? _ids;

        protected override void WriteNodeImpl(object o, bool parameterDeclaration = false, object? metadata = null) {
            if (o is ParameterExpression pexpr) {
                Write(GetVariableName(pexpr, ref _ids));

                if (!parameterDeclaration) { return; }

                SetInsertionPoint("declarations");
                Write(language switch
                {
                    CSharp => "var",
                    VisualBasic => "Dim",
                    _ => throw new NotImplementedException("Invalid language.")
                });
                Write($" {GetVariableName(pexpr, ref _ids)} = ");
            }

            var type = writeNew(o);
            var preferredOrder = PreferredPropertyOrders.FirstOrDefault(x => x.type.IsAssignableFrom(o.GetType())).propertyNames;
            var properties = type.GetProperties().Where(x => {
                if (x.Name.In("CanReduce", "TailCall", "CanReduce", "IsLifted", "IsLiftedToNull", "ArgumentCount")) { return false; }
                if (x.Name == "NodeType" && type.In(hideNodeType)) { return false; }
                return true;
            }).ToList();

            if (properties.None()) {
                if (language == CSharp) { Write("()"); }
                return;
            }

            if (language == VisualBasic) { Write(" With"); }
            Write(" {");
            Indent();
            WriteEOL();

            properties.OrderBy(x => {
                if (x.Name.In("NodeType", "Type")) { return -2; }
                if (preferredOrder == null) { return -1; }
                var indexOf = Array.IndexOf(preferredOrder, x.Name);
                if (indexOf == -1) { return 1000; }
                return indexOf;
            })
            .ThenBy(x => x.Name)
            .Select(x => {
                object value;
                try {
                    value = x.GetValue(o);
                } catch (Exception ex) {
                    value = ex.Message;
                }
                return (x, value);
            })
            .WhereT((_, value) => {
                if (value == null) { return false; }
                if (value is IEnumerable seq && seq.None()) { return false; }
                return true;
            })
            .ForEachT((x, value, index) => {
                if (index > 0) {
                    Write(",");
                    WriteEOL();
                }
                if (language == VisualBasic) { Write("."); }
                Write(x.Name);
                Write(" = ");

                if (x.PropertyType.InheritsFromOrImplementsAny(PropertyTypes)) {
                    var parameterDeclaration =
                        (o is LambdaExpression && x.Name == "Parameters") ||
                        (o is BlockExpression && x.Name == "Variables");
                    WriteCollection((IEnumerable)value, x.Name, parameterDeclaration);
                } else if (x.PropertyType.InheritsFromOrImplementsAny(NodeTypes)) {
                    WriteNode(x.Name, value);
                } else {
                    Write(RenderLiteral(value, language));
                }
            });

            WriteEOL(true);
            Write("}");

            if (parameterDeclaration) {
                Write(";");
                WriteEOL();
                SetInsertionPoint("");
            }
        }

        private static readonly HashSet<Type> hideNodeType = new HashSet<Type>() {
            typeof(BlockExpression),
            typeof(ConditionalExpression),
            typeof(ConstantExpression),
            typeof(DebugInfoExpression),
            typeof(DefaultExpression),
            typeof(DynamicExpression),
            typeof(GotoExpression),
            typeof(IndexExpression),
            typeof(InvocationExpression),
            typeof(LabelExpression),
            typeof(ListInitExpression),
            typeof(LoopExpression),
            typeof(MemberExpression),
            typeof(MemberInitExpression),
            typeof(MethodCallExpression),
            typeof(NewExpression),
            typeof(ParameterExpression),
            typeof(RuntimeVariablesExpression),
            typeof(SwitchExpression),
            typeof(TryExpression)
        };

        private void WriteCollection(IEnumerable seq, string pathSegment, bool parameterDeclaration = false) {
            writeNew(seq);
            var items = seq.Cast<object>().ToList();
            if (items.None()) {
                if (language == CSharp) { Write("()"); }
                return;
            }
            if (language == VisualBasic) { Write(" From"); }
            Write(" {");
            Indent();
            WriteEOL();

            WriteNodes(pathSegment, items, true, ",", parameterDeclaration);

            WriteEOL(true);
            Write("}");
        }

        private Type writeNew(object o) {
            Write(
                language == CSharp ? "new " :
                language == VisualBasic ? "New " :
                ""
            );
            var t = o.GetType().BaseTypes(false, true).First(x => x.IsPublic && !x.IsInterface);
            Write(t.FriendlyName(language));
            return t;
        }
    }
}
