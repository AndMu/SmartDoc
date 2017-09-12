using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crc32C;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Sentiment.Analysis.Processing;
using Wikiled.Sentiment.Text.Extensions;
using Wikiled.Sentiment.Text.Parser;
using Wikiled.SmartDoc.Logic.Pdf.Readers;
using Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress;
using Wikiled.SmartDoc.Logic.Results;
using Wikiled.Text.Analysis.POS;

namespace Wikiled.SmartDoc.Logic.Pdf
{
    public class DocumentParser : IDocumentParser
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ITextParserFactory textParserFactory;

        private readonly ISplitterHelper textSplitter;

        private readonly ConcurrentDictionary<string, string> wordsTable = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public DocumentParser(ISplitterHelper textSplitter, ITextParserFactory textParserFactory)
        {
            Guard.NotNull(() => textSplitter, textSplitter);
            Guard.NotNull(() => textParserFactory, textParserFactory);
            this.textSplitter = textSplitter;
            this.textParserFactory = textParserFactory;
        }

        public string[] Supported => textParserFactory.Supported;

        public async Task<DocumentDefinition> ParseDocument(DirectoryInfo repositoryPath, FileInfo file, CancellationToken token)
        {
            logger.Debug("ParseDocument: {0}", file);
            Guard.NotNull(() => file, file);
            Guard.NotNull(() => repositoryPath, repositoryPath);
            Guard.IsValid(() => file, file, info => info.Exists, "invalid file");

            var parser = textParserFactory.ConstructParsers(file);
            if (parser == NullTextParser.Instance)
            {
                logger.Debug("Null parser: {0}", file);
                return null;
            }

            var text = parser.Parse();
            DocumentDefinition definition = new DocumentDefinition();
            var bytes = File.ReadAllBytes(file.FullName);
            token.ThrowIfCancellationRequested();
            definition.Crc32 = Crc32CAlgorithm.Compute(bytes);
            string path = string.IsNullOrEmpty(repositoryPath.FullName) || repositoryPath.FullName[repositoryPath.FullName.Length - 1] == Path.DirectorySeparatorChar
                              ? repositoryPath.FullName
                              : $"{repositoryPath.FullName}{Path.DirectorySeparatorChar}";
            var directory = path.GetRelativePath(file.DirectoryName);
            if (directory == string.Empty)
            {
                if (file.Directory != null)
                {
                    definition.Labels = new[] { file.Directory.Name };
                }
            }
            else
            {
                definition.Labels = new[] { directory.Split(Path.DirectorySeparatorChar).First() };
            }

            definition.Labels = definition.Labels.Select(item => item.CreateLetterText()).ToArray();
            definition.Path = file.FullName;

            if (!string.IsNullOrWhiteSpace(text))
            {
                var result = await textSplitter.Splitter.Process(new ParseRequest(text)).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();
                var review = result.GetReview(textSplitter.DataLoader);
                token.ThrowIfCancellationRequested();
                var words = review.Items.Where(
                    item =>
                    item.POS.WordType != WordType.Number &&
                    item.POS.WordType != WordType.SeparationSymbol &&
                    item.POS.WordType != WordType.Symbol &&
                    item.POS.WordType != WordType.Conjunction &&
                    item.POS.WordType != WordType.Sentence &&
                    !item.IsStopWord)
                                  .Select(item => item.Text)
                                  .ToArray();

                foreach (var word in words)
                {
                    token.ThrowIfCancellationRequested();
                    string underlyingWord;
                    if (!wordsTable.TryGetAddItem(word, word, out underlyingWord))
                    {
                        underlyingWord = word;
                    }

                    var total = definition.WordsTable.GetSafe(underlyingWord);
                    total++;
                    definition.WordsTable[underlyingWord] = total;
                }
            }

            return definition;
        }
    }
}
