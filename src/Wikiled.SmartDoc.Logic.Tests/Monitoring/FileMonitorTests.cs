using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Monitoring;
using Wikiled.SmartDoc.Logic.Pdf.Preview;

namespace Wikiled.SmartDoc.Logic.Tests.Monitoring
{
    [TestFixture]
    public class FileMonitorTests
    {
        private Mock<IFileWatcher> watcher;

        private Mock<ILearnedClassifier> learnedClassifier;

        private FileMonitor instance;

        private Subject<FileSystemEventArgs> subject;

        private Mock<IPreviewCreator> preview;

        [SetUp]
        public void Setup()
        {
            subject = new Subject<FileSystemEventArgs>();
            watcher = new Mock<IFileWatcher>();
            preview = new Mock<IPreviewCreator>();
            learnedClassifier = new Mock<ILearnedClassifier>();
            watcher.Setup(item => item.FileChanged).Returns(subject.AsObservable());
            watcher.Setup(item => item.Path).Returns(@"..\");
            instance = new FileMonitor(watcher.Object, learnedClassifier.Object, preview.Object);
        }

        [Test]
        public void Contruct()
        {
            Assert.Throws<ArgumentNullException>(() => new FileMonitor(null, learnedClassifier.Object, preview.Object));
            Assert.Throws<ArgumentNullException>(() => new FileMonitor(watcher.Object, null, preview.Object));
            Assert.Throws<ArgumentNullException>(() => new FileMonitor(watcher.Object, learnedClassifier.Object, null));
            Assert.NotNull(instance);
        }

        [Test]
        public void Dispose()
        {
            instance.Dispose();
            watcher.Verify(item => item.Dispose());
        }

        [Test]
        public async Task Exception()
        {
            learnedClassifier.Setup(item => item.Classify(It.IsAny<FileInfo>())).Throws<NullReferenceException>();
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data");
            ObservableCollection<MonitoringResult> target = new ObservableCollection<MonitoringResult>();
            await instance.Start(target, TimeSpan.FromMilliseconds(1), CancellationToken.None);
            subject.OnNext(new FileSystemEventArgs(WatcherChangeTypes.Created, path, "x.pdf"));
            Thread.Sleep(500);
            Assert.AreEqual(0, target.Count);
        }
    }
}
