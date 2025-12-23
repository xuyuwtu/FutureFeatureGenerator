#region
#endregion
namespace System;
internal static partial class FutureType
{
    #region GetConstructor(BindingFlags,Type[])
    [Alias(nameof(GetConstructor))]
#if !NET6_0_OR_GREATER
    internal static Reflection.ConstructorInfo GetConstructor(this Type self, Reflection.BindingFlags bindingAttr, Type[] types)
    {
        return self.GetConstructor(bindingAttr, null, types, modifiers: null);
    }
#endif
    #endregion

    #region GetMethod(BindingFlags,Type[])
    [Alias(nameof(GetMethod))]
#if !NET6_0_OR_GREATER
    internal static Reflection.MethodInfo? GetMethod(this Type self, string name, Reflection.BindingFlags bindingAttr, Type[] types)
    {
        return self.GetMethod(name, bindingAttr, null, types, modifiers: null);
    }
#endif
    #endregion

    #region IsAssignableTo(Type)
    [Alias(nameof(IsAssignableTo))]
    [RequireType(nameof(System.Diagnostics.CodeAnalysis.NotNullWhenAttribute))]
#if !NET5_0_OR_GREATER
    internal static bool IsAssignableTo(this Type self, [Diagnostics.CodeAnalysis.NotNullWhen(true)] Type? targetType)
    {
        return targetType?.IsAssignableFrom(self) ?? false;
    }
#endif
    #endregion
}