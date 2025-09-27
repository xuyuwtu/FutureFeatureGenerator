using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FutureFeatureGenerator;

internal static class Utils
{
    public static readonly char[] PointSeparator = ['.'];
    public static readonly char[] SpaceSeparator = [' '];
    private static char[] GeneratedCodeAttributeNamespaceChars = "System.CodeDom.Compiler".ToCharArray();
    public static bool SkipWhileSpaceFirstCharIs(ReadOnlySpan<char> text, char chr)
    {
        foreach(var c in text)
        {
            if (!char.IsWhiteSpace(c))
            {
                return chr == c;
            }
        }
        return false;
    }
    public static List<(int start, int length)> Split(this ReadOnlySpan<char> self, params ReadOnlySpan<char> separator)
    {
        var result = new List<(int, int)>();
        var start = 0;
        while (true)
        {
            var sub = self.Slice(start);
            if (sub.Length == 0)
            {
                break;
            }
            var length = sub.IndexOfAny(separator);
            if (length == -1)
            {
                result.Add((start, sub.Length));
                break;
            }
            if (length == 0)
            {
                start++;
                continue;
            }
            result.Add((start, length));
            start += length + 1;
        }
        return result;
    }
    public static List<(int start, int length)> Split(this ReadOnlySpan<char> self, Func<char, bool> func)
    {
        var result = new List<(int, int)>();
        var start = 0;
        while (true)
        {
            var sub = self.Slice(start);
            if (sub.Length == 0)
            {
                break;
            }
            var length = sub.IndexOf(func);
            if (length == -1)
            {
                result.Add((start, sub.Length));
                break;
            }
            if (length == 0)
            {
                start++;
                continue;
            }
            result.Add((start, length));
            start += length + 1;
        }
        return result;
    }
    public static int IndexOf<T>(this ReadOnlySpan<T> self, Func<T, bool> func)
    {
        var count = self.Length;
        for (int i = 0; i < count; i++)
        {
            if (func(self[i]))
            {
                return i;
            }
        }
        return -1;
    }
    public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> self, (int start, int length) tuple) => self.Slice(tuple.start, tuple.length);
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IReadOnlyCollection<T>? self)
    {
        if(self is null)
        {
            return true;
        }
        return self.Count == 0;
    }
    public static StreamReaderWrapper GetWrapper(this StreamReader reader) => new(reader);
    public static Func<string[], bool> GetConditionFunc(string condition)
    {
        if (string.IsNullOrEmpty(condition))
        {
            return static _ => true;
        }
        var syntaxTree = CSharpSyntaxTree.ParseText(
               $"""
                #if {condition}
                #endif
                """);
        var ifDirectiveTriviaSyntax = syntaxTree.GetRoot().DescendantNodes(null, true).OfType<IfDirectiveTriviaSyntax>().First();
        var parameterExpression = Expression.Parameter(typeof(string[]), "preprocessorSymbolNames");
        var bodyExpression = GetExpression(ifDirectiveTriviaSyntax.Condition, parameterExpression);
        Func<string[], bool> func;
        if (bodyExpression is ConstantExpression constantExpression && constantExpression.Type == typeof(bool))
        {
            func = (bool)constantExpression.Value! ? static (string[] names) => true : static (string[] names) => false;
        }
        else
        {
            func = Expression.Lambda<Func<string[], bool>>(bodyExpression, parameterExpression).Compile();
        }
        return func;
    }
    static Expression GetExpression(ExpressionSyntax expression, ParameterExpression parameterExpression)
    {
        switch (expression)
        {
            case PrefixUnaryExpressionSyntax prefixUnaryExpressionSyntax:
                return prefixUnaryExpressionSyntax.Kind() switch
                {
                    SyntaxKind.LogicalNotExpression => Expression.Not(GetExpression(prefixUnaryExpressionSyntax.Operand, parameterExpression)),
                    _ => throw new InvalidDataException(prefixUnaryExpressionSyntax.Kind().ToString()),
                };
            case ParenthesizedExpressionSyntax parenthesizedExpressionSyntax:
                return GetExpression(parenthesizedExpressionSyntax.Expression, parameterExpression);
            case BinaryExpressionSyntax binaryExpressionSyntax:
                return binaryExpressionSyntax.Kind() switch
                {
                    SyntaxKind.LogicalAndExpression => Expression.And(GetExpression(binaryExpressionSyntax.Left, parameterExpression), GetExpression(binaryExpressionSyntax.Right, parameterExpression)),
                    SyntaxKind.LogicalOrExpression => Expression.Or(GetExpression(binaryExpressionSyntax.Left, parameterExpression), GetExpression(binaryExpressionSyntax.Right, parameterExpression)),
                    _ => throw new InvalidDataException(binaryExpressionSyntax.Kind().ToString()),
                };
            case IdentifierNameSyntax identifierNameSyntax:
                Expression<Func<string[], bool>> func = names => names.Contains("1");
                return Expression.Call(((MethodCallExpression)func.Body).Method, parameterExpression, Expression.Constant(identifierNameSyntax.Identifier.Text, typeof(string)));
            case LiteralExpressionSyntax:
                return expression.Kind() switch
                {
                    SyntaxKind.TrueLiteralExpression => Expression.Constant(true, typeof(bool)),
                    SyntaxKind.FalseLiteralExpression => Expression.Constant(false, typeof(bool)),
                    _ => throw new InvalidDataException(expression.Kind().ToString()),
                };
            default:
                throw new InvalidDataException(expression.GetType().FullName);
        }
    }
    private static Regex nameofValueRegex = new Regex(@"nameof\((.*?)\)");
    public static NodeMethod GetMethodNode(string name, StreamReaderWrapper readerWrapper, int skipCount, int startLine)
    {
        const int ReadDependencies = 0, ReadCondition = 1, ReadLine = 2, EndCondition = 3;
        var dependencies = new List<object>();
        var modifierLineIndex = -1;
        string? aliasName = null;
        string? condition = null;
        var lines = new List<string>();
        var state = 0;
        var isStatic = false;
        while (readerWrapper.CurrentLine < startLine)
        {
            readerWrapper.ReadLine();
        }
        string? text;
        string? realCondition = null;
        var ifCount = 0;
        while (!readerWrapper.EndOfStream)
        {
            text = readerWrapper.ReadLine();
            if (string.IsNullOrEmpty(text))
            {
                continue;
            }
            switch (state)
            {
                case ReadDependencies:
                    var dependencyText = text.AsSpan(skipCount);
                    if (dependencyText.StartsWith("//".AsSpan()))
                    {
                        dependencies.Add(text.AsSpan(skipCount + "//".Length).Trim().ToString());
                    }
                    else if (dependencyText.StartsWith("[RequireType".AsSpan()))
                    {
                        dependencies.Add(nameofValueRegex.Match(text).Groups[1].Value);
                    }
                    else if (dependencyText.StartsWith("[Alias".AsSpan()))
                    {
                        aliasName = nameofValueRegex.Match(text).Groups[1].Value + "()";
                    }
                    else if (dependencyText.StartsWith("[RealCondition".AsSpan()))
                    {
                        realCondition = Regex.Match(text, "\"(.*?)\"").Groups[1].Value;
                    }
                    else
                    {
                        state++;
                        goto case ReadCondition;
                    }
                    break;
                case ReadCondition:
                    condition = text.Substring("#if".Length).Trim();
                    if (realCondition is not null)
                    {
                        condition = realCondition;
                    }
                    ifCount = 1;
                    state++;
                    break;
                case ReadLine:
                    if (text.StartsWith("#if"))
                    {
                        ifCount++;
                        lines.Add(text);
                    }
                    else if (text.StartsWith("#endif"))
                    {
                        if (--ifCount == 0)
                        {
                            state++;
                            goto case EndCondition;
                        }
                        else
                        {
                            lines.Add(text);
                        }
                    }
                    else
                    {
                        // #else
                        if (text[0] == '#')
                        {
                            lines.Add(text);
                        }
                        else
                        {
                            var textSpan = text.AsSpan(skipCount);
                            if (modifierLineIndex == -1)
                            {
                                if (textSpan.StartsWith(Modifiers.Internal.AsSpan()))
                                {
                                    textSpan = textSpan.Slice(Modifiers.Internal.Length).Trim();
                                    modifierLineIndex = lines.Count;
                                }
                                if (textSpan.IndexOf("(this".AsSpan()) == -1)
                                {
                                    isStatic = true;
                                }
                            }
                            lines.Add(textSpan.ToString());
                        }

                    }
                    break;
                case EndCondition:
                    if (text.Trim() != "#endif")
                    {
                        throw new InvalidDataException("the last line must be '#endif'");
                    }
                    goto endRead;
                default:
                    throw new NotImplementedException("unknown state");
            }
        }
    endRead:
        if (state != EndCondition)
        {
            throw new InvalidDataException("not found '^#endif'");
        }
        if (string.IsNullOrEmpty(condition))
        {
            throw new ArgumentException("Condition cannot be null");
        }
        // resolve null warn
        condition ??= "";
        return new NodeMethod([.. lines], condition, name) { AliasName = aliasName, Dependencies = [.. dependencies], ModifierLineIndex = modifierLineIndex, IsStatic = isStatic };
    }
    public static TValue? TryGet<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key) where TValue : class
    {
        if(self.TryGetValue(key, out var value))
        {
            return value;
        }
        return default;
    }
    private static int GetNotEqualIndex(string findNs, char[] baseNs)
    {
        var count = Math.Min(findNs.Length, baseNs.Length);
        int i = 0;
        for(; i < count; i++)
        {
            if (findNs[i] != baseNs[i])
            {
                break;
            }
        }
        return i;
    }
    public static void WriteGeneratedCodeAttribute(IndentedTextWriter writer, string currentNamespace)
    {
        var index = GetNotEqualIndex(currentNamespace, GeneratedCodeAttributeNamespaceChars);
        writer.Write('[');
        if(index == 0)
        {
            writer.Write(GeneratedCodeAttributeNamespaceChars);
        }
        else
        {
            // skip '.' for example 'System'
            if (index == currentNamespace.Length)
            {
                index++;
            }
            writer.Write(GeneratedCodeAttributeNamespaceChars, index, GeneratedCodeAttributeNamespaceChars.Length - index);
        }
        writer.WriteLine($".{nameof(GeneratedCodeAttribute)}(\"{nameof(FeatureGenerator)}\", \"{FeatureGenerator.Version}\")]");
    }
}
