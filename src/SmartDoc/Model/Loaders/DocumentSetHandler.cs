using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Model.Loaders
{
    public class DocumentSetHandler : IDataHandler<DocumentSet>
    {
        private readonly string fileName;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DocumentSetHandler(string fileName)
        {
            Guard.NotNullOrEmpty(() => fileName, fileName);
            this.fileName = fileName;
        }

        public Task<DocumentSet> Load(CancellationToken cancellation)
        {
            return Task.Run(() => LoadInternal(), cancellation);
        }

        public Task Save(DocumentSet result, CancellationToken cancellation)
        {
            return Task.Run(() => SaveData(result), cancellation);
        }
        
        private void SaveData(DocumentSet result)
        {
            if (result == null)
            {
                return;
            }

            try
            {
                Logger.Debug("Saving: {0} with {1} results", fileName, result.Document?.Length);
                File.WriteAllBytes(fileName, result.ProtoSerializeCompress());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private DocumentSet LoadInternal()
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return null;
                }

                Logger.Debug("Loading {0}", fileName);
                var data = File.ReadAllBytes(fileName);
                var doc = data.ProtoDecompressDeserialize<DocumentSet>();
                return doc;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }
    }
}
