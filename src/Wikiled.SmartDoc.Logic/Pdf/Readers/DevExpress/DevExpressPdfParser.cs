using System.IO;
using System.Text;
using DevExpress.Pdf;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress
{
    public class DevExpressPdfParser : ITextParser
    {
        private readonly int maxPages;

        private readonly FileInfo file;

        public DevExpressPdfParser(FileInfo file, int maxPages)
        {
            Guard.NotNull(() => file, file);
            Guard.IsValid(() => maxPages, maxPages, i => i > 0, "Invalid number of pages");
            this.maxPages = maxPages;
            this.file = file;
        }

        public string Parse()
        {
            StringBuilder builder = new StringBuilder();
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(file.FullName);
                int pages = maxPages > documentProcessor.Document.Pages.Count ? documentProcessor.Document.Pages.Count : maxPages;
                for (int i = 0; i < pages; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(" ");
                    }

                    builder.Append(documentProcessor.GetPageText(i));
                }
            }

            return builder.ToString();
        }
    }
}
