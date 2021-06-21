using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionType;

namespace ExpressionTreeToString {
    public enum BuiltinRenderer {
        CSharp,
        VisualBasic,
        FactoryMethods,
        ObjectNotation,
        TextualTree,
        DebugView,
        ToString,
        DynamicLinq
    }

    public static class RendererNames {
        public const string CSharp = "C#";
        public const string VisualBasic = "Visual Basic";
        public const string FactoryMethods = "Factory methods";
        public const string ObjectNotation = "Object notation";
        public const string DebugView = "DebugView";
        public const string TextualTree = "Textual tree";
        public const string ToStringRenderer = "ToString";
        public const string DynamicLinq = "Dynamic LINQ";
    }

    public static class Globals {
        public static readonly List<Type> NodeTypes = new() {
            typeof(Expression),
            typeof(MemberBinding),
            typeof(ElementInit),
            typeof(SwitchCase),
            typeof(CatchBlock),
            typeof(LabelTarget)
        };

        public static readonly List<Type> PropertyTypes = NodeTypes.Select(x => typeof(IEnumerable<>).MakeGenericType(x)).ToList();

        public static readonly Dictionary<ExpressionType, string> FactoryMethodNames = new() {
            { Add, "Add" },
            { AddAssign, "AddAssign" },
            { AddAssignChecked, "AddAssignChecked" },
            { AddChecked, "AddChecked" },
            { And, "And" },
            { AndAlso, "AndAlso" },
            { AndAssign, "AndAssign" },
            { ArrayIndex, "ArrayIndex" },
            { ArrayLength, "ArrayLength" },
            { Assign, "Assign" },
            { Block, "Block" },
            { Call, "Call" },
            { Coalesce, "Coalesce" },
            { Conditional, "Conditional" },
            { Constant, "Constant" },
            { ExpressionType.Convert, "Convert" },
            { ConvertChecked, "ConvertChecked" },
            { DebugInfo, "DebugInfo" },
            { Decrement, "Decrement" },
            { Default, "Default" },
            { Divide, "Divide" },
            { DivideAssign, "DivideAssign" },
            { Dynamic, "Dynamic" },
            { Equal, "Equal" },
            { ExclusiveOr, "ExclusiveOr" },
            { ExclusiveOrAssign, "ExclusiveOrAssign" },
            { Extension, "Extension" },
            { Goto, "Goto" },
            { GreaterThan, "GreaterThan" },
            { GreaterThanOrEqual, "GreaterThanOrEqual" },
            { Increment, "Increment" },
            { ExpressionType.Index, "Index" },
            { Invoke, "Invoke" },
            { IsFalse, "IsFalse" },
            { IsTrue, "IsTrue" },
            { Label, "Label" },
            { Lambda, "Lambda" },
            { LeftShift, "LeftShift" },
            { LeftShiftAssign, "LeftShiftAssign" },
            { LessThan, "LessThan" },
            { LessThanOrEqual, "LessThanOrEqual" },
            { ListInit, "ListInit" },
            { Loop, "Loop" },
            { MemberAccess, "MemberAccess" },
            { MemberInit, "MemberInit" },
            { Modulo, "Modulo" },
            { ModuloAssign, "ModuloAssign" },
            { Multiply, "Multiply" },
            { MultiplyAssign, "MultiplyAssign" },
            { MultiplyAssignChecked, "MultiplyAssignChecked" },
            { MultiplyChecked, "MultiplyChecked" },
            { Negate, "Negate" },
            { NegateChecked, "NegateChecked" },
            { New, "New" },
            { NewArrayBounds, "NewArrayBounds" },
            { NewArrayInit, "NewArrayInit" },
            { Not, "Not" },
            { NotEqual, "NotEqual" },
            { OnesComplement, "OnesComplement" },
            { Or, "Or" },
            { OrAssign, "OrAssign" },
            { OrElse, "OrElse" },
            { Parameter, "Parameter" },
            { PostDecrementAssign, "PostDecrementAssign" },
            { PostIncrementAssign, "PostIncrementAssign" },
            { Power, "Power" },
            { PowerAssign, "PowerAssign" },
            { PreDecrementAssign, "PreDecrementAssign" },
            { PreIncrementAssign, "PreIncrementAssign" },
            { Quote, "Quote" },
            { RightShift, "RightShift" },
            { RightShiftAssign, "RightShiftAssign" },
            { RuntimeVariables, "RuntimeVariables" },
            { Subtract, "Subtract" },
            { SubtractAssign, "SubtractAssign" },
            { SubtractAssignChecked, "SubtractAssignChecked" },
            { SubtractChecked, "SubtractChecked" },
            { Switch, "Switch" },
            { Throw, "Throw" },
            { Try, "Try" },
            { TypeAs, "TypeAs" },
            { TypeEqual, "TypeEqual" },
            { TypeIs, "TypeIs" },
            { UnaryPlus, "UnaryPlus" },
            { Unbox, "Unbox" },
        };

