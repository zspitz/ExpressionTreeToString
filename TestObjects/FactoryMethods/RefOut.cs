//using System.Linq.Expressions;
//using static ExpressionTreeTestObjects.Categories;
//using static System.Linq.Expressions.Expression;
//using static ExpressionTreeTestObjects.Functions;

namespace ExpressionTreeTestObjects {
    //internal delegate int DelegateWithOut(out int i);
    //internal delegate int DelegateWithRef(ref int i);

    //partial class FactoryMethods {
    //    [TestObject(RefOut)]
    //    internal static readonly Expression OutParameter = IIFE(() => {
    //        var prm = Parameter(typeof(int).MakeByRefType());
    //        return Lambda(
    //            typeof(DelegateWithOut),
    //            prm,
    //            prm
    //        );
    //    });

    //    [TestObject(RefOut)]
    //    internal static readonly Expression RefParameter = IIFE(() => {
    //        var prm = Parameter(typeof(int).MakeByRefType());
    //        return Lambda(
    //            typeof(DelegateWithRef),
    //            prm,
    //            prm
    //        );
    //    });

    //    [TestObject(RefOut)]
    //    internal static readonly Expression PassRef = 
    //        Call(
    //            typeof(Dummy).GetMethod("DummyMethodWithRef"),
    //            Parameter(typeof(int))
    //        );

    //    [TestObject(RefOut)]
    //    internal static readonly Expression PassOut =
    //        Call(
    //            typeof(Dummy).GetMethod("DummyMethodWithOut"),
    //            Parameter(typeof(int))
    //        );

    //    [TestObject(RefOut)]
    //    internal static readonly Expression PassRefField =
    //        Call(
    //            typeof(Dummy).GetMethod("DummyMethodWithRef"),
    //            PropertyOrField(
    //                Constant(new Dummy2()),
    //                "Data"
    //            )
    //        );

    //    [TestObject(RefOut)]
    //    internal static readonly Expression PassOutField =
    //        Call(
    //            typeof(Dummy).GetMethod("DummyMethodWithOut"),
    //            PropertyOrField(
    //                Constant(new Dummy2()),
    //                "Data"
    //            )
    //        );
    //}
}
