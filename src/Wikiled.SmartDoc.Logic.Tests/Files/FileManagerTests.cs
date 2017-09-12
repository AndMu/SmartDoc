using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Tests.Files
{
    [TestFixture]
    public class FileManagerTests
    {
        private FileManager manager;

        private Mock<IDocumentParser> parser;

        [SetUp]
        public void Setup()
        {
            parser = new Mock<IDocumentParser>();
            manager = new FileManager(parser.Object, new CancellationToken());
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(
                () => new FileManager(null, CancellationToken.None));
        }

        [Test]
        public async Task Load()
        {
            DocumentDefinition definition = new DocumentDefinition();
            parser.Setup(item => item.ParseDocument(It.IsAny<DirectoryInfo>(), It.IsAny<FileInfo>(), It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult(definition));
            List<ProcessingProgress> list = new List<ProcessingProgress>();
            var subscription = manager.ProgressUpdate.Subscribe(
                progress =>
                {
                    lock (list)
                    {
                        list.Add(progress);
                    }
                });

            var result = await manager.LoadAll(new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\pdf")));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Path);
            Assert.AreEqual(1, result.Document.Length);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0].Total);
            Assert.AreEqual(0, list[0].Current);
            Assert.AreEqual(1, list[1].Total);
            Assert.AreEqual(1, list[1].Current);
            subscription.Dispose();
        }
    }
}
