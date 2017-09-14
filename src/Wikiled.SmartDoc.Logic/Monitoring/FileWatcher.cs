using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NLog;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.SmartDoc.Logic.Monitoring
{
    public class FileWatcher : IDisposable, IFileWatcher
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Subject<FileSystemEventArgs> monitorResult = new Subject<FileSystemEventArgs>();

        private readonly FileSystemWatcher watcher;

        private readonly string[] filter;

        public FileWatcher(string path, string[] filters = null)
        {
            Guard.NotNullOrEmpty(() => path, path);
            logger.Debug("Construct: <{0}> <{1}>", path, filter);
            filter = filters;
            Path = path;
            watcher = new FileSystemWatcher(path);
            watcher.IncludeSubdirectories = true;
        }

        public IObservable<FileSystemEventArgs> FileChanged => monitorResult.AsObservable();

        public string Path { get; }

        public bool CanUseFile(string filename)
        {
            Guard.NotNullOrEmpty(() => filename, filename);
            if (filter != null)
            {
                if (!filter.Any(item => filename.EndsWith(item, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }

            return true;
        }

        public void Dispose()
        {
            logger.Debug("Dispose");
            monitorResult.OnCompleted();
            watcher.Dispose();
        }

        public void Start()
        {
            logger.Debug("Start");
            /* Watch for changes in LastAccess and LastWrite times, and
           the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            // Only watch text files.
            // Add event handlers.
            watcher.Changed += WatcherOnChanged;
            watcher.Created += WatcherOnChanged;
            watcher.Deleted += WatcherOnChanged;
            watcher.Renamed += WatcherOnChanged;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (!CanUseFile(fileSystemEventArgs.Name))
            {
                return;
            }

            logger.Debug("WatcherOnChanged: {0}:{1}", fileSystemEventArgs.Name, fileSystemEventArgs.ChangeType);
            monitorResult.OnNext(fileSystemEventArgs);
        }
    }
}
