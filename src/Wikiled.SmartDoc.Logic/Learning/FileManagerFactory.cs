using System.Threading;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Logic.Pdf;

namespace Wikiled.SmartDoc.Logic.Learning
{
    public class FileManagerFactory : IFileManagerFactory
    {
        private readonly IDocumentParser parser;

        public FileManagerFactory(IDocumentParser parser)
        {
            Guard.NotNull(() => parser, parser);
            this.parser = parser;
        }

        public IFileManager Create(CancellationToken token)
        {
            return new FileManager(parser, token, 4);
        }
    }
}