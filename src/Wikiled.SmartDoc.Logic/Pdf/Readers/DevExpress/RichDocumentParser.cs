using System.IO;
using DevExpress.XtraRichEdit;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress
{
    public class RichDocumentParser : ITextParser
    {
        private readonly FileInfo file;

        public RichDocumentParser(FileInfo file)
        {
            Guard.NotNull(() => file, file);
            this.file = file;
        }

        public string Parse()
        {
            using (var documentProcessor = new RichEditDocumentServer())
            {
                documentProcessor.LoadDocument(file.FullName);
                return documentProcessor.Text;
            }
        }
    }
}
