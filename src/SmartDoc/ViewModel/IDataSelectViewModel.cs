using Wikiled.SmartDoc.Logic.Results;
using Wikiled.SmartDoc.Model;
using Wikiled.SmartDoc.ViewModel.Definitions;

namespace Wikiled.SmartDoc.ViewModel
{
    public interface IDataSelectViewModel : ICancelableViewModel<DocumentSet, TrainedTreeData>
    {
        ISelectableViewModel Select { get; }
    }
}