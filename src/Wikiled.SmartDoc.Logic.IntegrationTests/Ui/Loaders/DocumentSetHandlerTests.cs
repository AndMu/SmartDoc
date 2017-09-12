using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Results;
using Wikiled.SmartDoc.Model.Loaders;

namespace Wikiled.SmartDoc.Logic.IntegrationTests.Ui.Loaders
{
    [TestFixture]
    public class DocumentSetHandlerTests
    {
        [Test]
        public async Task LoadingSaving()
        {
            var file = Path.Combine(TestContext.CurrentContext.TestDirectory, Guid.NewGuid().ToString());
            DocumentSetHandler loader = new DocumentSetHandler(file);
            var data = await loader.Load(CancellationToken.None).ConfigureAwait(false);
            Assert.IsNull(data);
            data = new DocumentSet();
            await loader.Save(data, CancellationToken.None).ConfigureAwait(false);
            data = await loader.Load(CancellationToken.None).ConfigureAwait(false);
            Assert.IsNotNull(data);
            Assert.IsTrue(File.Exists(file));
        }
    }
}
