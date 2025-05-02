Create a file named `FutureFeature.txt` in your project directory. If it doesn't take effect, check the file properties and set it to `C# Analyzer Additional Files`.

File example:
```
System.Diagnostics.CodeAnalysis.AllowNullAttribute
;Comment
System
    Reflection
        AssemblyMetadataAttribute
    Runtime.CompilerServices public
        * 
    Diagnostics.CodeAnalysis
        DisallowNullAttribute public
    IO
        Stream
            Read() public
            Write() public
    ArgumentNullException public
        ThrowIfNull() public
```
You can write the full type name on one line, or you can write it with indentation levels as shown below. When writing to the type name, you can use * to include all types in the current namespace.

Each level of indentation is defined by 1 `Tab` or 4 `Spaces`. Adjacent levels are considered connected by `.`, non-adjacent levels will be skipped.

You can write * on the first line of the file, which will include all types.

Each type has its own #if conditional compilation, and it will also check for `public` type definitions in all referenced assemblies. If the names match exactly, the code for that type will not be generated.

The default modifier is `internal`. If you want to change it to public, simply add a space and the word `public` after the type you wish to modify.

If the target to be added is a method, the type where the method resides will be named "Future" plus the original type name. If the method is an instance method, `this` will be added as an extension method. If it is a `static` method, it will be added as is.

For the convenience of multi-targeting compilation, such as `netstandard2.0;net6`, the compilation condition for `static` methods is set to `true`, so there's no need to write conditional compilation.
```csharp
FutureArgumentNullException.ThrowIfNull(arg);

#if NET6_0_OR_GREATER
ArgumentNullException.ThrowIfNull(arg);
#else
FutureArgumentNullException.ThrowIfNull(arg);
#endif
```

There are two configuration items. `UseExtensions` adds `extensions()` outside `static` methods. `UseRealCondition` uses actual conditions instead of `#if true`. Below is the enabling syntax.
```
@UseExtensions true
@UseRealCondition true
```

Example:
```
@UseExtensions true
@UseRealCondition true
System.ArgumentException
    ThrowIfNullOrEmpty()
    ThrowIfNullOrWhiteSpace()
```

Provide the following types and methods:
```
System
    Diagnostics.CodeAnalysis
        AllowNullAttribute
        DisallowNullAttribute
        DoesNotReturnAttribute
        DoesNotReturnIfAttribute
        ExperimentalAttribute
        MaybeNullAttribute
        MaybeNullWhenAttribute
        MemberNotNullAttribute
        MemberNotNullWhenAttribute
        NotNullAttribute
        NotNullIfNotNullAttribute
        NotNullWhenAttribute
        StringSyntaxAttribute
    IO
        Stream
            ;Read(Span<byte>)
            Read()
            ;Write(ReadOnlySpan<byte>)
            Write()
    Reflection
        AssemblyMetadataAttribute
    Runtime.CompilerServices
        CallerArgumentExpressionAttribute
        CallerFilePathAttribute
        CallerLineNumberAttribute
        CallerMemberNameAttribute
        CompilerFeatureRequiredAttribute
        IsExternalInit
        OverloadResolutionPriorityAttribute
        RequiredMemberAttribute
    ; FutureArgumentException
    ArgumentException
        ThrowIfNullOrEmpty()
        ThrowIfNullOrWhiteSpace()
    ; FutureArgumentNullException
    ArgumentNullException
        ThrowIfNull()
    ArgumentOutOfRangeException
        ThrowIfEqual()
        ThrowIfNotEqual()
        ThrowIfGreaterThan()
        ThrowIfGreaterThanOrEqual()
        ThrowIfLessThan()
        ThrowIfLessThanOrEqual()
    Char
        IsAscii()
        IsBetween()
        IsAsciiDigit()
    Index
    Range
    Type
        GetConstructor()
        GetMethod()
    ObjectDisposedException
        ;ThrowIf()
        ThrowIf(Boolean,Object)
        ThrowIf(Boolean,Type)
```