using System;
using System.IO;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress;

namespace Wikiled.SmartDoc.Logic.Tests.Pdf.Readers.DevExpress
{
    [TestFixture]
    public class DevExpressPdfParserTests
    {
        private DevExpressPdfParser instance;

        private FileInfo file;

        [SetUp]
        public void Setup()
        {
            file = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Pdf\Data\Tickets.pdf"));
            instance = new DevExpressPdfParser(file, 10);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentException>(() => new DevExpressPdfParser(file, 0));
            Assert.Throws<ArgumentNullException>(() => new DevExpressPdfParser(null, 10));
        }

        [Test]
        public void Parse()
        {
            var result = instance.Parse();
            Assert.IsNotNull(result);
            Assert.AreEqual(4717, result.Length);
        }
    }
}