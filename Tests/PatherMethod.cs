using Pather.CSharp.PathElements;
using System;
using System.Text.RegularExpressions;
using static System.Reflection.BindingFlags;

namespace ExpressionTreeToString.Tests {
    public class MethodInvocationFactory : IPathElementFactory {
        public bool IsApplicable(string path) => Regex.IsMatch(path, @"^\w+\(\)");

        public IPathElement Create(string path, out string newPath) {
            var methodName = Regex.Matches(path, @"^(\w+)\(\)")[0].Groups[1].Value;
            newPath = path.Remove(0, methodName.Length + 2);
            return new MethodInvocation(methodName);
        }
    }

    public class MethodInvocation : PathElementBase {
        private readonly string methodName;
        public MethodInvocation(string methodName) => this.methodName = methodName;
        public override object? Apply(object target) {
            var mthd = target.GetType().GetMethod(methodName, Instance | Public | NonPublic, null, Type.EmptyTypes, null);
            if (mthd is null) {
                throw new ArgumentException($"The instance method '{methodName}' without any arguments cannot be found.");
            }
            return mthd.Invoke(target, new object[] { });
        }
    }
}
