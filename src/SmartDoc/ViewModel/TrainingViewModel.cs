using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using ReactiveUI;
using Wikiled.Arff.Normalization;
using Wikiled.Core.Utility.Arguments;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Results;
using Wikiled.SmartDoc.Model;
using Wikiled.SmartDoc.Model.Loaders;
using Wikiled.SmartDoc.ViewModel.Definitions;

namespace Wikiled.SmartDoc.ViewModel
{
    [Export(typeof(ITrainingViewModel))]
    public class TrainingViewModel : CancelableViewModel<TrainingResults, TrainedTreeData>, ITrainingViewModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IDataSelectViewModel dataViewModel;

        private readonly ITrainingManager trainingManager;

        private bool gridSelection;

        [ImportingConstructor]
        public TrainingViewModel(IDataHandler<TrainingResults> dataHandler, IDataSelectViewModel dataViewModel, ITrainingManager trainingManager, StateManager stateManager)
            : base(dataHandler)
        {
            Guard.NotNull(() => dataViewModel, dataViewModel);
            Guard.NotNull(() => trainingManager, trainingManager);
            Guard.NotNull(() => stateManager, stateManager);
            
            // http://log.paulbetts.org/creating-viewmodels-with-reactiveobject/
            this.trainingManager = trainingManager;
            this.dataViewModel = dataViewModel;

            var canSearch = this.WhenAny(item => item.dataViewModel.Result, model => model.Value != null);
            var visibility = this.WhenAny(item => item.dataViewModel.Result, model => model.Value == null ? Visibility.Collapsed : Visibility.Visible);

            InitMain(canSearch,  visibility);
            this.WhenAnyValue(item => item.Result)
                .Select(item => item)
                .Subscribe(item => GridSelection = item?.Header?.GridSelection ?? true);
        }

        public bool GridSelection
        {
            get => gridSelection;
            set => this.RaiseAndSetIfChanged(ref gridSelection, value);
        }

        public override string Name => "Learning";

        protected override async Task<TrainingResults> LoadLogic(CancellationToken token)
        {
            logger.Debug("Learn");
            var header = TrainingHeader.CreateDefault();
            header.Normalization = NormalizationType.L2;
            header.GridSelection = GridSelection;
            if(dataViewModel.SelectedItems == null ||
               dataViewModel.SelectedItems.Count == 0)
            {
                throw new InvalidDataException("Data not selected");
            }

            var set = dataViewModel.Result.GetFiltered(dataViewModel.SelectedItems.Select(item => item.Name).ToArray());
            var result = await trainingManager.Train(set, header, token).ConfigureAwait(false);
            return result;
        }

        protected override TrainedTreeData ConstructTree(TrainingResults value)
        {
            return TrainedTreeData.Construct(value.DataSet);
        }
    }
}
