using System;
using System.IO;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress
{
    public class DevExpressParserFactory : ITextParserFactory
    {
        private readonly int maxPages;

        public DevExpressParserFactory(int maxPages)
        {
            Guard.IsValid(() => maxPages, maxPages, i => i > 0, "Invalid number of pages");
            this.maxPages = maxPages;
        }

        public string[] Supported { get; } = { "pdf", "doc", "docx", "rtf", "txt" };

        public ITextParser ConstructParsers(FileInfo file)
        {
            Guard.NotNull(() => file, file);
            if (string.Compare(file.Extension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new DevExpressPdfParser(file, maxPages);
            }

            if (string.Compare(file.Extension, ".doc", StringComparison.OrdinalIgnoreCase) == 0 ||
               string.Compare(file.Extension, ".docx", StringComparison.OrdinalIgnoreCase) == 0 ||
               string.Compare(file.Extension, ".rtf", StringComparison.OrdinalIgnoreCase) == 0 ||
               string.Compare(file.Extension, ".txt", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new RichDocumentParser(file);
            }

            return NullTextParser.Instance;
        }
    }
}
