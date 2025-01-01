using System.IO;

namespace FutureFeatureGenerator;

internal class StreamReaderWrapper
{
    public StreamReader Reader;
    private int _currentLine = 1;
    public int CurrentLine => _currentLine;
    public bool EndOfStream => Reader.EndOfStream;
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
