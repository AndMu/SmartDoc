using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.Arff.Persistence;
using Wikiled.Arff.Persistence.Headers;
using Wikiled.MachineLearning.Svm.Clients;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Tests.Learning
{
    [TestFixture]
    public class LearnedClassifierTests
    {
        private Mock<ISvmTestClient> testClient;

        private Mock<IDocumentParser> parser;

        private Mock<IArffDataSet> dataset;

        private Mock<IArffDataRow> review;

        private LearnedClassifier instance;

        private FileInfo file;

        private DocumentDefinition definition;

        [SetUp]
        public void Setup()
        {
            parser = new Mock<IDocumentParser>();
            testClient = new Mock<ISvmTestClient>();
            dataset = new Mock<IArffDataSet>();
            review = new Mock<IArffDataRow>();
            dataset.Setup(item => item.AddDocument()).Returns(review.Object);
            testClient.Setup(item => item.CreateTestDataset()).Returns(dataset.Object);
            instance = new LearnedClassifier(parser.Object, testClient.Object);
            file = new FileInfo("test.doc");
            definition = new DocumentDefinition();
        }

        [Test]
        public void Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new LearnedClassifier(null, testClient.Object));
            Assert.Throws<ArgumentNullException>(() => new LearnedClassifier(parser.Object, null));
        }

        [Test]
        public async Task Classify()
        {
            testClient.Setup(item => item.Classify(dataset.Object));
            definition.WordsTable["Test"] = 1;
            parser.Setup(item => item.ParseDocument(It.IsAny<DirectoryInfo>(), It.IsAny<FileInfo>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(definition));
            Mock<IHeader> header = new Mock<IHeader>();
            review.Setup(item => item.Class).Returns(new DataRecord(header.Object) { Value = "10" });
            var result = await instance.Classify(file).ConfigureAwait(false);
            Assert.AreEqual("10", result);
        }

        [Test]
        public async Task ClassifyNoWords()
        {
            testClient.Setup(item => item.Classify(dataset.Object));
            parser.Setup(item => item.ParseDocument(It.IsAny<DirectoryInfo>(), It.IsAny<FileInfo>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(definition));
            Mock<IHeader> header = new Mock<IHeader>();
            review.Setup(item => item.Class).Returns(new DataRecord(header.Object) { Value = "10" });
            var result = await instance.Classify(file).ConfigureAwait(false);
            Assert.AreEqual(string.Empty, result);
        }
    }
}
