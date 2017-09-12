using System;
using System.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.SmartDoc.Logic.Results
{
    public static class DocumentSetExtension
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static DocumentSet GetFiltered(this DocumentSet set, string[] labels)
        {
            Guard.NotNull(() => set, set);
            Guard.NotNull(() => labels, labels);
            logger.Debug("GetFiltered: {0}", labels.Length);
            var lookpup = labels.ToLookup(item => item, item => item, StringComparer.OrdinalIgnoreCase);
            DocumentSet result = new DocumentSet();
            result.TotalRequested = set.TotalRequested;
            result.Path = set.Path;
            result.Created = set.Created;
            result.Document = set.Document
                .Where(item => item.Labels != null && item.Labels.Any(label => lookpup.Contains(label)))
                .ToArray();
            return result;
        }
    }
}
