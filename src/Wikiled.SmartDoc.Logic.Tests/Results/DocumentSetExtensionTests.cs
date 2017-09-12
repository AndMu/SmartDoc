using System;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Tests.Results
{
    [TestFixture]
    public class DocumentSetExtensionTests
    {
        [Test]
        public void GetFiltered()
        {
            DocumentSet set = new DocumentSet();
            set.Document = new[]
                           {
                               new DocumentDefinition
                               {
                                   Labels = new[] {"Test1"}
                               },
                               new DocumentDefinition
                               {
                                   Labels = new[] {"Test2"}
                               },
                           };
            set.TotalRequested = 2;
            Assert.Throws<ArgumentNullException>(() => DocumentSetExtension.GetFiltered(null, new[] { "test" }));
            Assert.Throws<ArgumentNullException>(() => DocumentSetExtension.GetFiltered(null, new[] { "test" }));
            var result = DocumentSetExtension.GetFiltered(set, new[] { "Test2" });
            Assert.AreNotSame(set, result);
            Assert.AreEqual(1, result.Document.Length);
            Assert.AreEqual("Test2", result.Document[0].Labels[0]);
        }
    }
}
