using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using Wikiled.Core.Utility.Extensions;
using Wikiled.SmartDoc.Logic.Monitoring;

namespace Wikiled.SmartDoc.Logic.Tests.Monitoring
{
    [TestFixture]
    public class MonitoringResultTests
    {
        private Bitmap bitmap;

        [SetUp]
        public void Setup()
        {
            bitmap = new Bitmap(36, 50);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new MonitoringResult(null, "Test", bitmap));
            Assert.Throws<ArgumentNullException>(() => new MonitoringResult(new FileInfo("XX1"), "Test", null));
            var result = new MonitoringResult(new FileInfo("XX1"), "Test", bitmap);
            Assert.AreEqual("Test", result.Class);
            Assert.AreEqual("XX1", result.File.Name);
            Assert.IsNotNull(result.Preview);
        }

        [Test]
        public void ApplyChange()
        {
            var file = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Data\Data.txt");
            var result = new MonitoringResult(new FileInfo(file), "Test", bitmap);
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Test").EnsureDirectoryExistence();
            bool completed = false;
            result.MoveRequest += (sender, args) => completed = true;
            result.ApplyChange(new DirectoryInfo(TestContext.CurrentContext.TestDirectory));
            Assert.IsTrue(completed);
        }
    }
}
