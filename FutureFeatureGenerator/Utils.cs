using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FutureFeatureGenerator;

internal static class Utils
{
    public static readonly char[] PointSeparator = new char[] { '.' };
    public static readonly char[] SpaceSeparator = new char[] { ' ' };
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
    public static int GetNumberFromSingleLineComment(string s)
    {
        var startIndex = s.IndexOf(' ', "//".Length);
        if (startIndex == -1)
        {
            throw new ArgumentException("invalid syntax");
        }
        var endIndex = s.IndexOf(' ', startIndex + 1);
        if (endIndex == -1)
        {
            return int.Parse(s.Substring(startIndex + 1));
        }
        return int.Parse(s.Substring(startIndex, endIndex - startIndex));
    }
    public static int GetNumberFromSingleLineCommentOrDefault(string s, int defaultValue)
    {
        var startIndex = s.IndexOf(' ', "//".Length);
        if (startIndex == -1)
        {
            return defaultValue;
        }
        if (s.AsSpan(startIndex).IsWhiteSpace())
        {
            return defaultValue;
        }
        var endIndex = s.IndexOf(' ', startIndex + 1);
        if (endIndex == -1)
        {
            return int.Parse(s.Substring(startIndex + 1));
        }
        return int.Parse(s.Substring(startIndex, endIndex - startIndex));
    }
    public static void GetNumbersFromSingleLineComment(string s, out int num1, out int num2)
    {
        var startIndex = s.IndexOf(' ', "//".Length);
        if (startIndex == -1)
        {
            throw new ArgumentException("invalid syntax");
        }
        var result = (stackalloc int[2]);
        for (int i = 0; i < 2; i++)
        {
            var endIndex = s.IndexOf(' ', startIndex + 1);
            result[i] = int.Parse(s.Substring(startIndex, endIndex - startIndex));
            startIndex = endIndex;
        }
        num1 = result[0];
        num2 = result[1];
    }
    public static int[] GetNumbersFromSingleLineComment(string s, int count)
    {
        var startIndex = s.IndexOf(' ', "//".Length);
        if (startIndex == -1)
        {
            throw new ArgumentException("invalid syntax");
        }
        var result = new int[count];
        for (int i = 0; i < count; i++)
        {
            var endIndex = s.IndexOf(' ', startIndex + 1);
            result[i] = int.Parse(s.Substring(startIndex, endIndex - startIndex));
            startIndex = endIndex;
        }
        return result;
    }
    public static StreamReaderWrapper GetWrapper(this StreamReader reader) => new(reader);
    public static Func<string[], bool> GetConditionFunc(string condition)
    {
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
}
