using System;
using System.ComponentModel;
using System.Windows;
using DevExpress.Mvvm;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using ReactiveUI.Testing;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.SmartDoc.Logic.Monitoring;
using Wikiled.SmartDoc.Logic.Results;
using Wikiled.SmartDoc.TestHelpers.Data;
using Wikiled.SmartDoc.ViewModel;
using Wikiled.SmartDoc.ViewModel.Definitions;

namespace Wikiled.SmartDoc.Tests.ViewModel
{
    [TestFixture]
    public class MonitorViewModelTests
    {
        private Mock<ITrainingViewModel> trainingViewModel;

        private Mock<IFileMonitorFactory> fileMonitorFactory;

        private Mock<IFolderBrowserDialogService> fileFolder;

        private Mock<IDataSelectViewModel> dataViewModel;

        private MonitorViewModel instance;

        private TrainingResults training;

        private Mock<ISelectableViewModel> seletableViewModel;

        [SetUp]
        public void Setup()
        {
            dataViewModel = new Mock<IDataSelectViewModel>();
            trainingViewModel = new Mock<ITrainingViewModel>();
            seletableViewModel = new Mock<ISelectableViewModel>();
            dataViewModel.Setup(item => item.Select).Returns(seletableViewModel.Object);
            fileMonitorFactory = new Mock<IFileMonitorFactory>();
            fileFolder = new Mock<IFolderBrowserDialogService>();
            training = TestConstants.GetTrainingResults();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new MonitorViewModel(null, trainingViewModel.Object, fileMonitorFactory.Object, fileFolder.Object));
            Assert.Throws<ArgumentNullException>(() => new MonitorViewModel(dataViewModel.Object, null, fileMonitorFactory.Object, fileFolder.Object));
            Assert.Throws<ArgumentNullException>(() => new MonitorViewModel(dataViewModel.Object, trainingViewModel.Object, null, fileFolder.Object));
            Assert.Throws<ArgumentNullException>(() => new MonitorViewModel(dataViewModel.Object, trainingViewModel.Object, fileMonitorFactory.Object, null));
            Assert.IsNotNull(instance);
        }

        [Test]
        public void CheckVisibility()
        {
            new TestScheduler().With(
                scheduler =>
                {
                    instance = new MonitorViewModel(dataViewModel.Object, trainingViewModel.Object, fileMonitorFactory.Object, fileFolder.Object);
                    Assert.AreEqual(Visibility.Collapsed, instance.Visibility);
                    trainingViewModel.Setup(item => item.Result).Returns(training);
                    trainingViewModel.Raise(item => item.PropertyChanged += null, new PropertyChangedEventArgs("Result"));
                    scheduler.AdvanceByMs(200);
                    Assert.AreEqual(Visibility.Collapsed, instance.Visibility);

                    dataViewModel.Setup(item => item.Result).Returns(new DocumentSet());
                    dataViewModel.Raise(item => item.PropertyChanged += null, new PropertyChangedEventArgs("Result"));
                    scheduler.AdvanceByMs(200);
                    Assert.AreEqual(Visibility.Visible, instance.Visibility);

                    trainingViewModel.Setup(item => item.Result).Returns((TrainingResults)null);
                    trainingViewModel.Raise(item => item.PropertyChanged += null, new PropertyChangedEventArgs("Result"));
                    scheduler.AdvanceByMs(200);
                    Assert.AreEqual(Visibility.Collapsed, instance.Visibility);
                });
        }
    }
}
