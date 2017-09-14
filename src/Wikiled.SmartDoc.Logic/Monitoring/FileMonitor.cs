using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Pdf.Preview;

namespace Wikiled.SmartDoc.Logic.Monitoring
{
    public class FileMonitor : IFileMonitor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ILearnedClassifier classifier;

        private readonly ConcurrentDictionary<string, MonitoringResult[]> directoryFiles = new ConcurrentDictionary<string, MonitoringResult[]>(StringComparer.OrdinalIgnoreCase);

        private readonly SemaphoreSlim limit = new SemaphoreSlim(4);
        

        private readonly IPreviewCreator previewCreator;

        private readonly object syncRoot = new object();

        private readonly IFileWatcher watcher;

        private ObservableCollection<MonitoringResult> pendingFiles;

        private IDisposable subscription;

        private CancellationToken token;

        public FileMonitor(IFileWatcher watcher, ILearnedClassifier classifier, IPreviewCreator previewCreator)
        {
            Guard.NotNull(() => watcher, watcher);
            Guard.NotNull(() => classifier, classifier);
            Guard.NotNull(() => previewCreator, previewCreator);
            this.classifier = classifier;
            this.previewCreator = previewCreator;
            this.watcher = watcher;
        }

        public void Dispose()
        {
            watcher.Dispose();
            subscription?.Dispose();
        }

        public async Task Start(ObservableCollection<MonitoringResult> target, TimeSpan throttle, CancellationToken cancellationToken)
        {
            lock (syncRoot)
            {
                pendingFiles = target;
            }

            target.Clear();
            token = cancellationToken;
            var directories = Directory.GetDirectories(watcher.Path, "*", SearchOption.AllDirectories);
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(() => ProcessDirectory(watcher.Path), token));
            foreach (var directory in directories)
            {
                tasks.Add(Task.Run(() => ProcessDirectory(directory), token));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            subscription = watcher.FileChanged
                                  .Throttle(throttle)
                                  .ObserveOn(ThreadPoolScheduler.Instance)
                                  .Subscribe(UpdateReceived);
            watcher.Start();
        }

        private async Task<MonitoringResult> CreateNewFile(string file)
        {
            try
            {
                await limit.WaitAsync(token).ConfigureAwait(false);
                FileInfo fileInfo = new FileInfo(file);
                var previewTask = Task.Run(() => previewCreator.CreatePreview(fileInfo), token);
                string className = await classifier.Classify(fileInfo).ConfigureAwait(false);
                MonitoringResult result = new MonitoringResult(fileInfo, className, await previewTask.ConfigureAwait(false));
                result.MoveRequest += ResultOnMoveRequest;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                limit.Release();
            }

            return null;
        }

        private async Task ProcessDirectory(string directory)
        {
            MonitoringResult[] results;
            if (!directoryFiles.TryGetValue(directory, out results))
            {
                results = new MonitoringResult[] { };
            }

            var fileTable = results.Where(item => pendingFiles.Contains(item))
                                   .ToDictionary(item => item.File.FullName, item => item);

            var tasks = Directory.EnumerateFiles(directory)
                                 .Where(item => watcher.CanUseFile(item))
                                 .Select(file => ProcessFile(fileTable, file))
                                 .ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            RemoveFiles(fileTable);
            directoryFiles[directory] = tasks.Select(item => item.Result).ToArray();
        }

        private async Task<MonitoringResult> ProcessFile(Dictionary<string, MonitoringResult> fileTable, string file)
        {
            MonitoringResult result;
            if (!fileTable.TryGetValue(file, out result))
            {
                result = await CreateNewFile(file).ConfigureAwait(false);
                lock (syncRoot)
                {
                    pendingFiles.Add(result);
                }
            }
            else
            {
                fileTable.Remove(file);
            }

            return result;
        }

        private void QuickScan(FileSystemEventArgs args)
        {
            if (!File.Exists(args.FullPath))
            {
                var file = pendingFiles.FirstOrDefault(item => string.Compare(item.File.FullName, args.FullPath, StringComparison.OrdinalIgnoreCase) == 0);
                if (file != null)
                {
                    pendingFiles.Remove(file);
                }
            }
        }

        private void RemoveFiles(Dictionary<string, MonitoringResult> fileTable)
        {
            if (fileTable.Count > 0)
            {
                Logger.Info("Removing: {0}", fileTable.Count);
                foreach (var value in fileTable.Values)
                {
                    lock (syncRoot)
                    {
                        pendingFiles.Remove(value);
                    }
                }
            }
        }

        private async void UpdateReceived(FileSystemEventArgs args)
        {
            Logger.Debug("UpdateReceived {0}", args);
            var directory = Path.GetDirectoryName(args.FullPath);
            if (string.IsNullOrEmpty(directory))
            {
                Logger.Error("Invalid path received: {0}", args.FullPath);
                return;
            }

            QuickScan(args);
            await ProcessDirectory(directory).ConfigureAwait(false);
        }

        private void ResultOnMoveRequest(object sender, EventArgs eventArgs)
        {
            var result = (MonitoringResult)sender;
            result.File.MoveTo(result.DestinationFile.FullName);
            result.MoveRequest -= ResultOnMoveRequest;
            result.Dispose();
            lock (syncRoot)
            {
                pendingFiles.Remove(result);
            }
        }
    }
}
