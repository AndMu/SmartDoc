using System;
using System.IO;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress;

namespace Wikiled.SmartDoc.Logic.Tests.Pdf.Readers.DevExpress
{
    [TestFixture]
    public class DevExpressPdfParserFactoryTests
    {
         private DevExpressParserFactory instance;

        [SetUp]
        public void Setup()
        {
            instance = new DevExpressParserFactory(10);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentException>(() => new DevExpressParserFactory(0));
        }

        [Test]
        public void ConstructParsers()
        {
            var file = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Pdf\Tickets.pdf"));
            Assert.Throws<ArgumentNullException>(() => instance.ConstructParsers(null));
            var parser = instance.ConstructParsers(file);
            Assert.IsNotNull(parser);
        }
    }
}
