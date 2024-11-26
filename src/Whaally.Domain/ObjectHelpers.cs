using System.Reflection;
using System.Reflection.Emit;

namespace Whaally.Domain;

internal static class ObjectHelpers
{
    // Source: https://stackoverflow.com/a/23433748/1720761
    internal static ObjectActivator CreateCtor(Type type)
    {
        if (type == null)
            throw new NullReferenceException("type");

        ConstructorInfo emptyConstructor = type.GetConstructor(Type.EmptyTypes)!;
        var dynamicMethod = new DynamicMethod("CreateInstance", type, Type.EmptyTypes, true);
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Nop);
        ilGenerator.Emit(OpCodes.Newobj, emptyConstructor);
        ilGenerator.Emit(OpCodes.Ret);
        return (ObjectActivator)dynamicMethod.CreateDelegate(typeof(ObjectActivator));
    }
    
    internal delegate object ObjectActivator();
}
