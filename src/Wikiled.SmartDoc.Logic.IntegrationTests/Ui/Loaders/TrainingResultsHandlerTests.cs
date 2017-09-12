using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.SmartDoc.Model.Loaders;
using Wikiled.SmartDoc.TestHelpers.Data;

namespace Wikiled.SmartDoc.Logic.IntegrationTests.Ui.Loaders
{
    [TestFixture]
    public class TrainingResultsHandlerTests
    {
        [Test]
        public async Task LoadingSaving()
        {
            var file = Path.Combine(TestContext.CurrentContext.TestDirectory, Guid.NewGuid().ToString());
            TrainingResultsHandler loader = new TrainingResultsHandler(file);
            var data = await loader.Load(CancellationToken.None).ConfigureAwait(false);
            Assert.IsNull(data);
            data = TestConstants.GetTrainingResults();
            await loader.Save(data, CancellationToken.None).ConfigureAwait(false);
            data = await loader.Load(CancellationToken.None).ConfigureAwait(false);
            Assert.IsNotNull(data);
            Assert.IsTrue(Directory.Exists(file));
        }
    }
}
