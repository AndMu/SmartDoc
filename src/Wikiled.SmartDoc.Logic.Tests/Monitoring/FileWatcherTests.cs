using System;
using NUnit.Framework;
using Wikiled.SmartDoc.Logic.Monitoring;

namespace Wikiled.SmartDoc.Logic.Tests.Monitoring
{
    [TestFixture]
    public class FileWatcherTests
    {
        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new FileWatcher(null));
        }

        [Test]
        public void Dispose()
        {
            var watcher = new FileWatcher(".");
            watcher.Dispose();
        }
    }
}
