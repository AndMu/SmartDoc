using NLog;
using NUnit.Framework;
using Wikiled.Sentiment.Analysis.Processing.Splitters;

namespace Wikiled.SmartDoc.Logic.Tests
{
    [SetUpFixture]
    public class Global
    {
        public static LightSplitterHelper TextSplitter { get; private set; }

        [OneTimeSetUp]
        public void Setup()
        {
            TextSplitter = new LightSplitterHelper();
            TextSplitter.Load();
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}
