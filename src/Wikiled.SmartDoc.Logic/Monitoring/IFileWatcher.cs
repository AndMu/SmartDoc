using System;
using System.IO;

namespace Wikiled.SmartDoc.Logic.Monitoring
{
    public interface IFileWatcher
    {
        IObservable<FileSystemEventArgs> FileChanged { get; }

        string Path { get; }

        bool CanUseFile(string filename);

        void Dispose();

        void Start();
    }
}