using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using ReactiveUI.Testing;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Results;
using Wikiled.SmartDoc.Model;
using Wikiled.SmartDoc.Model.Loaders;
using Wikiled.SmartDoc.TestHelpers.Data;
using Wikiled.SmartDoc.ViewModel;

namespace Wikiled.SmartDoc.Tests.ViewModel
{
    [TestFixture]
    public class TrainingViewModelTests
    {
        private TrainingViewModel viewModel;

        private Mock<IDataSelectViewModel> dataViewModel;

        private Mock<ITrainingManager> trainingManager;

        private Mock<IDataHandler<TrainingResults>> dataHandler;

        private DocumentSet documentSet;

        private TrainingResults training;

        private StateManager manager;

        [SetUp]
        public void Setup()
        {
            manager = new StateManager();
            training = TestConstants.GetTrainingResults();
            dataViewModel = new Mock<IDataSelectViewModel>();
            trainingManager = new Mock<ITrainingManager>();
            dataHandler = new Mock<IDataHandler<TrainingResults>>();
            documentSet = new DocumentSet();
            documentSet.Document = new[] { new DocumentDefinition { Labels = new[] { "Test" } } };
        }

        [Test]
        public void Create()
        {
            Assert.Throws<ArgumentNullException>(() => new TrainingViewModel(null, dataViewModel.Object, trainingManager.Object, manager));
            Assert.Throws<ArgumentNullException>(() => new TrainingViewModel(dataHandler.Object, null, trainingManager.Object, manager));
            Assert.Throws<ArgumentNullException>(() => new TrainingViewModel(dataHandler.Object, dataViewModel.Object, null, manager));
            Assert.Throws<ArgumentNullException>(() => new TrainingViewModel(dataHandler.Object, dataViewModel.Object, trainingManager.Object, null));
            Assert.IsNotNull(viewModel);
        }

        [Test]
        public void ChangeVisibility()
        {
            new TestScheduler().With(
                scheduler =>
                {
                    viewModel = new TrainingViewModel(dataHandler.Object, dataViewModel.Object, trainingManager.Object, manager);
                    Assert.AreEqual(Visibility.Collapsed, viewModel.Visibility);
                    dataViewModel.Setup(item => item.Result).Returns(documentSet);
                    dataViewModel.Raise(item => item.PropertyChanged += null, new PropertyChangedEventArgs("Result"));
                    scheduler.AdvanceByMs(500);
                    Assert.AreEqual(Visibility.Visible, viewModel.Visibility);
                });
        }

        [Test]
        public void Train()
        {
            new TestScheduler().With(
                async scheduler =>
                {
                    viewModel = new TrainingViewModel(dataHandler.Object, dataViewModel.Object, trainingManager.Object, manager);
                    var collection = new ObservableCollection<TrainedTreeData>();
                    var tree = TrainedTreeData.ConstructFromDocuments(new DocumentSet {Document = new[] {new DocumentDefinition {Labels = new[] {"test"}}}});
                    collection.Add(tree);
                    dataViewModel.Setup(item => item.Result).Returns(documentSet);
                    dataViewModel.Setup(item => item.SelectedItems).Returns(collection);

                    trainingManager.Setup(item => item.Train(It.IsAny<DocumentSet>(), It.IsAny<TrainingHeader>(), It.IsAny<CancellationToken>()))
                                   .Returns(Task.FromResult(training));
                    TrainingResults result = null;
                    viewModel.Perform.Subscribe(
                        results =>
                        {
                            result = results;
                        });

                    scheduler.AdvanceByMs(500);
                    await viewModel.Perform.Execute();
                    scheduler.AdvanceByMs(500);
                    Assert.AreEqual(training, result);
                });
        }
    }
}
