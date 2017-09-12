using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Svm.Logic;

namespace Wikiled.SmartDoc.Model.Loaders
{
    public class TrainingResultsHandler : IDataHandler<TrainingResults>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string dataPath;

        public TrainingResultsHandler(string dataPath)
        {
            Guard.NotNullOrEmpty(() => dataPath, dataPath);
            this.dataPath = dataPath;
        }

        public Task<TrainingResults> Load(CancellationToken cancellation)
        {
            return Task.Run(() =>
                {
                    try
                    {
                        logger.Debug("Loading: {0}", dataPath);
                        if (Directory.Exists(dataPath))
                        {
                            var result = TrainingResultsExtension.Load(dataPath);
                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                    return null;
                });
        }

        public Task Save(TrainingResults data, CancellationToken cancellation)
        {
            return Task.Run(() => data?.Save(dataPath));
        }
    }
}
