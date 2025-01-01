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

每个类型有版本语言版本要求，在设置的语言版本不达标时不会生成对应代码，每个类型都有各自的`#if`条件编译，还会检测所有引用的程序集中的`public`类型定义，如果名称完全匹配，那么这个类型也不会生成代码

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

提供以下类型和方法:
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