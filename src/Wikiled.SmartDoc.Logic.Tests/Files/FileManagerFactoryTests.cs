using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Pdf;

namespace Wikiled.SmartDoc.Logic.Tests.Files
{
    [TestFixture]
    public class FileManagerFactoryTests
    {
        private Mock<IDocumentParser> parser;

        private FileManagerFactory instance;

        [SetUp]
        public void Setup()
        {
            parser = new Mock<IDocumentParser>();
            instance = new FileManagerFactory(parser.Object);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new FileManagerFactory(null));
        }

        [Test]
        public void Create()
        {
            var insance = instance.Create(new CancellationToken());
            Assert.IsNotNull(insance);
        }
    }
}
