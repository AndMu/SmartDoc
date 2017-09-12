using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Svm.Clients;
using Wikiled.SmartDoc.Logic.Pdf;

namespace Wikiled.SmartDoc.Logic.Learning
{
    public class LearnedClassifier : ILearnedClassifier
    {
        private readonly IDocumentParser parser;

        private readonly ISvmTestClient testClient;

        public LearnedClassifier(IDocumentParser parser, ISvmTestClient testClient)
        {
            Guard.NotNull(() => parser, parser);
            Guard.NotNull(() => testClient, testClient);
            this.parser = parser;
            this.testClient = testClient;
        }

        public async Task<string> Classify(FileInfo file)
        {
            var dataHolder = testClient.CreateTestDataset();
            var parsed = await parser.ParseDocument(file.Directory, file, CancellationToken.None).ConfigureAwait(false);
            if (parsed == null ||
                parsed.WordsTable.Count == 0)
            {
                return string.Empty;
            }

            var review = dataHolder.AddDocument();
            foreach (var record in parsed.WordsTable)
            {
                var addedRecord = review.AddRecord(record.Key);
                if (addedRecord != null)
                {
                    addedRecord.Value = record.Value;
                }
            }

            await Task.Run(() => testClient.Classify(dataHolder)).ConfigureAwait(false);
            return review.Class.Value as string;
        }
    }
}
