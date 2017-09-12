using System;
using Moq;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Monitoring;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.TestHelpers.Data;

namespace Wikiled.SmartDoc.Logic.Tests.Monitoring
{
    [TestFixture]
    public class FileMonitorFactoryTests
    {
        private Mock<IDocumentParser> parser;

        private FileMonitorFactory instance;

        [SetUp]
        public void Setup()
        {
            parser = new Mock<IDocumentParser>();
            instance = new FileMonitorFactory(parser.Object);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new FileMonitorFactory(null));
            Assert.IsNotNull(instance);
        }

        [Test]
        public void Create()
        {
            string path = @"c:\";
            var results = TestConstants.GetTrainingResults();
            Assert.Throws<ArgumentNullException>(() => instance.Create(null, results));
            Assert.Throws<ArgumentNullException>(() => instance.Create(path, null));
            var result = instance.Create(path, results);
            Assert.IsNotNull(result);
            result.Dispose();
        }
    }
}
