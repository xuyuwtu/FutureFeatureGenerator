```
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