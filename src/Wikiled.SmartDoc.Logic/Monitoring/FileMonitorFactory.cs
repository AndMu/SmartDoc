using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Svm.Clients;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.Logic.Pdf.Preview;

namespace Wikiled.SmartDoc.Logic.Monitoring
{
    public class FileMonitorFactory : IFileMonitorFactory
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentParser documentParser;

        public FileMonitorFactory(IDocumentParser documentParser)
        {
            Guard.NotNull(() => documentParser, documentParser);
            this.documentParser = documentParser;
        }

        public IFileMonitor Create(string path, TrainingResults training)
        {
            Guard.NotNullOrEmpty(() => path, path);
            Guard.NotNull(() => training, training);
            logger.Debug("Create <{0}>", path);
            return new FileMonitor(
                new FileWatcher(path, documentParser.Supported),
                new LearnedClassifier(documentParser, new SvmTestClient(training.DataSet, training.Model)),
                new PdfPreviewCreator());
        }
    }
}
