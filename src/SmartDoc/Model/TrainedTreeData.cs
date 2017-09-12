using System;
using System.Collections.Generic;
using System.Linq;
using Wikiled.Arff.Persistence;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Model
{
    public class TrainedTreeData
    {
        private TrainedTreeData(string name, int count)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Name = name;
            Count = count;
        }

        public TrainedTreeData[] Children { get; private set; }

        public int Count { get; }

        public string Name { get; }

        public string Description => string.Format($"{Name} ({Count})");

        public static TrainedTreeData ConstructFromDocuments(DocumentSet dataSet)
        {
            Guard.NotNull(() => dataSet, dataSet);
            Guard.NotNull(() => dataSet.Document, dataSet.Document);
            return ConstructInternal(dataSet.Document.Select(item => (Func<string>)(() => item.Labels[0])));
        }

        public static TrainedTreeData Construct(IArffDataSet dataSet)
        {
            Guard.NotNull(() => dataSet, dataSet);
            return ConstructInternal(dataSet.Documents.Select(item => (Func<string>)(() => (string)item.Class.Value)));
        }

        private static TrainedTreeData ConstructInternal(IEnumerable<Func<string>> sources)
        {
            Dictionary<string, int> table = new Dictionary<string, int>();
            int totalItems = 0;
            foreach (var review in sources)
            {
                string className = review();
                int total;
                table.TryGetValue(className, out total);
                total++;
                totalItems++;
                table[className] = total;
            }

            var root = new TrainedTreeData("Documents", totalItems);
            root.Children = new TrainedTreeData[table.Count];
            int currentId = 0;
            foreach (var record in table)
            {
                root.Children[currentId] = new TrainedTreeData(record.Key, record.Value);
                currentId++;
            }

            return root;
        }
    }
}
