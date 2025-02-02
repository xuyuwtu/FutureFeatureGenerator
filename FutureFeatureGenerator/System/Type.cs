#region
#endregion
namespace System;
internal static partial class FutureType
{
    #region GetConstructor()
    /// <see cref="CSharpFeatureNames.ExtensionMethods"/>
#if !NET6_0_OR_GREATER
    internal static Reflection.ConstructorInfo GetConstructor(this Type self, Reflection.BindingFlags bindingAttr, Type[] types)
    {
        return self.GetConstructor(bindingAttr, null, types, modifiers: null);
    }
#endif
    #endregion

    #region GetMethod()
    /// <see cref="CSharpFeatureNames.ExtensionMethods"/>
#if !NET6_0_OR_GREATER
    internal static Reflection.MethodInfo? GetMethod(this Type self, string name, Reflection.BindingFlags bindingAttr, Type[] types)
    {
        return self.GetMethod(name, bindingAttr, null, types, modifiers: null);
    }
#endif
    #endregion
}