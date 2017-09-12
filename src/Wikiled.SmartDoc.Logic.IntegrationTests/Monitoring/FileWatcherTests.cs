using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Monitoring;

namespace Wikiled.SmartDoc.Logic.IntegrationTests.Monitoring
{
    [TestFixture]
    public class FileWatcherTests
    {
        private FileWatcher watcher;

        private ConcurrentBag<FileSystemEventArgs> data;

        private IDisposable observer;

        private string path;

        [SetUp]
        public void Setup()
        {
            path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Input");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
            watcher = new FileWatcher(path);
            data = new ConcurrentBag<FileSystemEventArgs>();
            observer = watcher.FileChanged.Subscribe(value => data.Add(value));
        }

        [TearDown]
        public void TearDown()
        {
            observer.Dispose();
        }

        [Test]
        public void Monitor()
        {
            watcher.Start();
            Assert.AreEqual(0, data.Count);
            Thread.Sleep(1000);
            Assert.AreEqual(0, data.Count);
            var file = Path.Combine(path, "Test.pdf");
            File.WriteAllText(file, "Test");
            Thread.Sleep(500);
            Assert.AreEqual(2, data.Count);
            File.Delete(file);
            Thread.Sleep(500);
            Assert.AreEqual(3, data.Count);
        }
    }
}
