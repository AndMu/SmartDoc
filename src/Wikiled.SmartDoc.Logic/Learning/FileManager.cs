using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.Logic.Results;

namespace Wikiled.SmartDoc.Logic.Learning
{
    public class FileManager : IFileManager
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly int parallel;

        private readonly IDocumentParser parser;

        private readonly Subject<ProcessingProgress> progressUpdate = new Subject<ProcessingProgress>();

        private CancellationToken token;

        private int total;

        private int totalProcessed;

        public FileManager(IDocumentParser parser, CancellationToken token, int parallel = 1)
        {
            Guard.NotNull(() => parser, parser);
            this.parser = parser;
            this.token = token;
            this.parallel = parallel;
        }

        public IObservable<ProcessingProgress> ProgressUpdate => progressUpdate.AsObservable();

        public async Task<DocumentSet> LoadAll(DirectoryInfo repositoryPath)
        {
            var allFiles = await Task.Run(
                                         () =>
                                             Directory.EnumerateFiles(repositoryPath.FullName, "*", SearchOption.AllDirectories)
                                                      .Where(
                                                          item => string.Compare(
                                                                      new FileInfo(item).Directory.FullName,
                                                                      repositoryPath.FullName,
                                                                      StringComparison.OrdinalIgnoreCase) !=
                                                                  0) // don't take root files
                                                      .ToArray(),
                                         token)
                                     .ConfigureAwait(false);
            total = allFiles.Length;
            totalProcessed = 0;
            progressUpdate.OnNext(new ProcessingProgress(0, total));
            ConcurrentBag<DocumentDefinition> documents = new ConcurrentBag<DocumentDefinition>();
            var chain = Construct(repositoryPath, documents);
            var input = chain.Item1;

            try
            {
                foreach (var file in allFiles)
                {
                    await input.SendAsync(file, token).ConfigureAwait(false);
                }

                input.Complete();
                await Task.WhenAll(chain.Item1.Completion, chain.Item2.Completion).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Error("One or more exceptions occured: " + ex.Message);
            }

            var set = new DocumentSet();
            set.Path = repositoryPath.FullName;
            set.Created = DateTime.Now;
            set.TotalRequested = allFiles.Length;
            set.Document = documents.ToArray();
            progressUpdate.OnCompleted();
            return set;
        }

        private Tuple<ITargetBlock<string>, IDataflowBlock> Construct(DirectoryInfo repositoryPath, ConcurrentBag<DocumentDefinition> documents)
        {
            var inputBlock = new BufferBlock<string>(
                new DataflowBlockOptions
                    {
                        BoundedCapacity = 8,
                        CancellationToken = token
                    });
            var deserializeBlock = new TransformBlock<string, DocumentDefinition>(
                async path =>
                    {
                        try
                        {
                            if (token.IsCancellationRequested)
                            {
                                return null;
                            }

                            return await ParseDocument(repositoryPath, path)
                                       .ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            return null;
                        }
                    },
                new ExecutionDataflowBlockOptions
                    {
                        BoundedCapacity = 1,
                        MaxDegreeOfParallelism = parallel,
                        CancellationToken = token
                    });
            var outputBlock = new ActionBlock<DocumentDefinition>(
                document =>
                    {
                        if (document != null)
                        {
                            documents.Add(document);
                        }
                    },
                new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = 6,
                        CancellationToken = token
                    });

            inputBlock.LinkTo(deserializeBlock, new DataflowLinkOptions { PropagateCompletion = true });
            deserializeBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });
            return new Tuple<ITargetBlock<string>, IDataflowBlock>(inputBlock, outputBlock);
        }

        private async Task<DocumentDefinition> ParseDocument(DirectoryInfo repositoryPath, string file)
        {
            try
            {
                return await parser.ParseDocument(repositoryPath, new FileInfo(file), token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    var current = Interlocked.Increment(ref totalProcessed);
                    progressUpdate.OnNext(new ProcessingProgress(current, total));
                }
            }
        }
    }
}
