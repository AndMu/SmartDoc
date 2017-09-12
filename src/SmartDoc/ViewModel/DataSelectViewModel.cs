using System.ComponentModel.Composition;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using ReactiveUI;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Results;
using Wikiled.SmartDoc.Model;
using Wikiled.SmartDoc.Model.Loaders;
using Wikiled.SmartDoc.ViewModel.Definitions;

namespace Wikiled.SmartDoc.ViewModel
{
    [Export(typeof(IDataSelectViewModel))]
    public class DataSelectViewModel : CancelableViewModel<DocumentSet, TrainedTreeData>, IDataSelectViewModel
    {
        private readonly IFileManagerFactory fileManagerFactory;

        private ObservableAsPropertyHelper<ProcessingProgress> progress;

        [ImportingConstructor]
        public DataSelectViewModel(IDataHandler<DocumentSet> dataHandler, IFileManagerFactory fileManagerFactory, IFolderBrowserDialogService openFolder)
            : base(dataHandler)
        {
            Guard.NotNull(() => openFolder, openFolder);
            Guard.NotNull(() => fileManagerFactory, fileManagerFactory);
            this.fileManagerFactory = fileManagerFactory;
            Select = new SelectableViewModel(openFolder, this);
            var canSearch = this.WhenAny(x => x.Select.Path, x => x.Value != null && Directory.Exists(x.Value));
            var visibility = Observable.Return(Visibility.Visible);
            InitMain(canSearch, visibility);
        }

        public override string Name => "Select Documents";

        public ProcessingProgress Progress => progress?.Value;

        public ISelectableViewModel Select { get; }

        protected override TrainedTreeData ConstructTree(DocumentSet value)
        {
            return TrainedTreeData.ConstructFromDocuments(value);
        }

        protected override async Task<DocumentSet> LoadData()
        {
            var result = await base.LoadData();
            if(result != null)
            {
                Select.Path = result.Path;
            }

            return result;
        }

        protected override async Task<DocumentSet> LoadLogic(CancellationToken token)
        {
            var fileManager = fileManagerFactory.Create(token);
            var tracking = fileManager.ProgressUpdate
                                      .Select(item => item)
                                      .ToProperty(this, model => model.Progress);
            using(tracking)
            {
                progress = tracking;
                var result = await fileManager.LoadAll(new DirectoryInfo(Select.Path));
                if(token.IsCancellationRequested)
                {
                    return await LoadData();
                }

                progress = null;
                return result;
            }
        }
    }
}
