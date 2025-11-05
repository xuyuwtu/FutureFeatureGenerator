在你的项目目录下创建文件`FutureFeature.txt`，如果不生效，查看文件属性，设置为`C# 分析器其他文件`。

文件示例
```
System.Diagnostics.CodeAnalysis.AllowNullAttribute
;注释
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

可以一行一次性写完整类型名称，也可以像下面一样按缩进层级写，写到类型名时可以用`*`，会加入当前命名空间下所有类型

每层缩进由1个`Tab`或4个`Space`定义，相邻的两层之间被认为由`.`连接，不相邻的两层会被跳过

可以在文件第一行写`*`，那么会加入所有类型

每个类型都有各自的`#if`条件编译

默认修饰符为`internal`，如果想修改为public，那么在要添加的类型后面加上一个空格和`public`

如果添加目标是方法，那么方法所在类型会命名为Future + 原类型名，如果方法是实例方法，会添加`this`作为扩展方法，如果是静态方法，那原样添加

为了方便多目标编译，比如`netstandard2.0;net6`，`static`方法的编译条件为`true`，这样就不用写条件编译了
```csharp
FutureArgumentNullException.ThrowIfNull(arg);

#if NET6_0_OR_GREATER
ArgumentNullException.ThrowIfNull(arg);
#else
FutureArgumentNullException.ThrowIfNull(arg);
#endif
```

有三个配置项，默认都为`false`
`UseExtensions`会在`static`方法外加上`extensions()`  
`UseRealCondition`则会使用实际的条件而不是`#if true`，下面是启用语法  
`DisableAddDependencies`取消自动添加依赖类型和方法
```
@UseExtensions true
@UseRealCondition true
@DisableAddDependencies true
```

例如:
```
@UseExtensions true
@UseRealCondition true
System.ArgumentException
    ThrowIfNullOrEmpty()
    ThrowIfNullOrWhiteSpace()
```
可以直接无缩进写最终节点名称。(如果找到多个，会都加入进来)
例如：
```
System.Runtime.CompilerServices
    IsExternalInit
;和下面效果一样
IsExternalInit
```
提供以下类型和方法:
```
System
    Collections.Generic
        CollectionExtensions
            AsReadOnly(IList<T>)
            AsReadOnly(IDictionary<TKey,TValue>)
            GetValueOrDefault(IReadOnlyDictionary<TKey,TValue>,TKey)
            GetValueOrDefault(IReadOnlyDictionary<TKey,TValue>,TKey,TValue)
            Remove(IDictionary<TKey,TValue>,TKey,out-TValue)
            TryAdd(IDictionary<TKey,TValue>,TKey,TValue)
        Dictionary
            TryAdd()
        KeyValuePair
            Deconstruct()
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
            ;Read()
            Read(Span<byte>)
            ;Write()
            Write(ReadOnlySpan<byte>)
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
    BitConverter
        ToBoolean()
        ToChar()
        ToDouble()
        ToInt16()
        ToInt32()
        ToInt64()
        ToSingle()
        ToUInt16()
        ToUInt32()
        ToUInt64()
        ;TryWriteBytes()
        TryWriteBytes(bool)
        TryWriteBytes(char)
        TryWriteBytes(double)
        TryWriteBytes(short)
        TryWriteBytes(int)
        TryWriteBytes(long)
        TryWriteBytes(float)
        TryWriteBytes(ushort)
        TryWriteBytes(uint)
        TryWriteBytes(ulong)
    Char
        IsAscii()
        IsBetween()
        IsAsciiDigit()
    Double
        IsFinite()
        IsNegative()
        IsNormal()
        IsSubnormal()
    Index
    Math
        Clamp(byte,byte,byte)
        Clamp(decimal,decimal,decimal)
        Clamp(double,double,double)
        Clamp(short,short,short)
        Clamp(int,int,int)
        Clamp(long,long,long)
        Clamp(sbyte,sbyte,sbyte)
        Clamp(float,float,float)
        Clamp(ushort,ushort,ushort)
        Clamp(uint,uint,uint)
        Clamp(ulong,ulong,ulong)
        Clamp(nint,nint,nint)
        Clamp(nuint,nuint,nuint)
    ObjectDisposedException
        ;ThrowIf()
        ThrowIf(Boolean,Object)
        ThrowIf(Boolean,Type)
    Range
    Single
        IsFinite()
        IsNegative()
        IsNormal()
        IsSubnormal()
    Type
        GetConstructor()
        GetMethod()
```