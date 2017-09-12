using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Wikiled.SmartDoc.Logic.Monitoring
{
    public interface IFileMonitor : IDisposable
    {
        Task Start(ObservableCollection<MonitoringResult> pendingFiles, TimeSpan throttle, CancellationToken token);
    }
}