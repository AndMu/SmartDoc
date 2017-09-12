using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Arff.Persistence;
using Wikiled.MachineLearning.Svm.Clients;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress;

namespace Wikiled.SmartDoc.Logic.IntegrationTests.Learning
{
    [TestFixture]
    public class LearnedClassifierTests
    {
        private LearnedClassifier instance;

        [SetUp]
        public void Setup()
        {
            var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "data.arff");
            var dataSet = ArffDataSet.LoadSimple(file);
            file = Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "model.dat");
            var model = Wikiled.MachineLearning.Svm.Logic.Model.Read(file);
            instance = new LearnedClassifier(
                new DocumentParser(Global.TextSplitter, new DevExpressParserFactory(20)),
                new SvmTestClient(dataSet, model));
        }
        
        [Test]
        public async Task Classify()
        {
            var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "data", "test.pdf");
            FileInfo file = new FileInfo(fileName);
            var result = await instance.Classify(file).ConfigureAwait(false);
            Assert.AreEqual("One", result);
        }
    }
}
