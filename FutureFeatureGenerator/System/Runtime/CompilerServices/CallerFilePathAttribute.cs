﻿// 5
#if (!NETCOREAPP && !NETSTANDARD && !NET45_OR_GREATER)
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class CallerFilePathAttribute : Attribute { }
#endif