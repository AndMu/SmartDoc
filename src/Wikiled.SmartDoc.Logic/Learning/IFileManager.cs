using System;
using System.IO;
using System.Threading.Tasks;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Learning
{
    public interface IFileManager
    {
        Task<DocumentSet> LoadAll(DirectoryInfo repositoryPath);

        IObservable<ProcessingProgress> ProgressUpdate { get; }
    }
}