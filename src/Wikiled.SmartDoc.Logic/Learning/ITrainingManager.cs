using System.Threading;
using System.Threading.Tasks;
using Wikiled.Arff.Persistence;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Learning
{
    public interface ITrainingManager
    {
        Task<TrainingResults> Train(DocumentSet documentSet, TrainingHeader header, CancellationToken token);

        IArffDataSet CreateDataset(DocumentSet documentSet, CancellationToken token);
    }
}