using Wikiled.MachineLearning.Svm.Logic;

namespace Wikiled.SmartDoc.Logic.Monitoring
{
    public interface IFileMonitorFactory
    {
        IFileMonitor Create(string path, TrainingResults training);
    }
}