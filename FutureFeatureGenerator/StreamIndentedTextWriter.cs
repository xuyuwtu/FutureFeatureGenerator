using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace FutureFeatureGenerator;

internal class StreamIndentedTextWriter
{
    static readonly byte[] NewLineData = Encoding.UTF8.GetBytes(Environment.NewLine);
    static readonly byte[] TabStringData = Encoding.UTF8.GetBytes(IndentedTextWriter.DefaultTabString);
    private int indentLevel;
    public int Indent
    {
        get => indentLevel;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            indentLevel = value;
        }
    }

    private StringCache _cache;
    private Stream _outStream;

    public StreamIndentedTextWriter(StringCache cache, Stream stream)
    {
        if (!stream.CanSeek || !stream.CanRead || !stream.CanWrite)
        {
            throw new ArgumentNullException(nameof(stream), "stream need CanSeek, CanRead and CanWrite");
        }
        _cache = cache;
        _outStream = stream;
    }
    public void WriteLine(char c)
    {
        if (c > 0x7f)
        {
            throw new ArgumentException($"char '{c}'({(ushort)c:x}) is not ascii", nameof(c));
        }
        OutputTabs();
        _outStream.WriteByte((byte)c);
        _outStream.Write(NewLineData, 0, NewLineData.Length);
    }
    public void WriteLine(string text)
    {
        OutputTabs();
        var buffer = _cache.GetOrAddAsBytes(text);
        _outStream.Write(buffer, 0, buffer.Length);
        _outStream.Write(NewLineData, 0, NewLineData.Length);
    }
    public void WriteLineWithSpace(string modifer, string text)
    {
        OutputTabs();
        var buffer = _cache.GetOrAddAsBytes(modifer);
        _outStream.Write(buffer, 0, buffer.Length);
        _outStream.WriteByte((byte)' ');
        buffer = _cache.GetOrAddAsBytes(text);
        _outStream.Write(buffer, 0, buffer.Length);
        _outStream.Write(NewLineData, 0, NewLineData.Length);
    }
    private void OutputTabs()
    {
        for (int i = 0; i < indentLevel; i++)
        {
            _outStream.Write(TabStringData, 0, TabStringData.Length);
        }
    }
    public void OpenBrace()
    {
        WriteLine('{');
        Indent++;
    }
    public void CloseBrace()
    {
        Indent--;
        WriteLine('}');
    }
    public override string ToString()
    {
        _outStream.Seek(0, SeekOrigin.Begin);
        return new StreamReader(_outStream, _cache.Encoding).ReadToEnd();
    }
}