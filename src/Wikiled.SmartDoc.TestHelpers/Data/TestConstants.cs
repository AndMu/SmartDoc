using Wikiled.Arff.Persistence;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.MachineLearning.Svm.Parameters;

namespace Wikiled.SmartDoc.TestHelpers.Data
{
    public static class TestConstants
    {
        public static TrainingResults GetTrainingResults()
        {
            var model = new Model();
            model.NumberOfClasses = 2;
            model.ClassLabels = null;
            model.NumberOfSVPerClass = null;
            model.PairwiseProbabilityA = null;
            model.PairwiseProbabilityB = null;
            model.SupportVectorCoefficients = new double[1][];
            model.Rho = new double[1];
            model.Rho[0] = 0;
            model.Parameter = new Parameter();
            var dataset = ArffDataSet.CreateSimple("Test");
            return new TrainingResults(model, TrainingHeader.CreateDefault(), dataset);
        }
    }
}
