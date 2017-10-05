using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress;

namespace Wikiled.SmartDoc.Logic.Tests.Pdf
{
    [TestFixture]
    public class DocumentParserTests
    {
        private DocumentParser instance;

        private DirectoryInfo root;

        [SetUp]
        public void Setup()
        {
            root = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "."));
            instance = new DocumentParser(Global.TextSplitter, new DevExpressParserFactory(20));
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new DocumentParser(null, new DevExpressParserFactory(20)));
            Assert.Throws<ArgumentNullException>(() => new DocumentParser(Global.TextSplitter, null));
        }

        [Test]
        public async Task ParsePdf()
        {
            var file = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, @".\Pdf\Data\Tickets.pdf"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.ParseDocument(root, null, CancellationToken.None).ConfigureAwait(false));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.ParseDocument(null, file, CancellationToken.None).ConfigureAwait(false));
            var parse = await instance.ParseDocument(root, file, CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(parse.Path.IndexOf("Ticket", StringComparison.OrdinalIgnoreCase) > 0);
            Assert.AreEqual(4120648248, parse.Crc32);
            Assert.AreEqual(1, parse.Labels.Length);
            Assert.AreEqual(232, parse.WordsTable.Count);
            Assert.AreEqual(2, parse.WordsTable["bank"]);
        }
    }
}
