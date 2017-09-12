using System.Threading;

namespace Wikiled.SmartDoc.Logic.Learning
{
    public interface IFileManagerFactory
    {
        IFileManager Create(CancellationToken token);
    }
}
