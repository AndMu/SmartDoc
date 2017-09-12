using NUnit.Framework;
using Wikiled.SmartDoc.Helpers;

namespace Wikiled.SmartDoc.Tests.Helpers
{
    [TestFixture]
    public class DocumentImageSelectorTests
    {
        [Test]
        public void Select()
        {
            DocumentImageSelector selector = new DocumentImageSelector();
            var image = selector.Select(null);
            Assert.IsNotNull(image);
            var image2 = selector.Select(null);
            Assert.IsNotNull(image);
            Assert.AreSame(image, image2);
        }
    }
}
