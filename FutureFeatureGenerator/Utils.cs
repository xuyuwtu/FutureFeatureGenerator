using Microsoft.CodeAnalysis.CSharp;

namespace FutureFeatureGenerator;

internal static class Utils
{
    public static readonly char[] Separator = new char[] { '.' };
    public static LanguageVersion GetLanguageVersion(string version)
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
            _ => throw new ArgumentException($"invalide version '{version}'"),
        };
    }
}
