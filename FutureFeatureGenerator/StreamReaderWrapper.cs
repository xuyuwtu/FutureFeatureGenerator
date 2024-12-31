using System.IO;

namespace FutureFeatureGenerator;

internal struct StreamReaderWrapper
{
    public StreamReader Reader;
    private int _currentLine = 1;
    public readonly int CurrentLine => _currentLine;
    public readonly bool EndOfStream => Reader.EndOfStream;
    public StreamReaderWrapper(StreamReader reader) => Reader = reader;
    public string ReadLine()
    {
        if (!Reader.EndOfStream)
        {
            _currentLine++;
        }
        return Reader.ReadLine();
    }
}
