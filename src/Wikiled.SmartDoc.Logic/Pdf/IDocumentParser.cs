using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Pdf
{
    public interface IDocumentParser
    {
        Task<DocumentDefinition> ParseDocument(DirectoryInfo repositoryPath, FileInfo file, CancellationToken token);

        string[] Supported { get; }
    }
}