using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using NLog;
using ReactiveUI;
using Wikiled.Arff.Persistence.Headers;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Logic.Monitoring;
using Wikiled.SmartDoc.Model.Loaders;
using Wikiled.SmartDoc.Properties;
using Wikiled.SmartDoc.ViewModel.Definitions;

namespace Wikiled.SmartDoc.ViewModel
{
    [Export(typeof(IMonitorViewModel))]
    public class MonitorViewModel : CancelableViewModel<IFileMonitor, MonitoringResult>, IMonitorViewModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IDataSelectViewModel dataViewModel;

        private readonly IFileMonitorFactory fileMonitorFactory;

        private readonly ObservableCollection<MonitoringResult> resultList = new ObservableCollection<MonitoringResult>();

        private readonly ITrainingViewModel trainingViewModel;

        private readonly ObservableAsPropertyHelper<string[]> classes;

        [ImportingConstructor]
        public MonitorViewModel(
            IDataSelectViewModel dataViewModel,
            ITrainingViewModel trainingViewModel,
            IFileMonitorFactory fileMonitorFactory,
            IFolderBrowserDialogService folderBrowserDialog)
            : base(new NullHandler<IFileMonitor>())
        {
            Guard.NotNull(() => trainingViewModel, trainingViewModel);
            Guard.NotNull(() => fileMonitorFactory, fileMonitorFactory);
            Guard.NotNull(() => dataViewModel, dataViewModel);
            this.dataViewModel = dataViewModel;
            this.trainingViewModel = trainingViewModel;
            this.fileMonitorFactory = fileMonitorFactory;
            Monitor = new SelectableViewModel(folderBrowserDialog, this, Observable.Return(Settings.Default.MonitorPath));
            Destination = new SelectableViewModel(folderBrowserDialog, path: dataViewModel.Select.PathData);
            var canMonitor = this.WhenAny(x => x.Monitor.Path, y => y.Result, (x, y) => x.Value != null && Directory.Exists(x.Value) && y.Value == null);
            this.WhenAnyValue(x => x.Monitor.Path)
                .Where(x => x != Settings.Default.MonitorPath)
                .Subscribe(
                    x =>
                    {
                        Settings.Default.MonitorPath = x;
                        Settings.Default.Save();
                    });

            var visibility = this.WhenAny(
                item => item.trainingViewModel.Result,
                item => item.dataViewModel.Result,
                (model1, model2) => model1.Value == null || model2.Value == null ? Visibility.Collapsed : Visibility.Visible);

            classes = this.WhenAny(item => item.trainingViewModel.Result, model => ((NominalHeader)model.Value?.DataSet?.Header?.Class)?.Nominals).ToProperty(this, model => model.Classes);

            InitMain(canMonitor, visibility);
            PendingFiles = resultList.CreateDerivedCollection(x => x, scheduler: RxApp.MainThreadScheduler);
            var selected = SelectedItems.CreateDerivedCollection(x => x);
            var canMove = selected.Changed.StartWith().Select(_ => selected.Count > 0);
            Move = ReactiveCommand.Create(MoveSelected, canMove);
            Move.Subscribe(o => { MoveSelected(); });
            Move.ThrownExceptions.Subscribe(
                ex =>
                {
                    MessageBox.Show("Move of some items failed", "Error", MessageBoxButton.OK);
                });
        }

        public string[] Classes => classes.Value;

        public override string Name => "Monitor";

        public ReactiveCommand<Unit, Unit> Move { get; }

        public IReactiveDerivedList<MonitoringResult> PendingFiles { get; }

        public ISelectableViewModel Monitor { get; }

        public ISelectableViewModel Destination { get; }

        public void DoubleClicked()
        {
            if (CurrentItem != null)
            {
                Process.Start(CurrentItem.File.FullName);
            }
        }

        protected override MonitoringResult ConstructTree(IFileMonitor value)
        {
            throw new NotSupportedException();
        }

        protected override async Task<IFileMonitor> LoadLogic(CancellationToken token)
        {
            logger.Debug("LoadLogic");
            using (var monitor = fileMonitorFactory.Create(Monitor.Path, trainingViewModel.Result))
            {
                await monitor.Start(resultList, TimeSpan.FromSeconds(10), token).ConfigureAwait(false);
                await Task.Delay(int.MaxValue, token).ConfigureAwait(false);
                return monitor;
            }
        }

        protected override void Reset()
        {
            base.Reset();
            resultList.Clear();
        }

        private void MoveSelected()
        {
            logger.Debug("MoveSelected");
            if (string.IsNullOrWhiteSpace(Destination.Path))
            {
                logger.Warn("Destination is null. Can't continue");
                return;
            }

            var destination = new DirectoryInfo(Destination.Path);
            var items = SelectedItems.ToArray();
            bool failed = false;
            foreach (var item in items)
            {
                try
                {
                    item.ApplyChange(destination);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    failed = true;
                }
            }

            if (failed)
            {
                throw new InvalidOperationException("Move of some items failed");
            }
        }
    }
}
