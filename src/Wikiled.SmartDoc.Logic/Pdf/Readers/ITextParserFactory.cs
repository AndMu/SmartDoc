using System.IO;

namespace Wikiled.SmartDoc.Logic.Pdf.Readers
{
    public interface ITextParserFactory
    {
        ITextParser ConstructParsers(FileInfo file);

        string[] Supported { get; }
    }
}
