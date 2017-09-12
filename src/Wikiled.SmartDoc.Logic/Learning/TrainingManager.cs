using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Arff.Extensions;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Logging;
using Wikiled.MachineLearning.Svm.Clients;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Learning
{
    public class TrainingManager : ITrainingManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public async Task<TrainingResults> Train(DocumentSet documentSet, TrainingHeader header, CancellationToken token)
        {
            Guard.NotNull(() => documentSet, documentSet);
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => documentSet.Document, documentSet.Document);
            log.Debug("Train with {0} - {1}", documentSet.Document.Length, header);
            using (PerformanceTrace.Info(log, "Training..."))
            {
                var arff = CreateDataset(documentSet, token);
                if (arff == null)
                {
                    throw new LearningException("Not enough documents to learn patterns");
                }

                arff.CompactHeader(3);
                arff.CompactReviews(3);
                arff.CompactClass(3);

                if (arff.TotalDocuments < 10)
                {
                    throw new LearningException("Not enough documents to learn patterns");
                }

                token.ThrowIfCancellationRequested();
                arff.Normalize(header.Normalization);
                SvmTrainClient train = new SvmTrainClient(arff);
                var model = await train.Train(header, token).ConfigureAwait(false);
                return model;
            }
        }

        public IArffDataSet CreateDataset(DocumentSet documentSet, CancellationToken token)
        {
            Guard.NotNull(() => documentSet, documentSet);
            log.Debug("CreateDataset");
            if (documentSet.Document == null ||
                documentSet.Document.Length == 0)
            {
                log.Warn("No documents");
                return null;
            }

            var labels = (from item in documentSet.Document
                          from label in item.Labels
                          select label).Distinct().ToArray();

            if (labels.Length == 0)
            {
                log.Warn("No labels found");
                return null;
            }

            var dataHolder = ArffDataSet.CreateSimple("subjectivity");
            dataHolder.Header.RegisterNominalClass(labels);
            foreach (var definition in documentSet.Document.Where(item => item.Labels.Length > 0))
            {
                token.ThrowIfCancellationRequested();
                var label = definition.Labels.Last();
                var review = dataHolder.AddDocument();
                review.Class.Value = label;
                foreach (var record in definition.WordsTable)
                {
                    review.AddRecord(record.Key).Value = record.Value;
                }
            }

            return dataHolder;
        }
    }
}
