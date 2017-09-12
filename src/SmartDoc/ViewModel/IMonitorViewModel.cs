using Wikiled.SmartDoc.Logic.Monitoring;
using Wikiled.SmartDoc.ViewModel.Definitions;

namespace Wikiled.SmartDoc.ViewModel
{
    public interface IMonitorViewModel : ICancelableViewModel<IFileMonitor, MonitoringResult>
    {
        ISelectableViewModel Monitor { get; }

        ISelectableViewModel Destination { get; }

        void DoubleClicked();
    }
}
