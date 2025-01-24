// 7 GetConstructor()
// 15 GetMethod()

namespace System;
internal static partial class FutureType
{
    // 8.0
#if !NET6_0_OR_GREATER
    internal static Reflection.ConstructorInfo GetConstructor(this Type self, Reflection.BindingFlags bindingAttr, Type[] types)
    {
        return self.GetConstructor(bindingAttr, binder: null, types, modifiers: null);
    }
#endif

    // 8.0
#if !NET6_0_OR_GREATER
    internal static Reflection.MethodInfo? GetMethod(this Type self, string name, Reflection.BindingFlags bindingAttr, Type[] types)
    {
        return self.GetMethod(name, bindingAttr, binder: null, types, modifiers: null);
    }
#endif
}