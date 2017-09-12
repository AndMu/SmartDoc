using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.Core.Utility.Extensions;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Monitoring;
using Wikiled.SmartDoc.Logic.Pdf.Preview;

namespace Wikiled.SmartDoc.Logic.IntegrationTests.Monitoring
{
    [TestFixture]
    public class FileMonitorTests
    {
        private string path;

        private FileMonitor monitor;

        private Mock<ILearnedClassifier> learnedClassifier;

        [SetUp]
        public void Setup()
        {
            path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Input2");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
            var watcher = new FileWatcher(path);
            learnedClassifier = new Mock<ILearnedClassifier>();
            monitor = new FileMonitor(watcher, learnedClassifier.Object, new PdfPreviewCreator());
            learnedClassifier.Setup(item => item.Classify(It.IsAny<FileInfo>())).Returns(Task.FromResult("Out"));
            var outputPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Out");
            outputPath.EnsureDirectoryExistence();
        }

        [Test]
        public async Task TestMonitoring()
        {
            ObservableCollection<MonitoringResult> target = new ObservableCollection<MonitoringResult>();
            await monitor.Start(target, TimeSpan.FromMilliseconds(1), CancellationToken.None).ConfigureAwait(false);
            Thread.Sleep(500);
            Assert.AreEqual(0, target.Count);
            var file = Path.Combine(path, "Test.pdf");
            File.WriteAllText(file, "Test");
            Thread.Sleep(500);
            Assert.AreEqual(1, target.Count);
            var targetDir = Path.Combine(TestContext.CurrentContext.TestDirectory, Guid.NewGuid().ToString());
            Path.Combine(targetDir, "Out").EnsureDirectoryExistence();
            target[0].ApplyChange(new DirectoryInfo(targetDir));
            Assert.AreEqual(0, target.Count);
            File.WriteAllText(file, "Test");
            Thread.Sleep(500);
            Assert.AreEqual(1, target.Count);
        }
    }
}
