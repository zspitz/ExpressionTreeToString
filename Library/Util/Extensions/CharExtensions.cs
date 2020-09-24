using System.Text;

namespace ExpressionTreeToString.Util {
    public static class CharExtensions {
        public static void AppendTo(this char c, StringBuilder sb) => sb.Append(c);
    }
}
