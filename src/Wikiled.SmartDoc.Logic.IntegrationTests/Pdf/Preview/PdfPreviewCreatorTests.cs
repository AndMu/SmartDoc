using System.IO;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Pdf.Preview;

namespace Wikiled.SmartDoc.Logic.IntegrationTests.Pdf.Preview
{
    [TestFixture]
    public class PdfPreviewCreatorTests
    {
        [TestCase("test.pdf")]
        [TestCase("xxx")]
        public void CreatePreview(string name)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", name);
            PdfPreviewCreator creator = new PdfPreviewCreator();
            var bitmap = creator.CreatePreview(new FileInfo(path));
            Assert.IsNotNull(bitmap);
        }
    }
}