        public static readonly List<(Type type, string[] propertyNames)> PreferredPropertyOrders = new() {
            (typeof(LambdaExpression), new [] {"Parameters", "Body" } ),
            (typeof(BinaryExpression), new [] {"Left", "Right", "Conversion"}),
            (typeof(BlockExpression), new [] { "Variables", "Expressions"}),
            (typeof(CatchBlock), new [] { "Variable", "Filter", "Body"}),
            (typeof(ConditionalExpression), new [] { "Test", "IfTrue", "IfFalse"}),
            (typeof(IndexExpression), new [] { "Object", "Arguments" }),
            (typeof(InvocationExpression), new [] {"Arguments", "Expression"}),
            (typeof(ListInitExpression), new [] {"NewExpression", "Initializers"}),
            (typeof(MemberInitExpression), new [] {"NewExpression", "Bindings"}),
            (typeof(MethodCallExpression), new [] {"Object", "Arguments"}),
            (typeof(SwitchCase), new [] {"TestValues", "Body"}),
            (typeof(SwitchExpression), new [] {"SwitchValue", "Cases", "DefaultBody"}),
            (typeof(TryExpression), new [] {"Body", "Handlers", "Finally", "Fault"}),
            (typeof(DynamicExpression), new [] {"Binder", "Arguments"})
        };

        public static readonly HashSet<ExpressionType> RelationalOperators = new() {
            Equal,
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual,
            NotEqual
        };

        public static readonly HashSet<ExpressionType> BinaryExpressionTypes = new() {
            Add, AddChecked, Divide, Modulo, Multiply, MultiplyChecked, Power, Subtract, SubtractChecked,   // mathematical operators
            And, Or, ExclusiveOr,   // bitwise / logical operations
            LeftShift, RightShift,     // shift operators
            AndAlso, OrElse,        // short-circuit boolean operators
            Equal, NotEqual, GreaterThanOrEqual, GreaterThan,LessThan,LessThanOrEqual,     // comparison operators
            Coalesce,
            ArrayIndex,

            Assign,
            AddAssign, AddAssignChecked,DivideAssign, ModuloAssign,MultiplyAssign, MultiplyAssignChecked, PowerAssign, SubtractAssign, SubtractAssignChecked,
            AndAssign, OrAssign, ExclusiveOrAssign,
            LeftShiftAssign,RightShiftAssign
        };

        public static readonly HashSet<ExpressionType> UnaryExpressionTypes = new() {
            ArrayLength, ExpressionType.Convert, ConvertChecked, Unbox, Negate, NegateChecked, Not, Quote, TypeAs, UnaryPlus, IsTrue, IsFalse,
            PreIncrementAssign, PreDecrementAssign, PostIncrementAssign, PostDecrementAssign,
            Increment, Decrement,
            Throw
        };

        public static readonly HashSet<string> VBKeywords = new(StringComparer.InvariantCultureIgnoreCase) {
            "AddHandler", "AddressOf", "Alias", "And",
            "AndAlso", "As", "Boolean", "ByRef",
            "Byte", "ByVal", "Call", "Case",
            "Catch", "CBool", "CByte", "CChar",
            "CDate", "CDbl", "CDec", "Char",
            "CInt", "Class", "CLng",
            "CObj", "Const", "Continue", "CSByte",
            "CShort", "CSng", "CStr", "CType",
            "CUInt", "CULng", "CUShort", "Date",
            "Decimal", "Declare", "Default", "Delegate",
            "Dim", "DirectCast", "Do", "Double",
            "Each", "Else", "ElseIf", "End",
            "EndIf", "Enum", "Erase",
            "Error", "Event", "Exit", "False",
            "Finally", "For", "", "Friend",
            "Function", "Get", "GetType", "GetXMLNamespace",
            "Global", "GoSub", "GoTo", "Handles",
            "If", "Implements",
            "Imports", "In",
            "Inherits", "Integer", "Interface", "Is",
            "IsNot", "Let", "Lib", "Like",
            "Long", "Loop", "Me", "Mod",
            "Module", "Module Statement", "MustInherit", "MustOverride",
            "MyBase", "MyClass", "NameOf", "Namespace",
            "Narrowing", "New", "Next",
            "Not", "Nothing", "NotInheritable",
            "NotOverridable", "Object", "Of", "On",
            "Operator", "Option", "Optional", "Or",
            "OrElse", "Overloads", "Overridable",
            "Overrides", "ParamArray", "Partial", "Private",
            "Property", "Protected", "Public", "RaiseEvent",
            "ReadOnly", "ReDim", "REM", "RemoveHandler",
            "Resume", "Return", "SByte", "Select",
            "Set", "Shadows", "Shared", "Short",
            "Single", "Static", "Step", "Stop",
            "String", "Structure", "Sub",
            "SyncLock", "Then", "Throw", "To",
            "True", "Try", "TryCast", "TypeOf",
            "UInteger", "ULong", "UShort", "Using",
            "Variant", "Wend", "When", "While",
            "Widening", "With", "WithEvents", "WriteOnly",
            "Xor"
        };
    }
}
