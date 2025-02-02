using System.IO;
using System.Linq.Expressions;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FutureFeatureGenerator;

internal static class Utils
{
    public static readonly char[] PointSeparator = new char[] { '.' };
    public static readonly char[] SpaceSeparator = new char[] { ' ' };
    public static LanguageVersion GetLanguageVersion(ReadOnlySpan<char> version)
    {
        return GetLanguageVersionByFeature(version) ?? version switch
        {
            "1" => LanguageVersion.CSharp1,
            "2" => LanguageVersion.CSharp2,
            "3" => LanguageVersion.CSharp3,
            "4" => LanguageVersion.CSharp4,
            "5" => LanguageVersion.CSharp5,
            "6" => LanguageVersion.CSharp6,
            "7.0" => LanguageVersion.CSharp7,
            "7.1" => LanguageVersion.CSharp7_1,
            "7.2" => LanguageVersion.CSharp7_2,
            "7.3" => LanguageVersion.CSharp7_3,
            "8.0" => LanguageVersion.CSharp8,
            "9.0" => LanguageVersion.CSharp9,
            "10.0" => (LanguageVersion)1000,
            "11.0" => (LanguageVersion)1100,
            "12.0" => (LanguageVersion)1200,
            "13.0" => (LanguageVersion)1300,
            _ => throw new ArgumentException($"invalide version '{version.ToString()}'"),
        };
    }
    private static LanguageVersion? GetLanguageVersionByFeature(ReadOnlySpan<char> feature)
    {
        return feature switch
        {
            CSharpFeatureNames.None => LanguageVersion.CSharp1,
            CSharpFeatureNames.GenericSupport => LanguageVersion.CSharp2,
            CSharpFeatureNames.PartialClasses => LanguageVersion.CSharp2,
            CSharpFeatureNames.AutomaticProperties => LanguageVersion.CSharp3,
            CSharpFeatureNames.ExtensionMethods => LanguageVersion.CSharp3,
            CSharpFeatureNames.InModifiers => LanguageVersion.CSharp7_2,
            CSharpFeatureNames.IntroduceReadonly => LanguageVersion.CSharp7_2,
            CSharpFeatureNames.NullableReferenceTypes => LanguageVersion.CSharp8,
            _ => null
        };
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
