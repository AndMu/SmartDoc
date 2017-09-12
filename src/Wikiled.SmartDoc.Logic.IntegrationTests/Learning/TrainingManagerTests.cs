using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Arff.Normalization;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress;

namespace Wikiled.SmartDoc.Logic.IntegrationTests.Learning
{
    [TestFixture]
    [Ignore("No documents to test")]
    public class TrainingManagerTests
    {
        [Test]
        public async Task Learn()
        {
            FileManager manager = new FileManager(
                new DocumentParser(Global.TextSplitter, new DevExpressParserFactory(20)),
                new CancellationToken(),
                4);
            var set = await manager.LoadAll(new DirectoryInfo(@"location")).ConfigureAwait(false);
            TrainingManager training = new TrainingManager();
            var header = TrainingHeader.CreateDefault();
            header.Normalization = NormalizationType.L2;
            var trainingTask = training.Train(set, header, CancellationToken.None);
            var model = await trainingTask.ConfigureAwait(false);
            Assert.LessOrEqual(0.80, Math.Round(model.Model.Parameter.Performance, 2));
        }
    }
}