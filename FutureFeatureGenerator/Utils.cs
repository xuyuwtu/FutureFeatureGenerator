using System.IO;

using Microsoft.CodeAnalysis.CSharp;

namespace FutureFeatureGenerator;

internal static class Utils
{
    public static readonly char[] PointSeparator = new char[] { '.' };
    public static readonly char[] SpaceSeparator = new char[] { ' ' };
    public static LanguageVersion GetLanguageVersion(ReadOnlySpan<char> version)
    {
        return version switch
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
    public static int GetNumberFromSingleLineComment(string s)
    {
        var startIndex = s.IndexOf(' ', "//".Length);
        if(startIndex == -1)
        {
            throw new ArgumentException("invalid syntax");
        }
        var endIndex = s.IndexOf(' ', startIndex + 1);
        if(endIndex == -1)
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
}
