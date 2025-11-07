using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FutureFeatureGenerator.GeneratorBuild.Test;

class MyText : AdditionalText
{
    public override string Path { get; }
    public string Text { get; }
    private MyText(string text, string path)
    {
        Text = text;
        Path = path;
    }
    public override SourceText? GetText(CancellationToken cancellationToken = default) => SourceText.From(Text);
    public static MyText FromPath(string path) => new MyText(File.ReadAllText(path), path);
    public static MyText From(string text, string path) => new MyText(text, path);
}