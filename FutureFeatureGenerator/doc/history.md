# 变更

## v1.7.0
### 添加
```
System
    Collections.Generic
        KeyValuePair
    Runtime
        CompilerServices
            RawArrayData
            RawData
        InteropServices
            MemoryMarshal
                GetArrayDataReference(T[])
```
### 修改
```
;old
System.Collections.Generic
    KeyValuePair
        Deconstruct()
;new
System.Collections.Generic
    KeyValuePair`2
        Deconstruct()
```

## v1.6.0
### 修复
- 生成GeneratedCodeAttribute时对于System.Collections.Generic命名空间错误的问题
### 添加
```
System
    ArraySegment
        CopyTo(T[])
        CopyTo(T[],int)
        CopyTo(ArraySegment<T>)
        Slice(int)
        Slice(int,int)
        ToArray()
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
    String
        EndsWith(char) 
        IndexOf(char,StringComparison) 
        GetHashCode(StringComparison) 
        StartsWith(char) 
    StringComparer
        FromComparison(StringComparison)
```
### 修改
- 改变了下UseRealCondition的行为
- ObjectDisposedException.ThrowIf的参数类型改为csharp关键字
## v1.5.0
### 添加
```
System.Collections.Generic.CollectionExtensions
    AsReadOnly(IList<T>)
    AsReadOnly(IDictionary<TKey,TValue>)
    GetValueOrDefault(IReadOnlyDictionary<TKey,TValue>,TKey)
    GetValueOrDefault(IReadOnlyDictionary<TKey,TValue>,TKey,TValue)
    Remove(IDictionary<TKey,TValue>,TKey,out-TValue)
    TryAdd(IDictionary<TKey,TValue>,TKey,TValue)
```
### 其他
- 现在会额外生成 GeneratedCodeAttribute
- 尝试修复多线程环境下(例如VS项目里项目和引用的项目都引用了生成器)生成可能会报错的问题
```
v1.4.0
添加 DisableAddDependencies 选项
添加 无缩进直接搜索终端节点
修复 extensions 会加到命名空间上的问题
删除 和程序集类型对比


v1.3.0
添加extensions支持

v1.2.0
添加 DevelopmentDependency防止传递
修复 无`FutureFeature.txt`时会生成报错
解决 在VisualStudio中点击'重新运行生成器'后报错的问题
添加一些内容

v1.1.1
移除设置依赖的修饰符

v1.1.0
修改注册函数 RegisterPostInitializationOutput => RegisterSourceOutput 以贴合IDE开发环境
添加注册比较以减少运行次数
添加一些ArgumentException的static方法和Stream的扩展方法Read(Span<byte>)和Write(ReadOnlySpan<byte>)
添加修饰符更改
注释符号 ; 不再限于本行第一个字符，变成空格和tab后的第一个字符
```