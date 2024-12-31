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

Each type has a version language version requirement, and if the set language version is not met, the corresponding code will not be generated. Each type has its own #if conditional compilation, and it will also check for `public` type definitions in all referenced assemblies. If the names match exactly, the code for that type will not be generated.

The default modifier is `internal`. If you want to change it to public, simply add a space and the word `public` after the type you wish to modify.

If the target to be added is a method, the type where the method resides will be named "Future" plus the original type name. If the method is an instance method, `this` will be added as an extension method. If it is a `static` method, it will be added as is.

Provide the following types and methods:
```
System
    Diagnostics.CodeAnalysis
;       8.0
        AllowNullAttribute
        DisallowNullAttribute
        DoesNotReturnAttribute
        DoesNotReturnIfAttribute
;       12.0
        ExperimentalAttribute
;       8.0
        MaybeNullAttribute
        MaybeNullWhenAttribute
;       9.0
        MemberNotNullAttribute
        MemberNotNullWhenAttribute
;       8.0
        NotNullAttribute
        NotNullIfNotNullAttribute
        NotNullWhenAttribute
    IO
        Stream
            ;Read(Span<byte>)
            Read()
            ;Write(ReadOnlySpan<byte>)
            Write()
    Reflection
;       5
        AssemblyMetadataAttribute
    Runtime.CompilerServices
;       10.0
        CallerArgumentExpressionAttribute
;       5
        CallerFilePathAttribute
        CallerLineNumberAttribute
        CallerMemberNameAttribute
;       11.0
        CompilerFeatureRequiredAttribute
;       9.0
        IsExternalInit
;       13.0
        OverloadResolutionPriorityAttribute
;       11.0
        RequiredMemberAttribute
    ; 10.0
    ; FutureArgumentException
    ArgumentException
        ThrowIfNullOrEmpty()
        ThrowIfNullOrWhiteSpace()
    ; FutureArgumentNullException
    ArgumentNullException
        ThrowIfNull()
    ; 10.0
    ArgumentOutOfRangeException
        ThrowIfEqual()
        ThrowIfNotEqual()
        ThrowIfGreaterThan()
        ThrowIfGreaterThanOrEqual()
        ThrowIfLessThan()
        ThrowIfLessThanOrEqual()
;   8.0
    Index
    Range
```