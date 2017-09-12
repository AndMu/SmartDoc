using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Tests.Learning
{
    [TestFixture]
    public class TrainingManagerTests
    {
        private TrainingManager manager;

        private DocumentSet set;

        [SetUp]
        public void Setup()
        {
            manager = new TrainingManager();
            set = new DocumentSet();
            set.Document = new[]
                           {
                               new DocumentDefinition
                               {
                                   Labels = new string[] {}
                               }
                           };
        }

        [Test]
        public void CreateDataset()
        {
            Assert.Throws<ArgumentNullException>(() => manager.CreateDataset(null, CancellationToken.None));
            var arff = manager.CreateDataset(set, CancellationToken.None);
            Assert.IsNull(arff);

            set.Document = new[]
                           {
                               new DocumentDefinition
                               {
                                   Labels = new []{"One", "Two"}
                               }
                           };

            set.Document[0].WordsTable["Test1"] = 1;
            set.Document[0].WordsTable["Test2"] = 2;
            arff = manager.CreateDataset(set, CancellationToken.None);
            Assert.AreEqual(1, arff.TotalDocuments);
            Assert.AreEqual("Two", arff.Documents.First().Class.Value);
            Assert.AreEqual(3, arff.Header.Total);
        }

        [Test]
        public void TrainInvalidArguments()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => manager.Train(null, TrainingHeader.CreateDefault(), CancellationToken.None));
            Assert.ThrowsAsync<ArgumentNullException>(() => manager.Train(set, null, CancellationToken.None));
            Assert.ThrowsAsync<LearningException>(() => manager.Train(set, TrainingHeader.CreateDefault(), CancellationToken.None));
        }
    }
}
