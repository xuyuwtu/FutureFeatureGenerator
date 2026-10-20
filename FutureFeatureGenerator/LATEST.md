## 1.8.0
### Features
- change option `UseExtensions` to default to `true` when the C# language version is 14 or above
- add option `AutoAddLangType`, automatically marks generated types with the C# language version they require
- add option `DefaultModifier`, sets the default modifier (`internal`/`public`)
- add option `NotWriteGeneratedCodeAttribute`, does not write `GeneratedCodeAttribute`
- add
```
System
    Array
        MaxLength
    Collections.Generic
        List
            EnsureCapacity(int)
        ReferenceEqualityComparer
    Diagnostics.CodeAnalysis
        ConstantExpectedAttribute
        SetsRequiredMembersAttribute
        UnscopedRefAttribute
    IO
        Stream
            ReadExactly(Span<byte>)
            ReadExactly(byte[],int,int)
    Runtime
        CompilerServices
            CollectionBuilderAttribute
            IgnoresAccessChecksToAttribute
            InterceptsLocationAttribute
            IUnion
            ModuleInitializerAttribute
            SkipLocalsInitAttribute
            UnionAttribute
        InteropServices
            LibraryImportAttribute
            StringMarshalling
    Type
        IsAssignableTo(Type)
```
- change
```
;old
System
    Type
        GetConstructor()
        GetMethod()
;new
System
    Type
        GetConstructor(BindingFlags,Type[])
        GetMethod(BindingFlags,Type[])
```
### Bug Fixes
- the option 'DisableAddDependencies' was invalid
- Stream `Read()` and `Write()` condition
