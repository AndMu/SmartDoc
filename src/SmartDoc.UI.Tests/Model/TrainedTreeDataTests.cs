using System;
using NUnit.Framework;
using Wikiled.Arff.Persistence;
using Wikiled.SmartDoc.Model;

namespace Wikiled.SmartDoc.Tests.Model
{
    [TestFixture]
    public class TrainedTreeDataTests
    {
        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => TrainedTreeData.Construct(null));
            var result = TrainedTreeData.Construct(ArffDataSet.CreateSimple("Test"));
            Assert.AreEqual(0, result.Children.Length);
            Assert.AreEqual(0, result.Count);
            Assert.AreEqual("Documents", result.Name);
            Assert.AreEqual("Documents (0)", result.Description);
        }

        [Test]
        public void ConstructFull()
        {
            var dataSet = ArffDataSet.CreateSimple("Test");
            dataSet.Header.RegisterNominalClass("One", "Two");
            var review = dataSet.AddDocument();
            review.Class.Value = "One";
            review = dataSet.AddDocument();
            review.Class.Value = "One";
            review = dataSet.AddDocument();
            review.Class.Value = "Two";
            var result = TrainedTreeData.Construct(dataSet);
            Assert.AreEqual(2, result.Children.Length);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Documents", result.Name);
        }
    }
}
