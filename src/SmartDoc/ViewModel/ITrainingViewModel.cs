using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Model;
using Wikiled.SmartDoc.ViewModel.Definitions;

namespace Wikiled.SmartDoc.ViewModel
{
    public interface ITrainingViewModel : ICancelableViewModel<TrainingResults, TrainedTreeData>
    {
        bool GridSelection { get; set; }
    }
}