using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Pdf;

namespace Wikiled.SmartDoc.Logic.Tests.Learning
{
    [TestFixture]
    public class FileManagerTests
    {
        private FileManager manager;

        private Mock<IDocumentParser> parser;

        private CancellationTokenSource token;

        [SetUp]
        public void Setup()
        {
            parser = new Mock<IDocumentParser>();
            token = new CancellationTokenSource();
            manager = new FileManager(parser.Object, token.Token);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new FileManager(null, token.Token));
            Assert.NotNull(manager);
        }
    }
}
