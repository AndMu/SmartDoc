using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using ReactiveUI.Testing;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Results;
using Wikiled.SmartDoc.Model.Loaders;
using Wikiled.SmartDoc.ViewModel;

namespace Wikiled.SmartDoc.Tests.ViewModel
{
    [TestFixture]
    public class DataSelectViewModelTests
    {
        private DataSelectViewModel instance;

        private Mock<IFileManagerFactory> factory;

        private Mock<IFileManager> fileManager;

        private Mock<IFolderBrowserDialogService> openFolder;

        private CancellationToken token;

        private DocumentSet set;

        private Mock<IDataHandler<DocumentSet>> dataHandler;

        [SetUp]
        public void Setup()
        {
            set = new DocumentSet();
            set.Document = new DocumentDefinition[] { };
            factory = new Mock<IFileManagerFactory>();
            fileManager = new Mock<IFileManager>();
            openFolder = new Mock<IFolderBrowserDialogService>();
            dataHandler = new Mock<IDataHandler<DocumentSet>>();
            factory.Setup(item => item.Create(It.IsAny<CancellationToken>()))
                   .Callback<CancellationToken>(r => token = r)
                   .Returns(fileManager.Object);
            Mock<IObservable<ProcessingProgress>> progresObservable = new Mock<IObservable<ProcessingProgress>>();
            fileManager.Setup(item => item.ProgressUpdate)
                       .Returns(progresObservable.Object);
            openFolder = new Mock<IFolderBrowserDialogService>();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new DataSelectViewModel(null, factory.Object, openFolder.Object));
            Assert.Throws<ArgumentNullException>(() => new DataSelectViewModel(dataHandler.Object, null, openFolder.Object));
            Assert.Throws<ArgumentNullException>(() => new DataSelectViewModel(dataHandler.Object, factory.Object, null));
            Assert.IsNotNull(instance);
        }

        [Test]
        public void CanSearch()
        {
            new TestScheduler().With(
                async scheduler =>
                {
                    instance = new DataSelectViewModel(dataHandler.Object, factory.Object, openFolder.Object);
                    var result = await instance.Perform.CanExecute;
                    Assert.IsFalse(result);
                    instance.Select.Path = ".";
                    scheduler.AdvanceByMs(500);
                    result = await instance.Perform.CanExecute;
                    Assert.IsTrue(result);
                });
        }

        [Test]
        public void Search()
        {
            new TestScheduler().With(
                async scheduler =>
                {
                    instance = new DataSelectViewModel(dataHandler.Object, factory.Object, openFolder.Object);
                    instance.Select.Path = ".";
                    fileManager.Setup(item => item.LoadAll(It.IsAny<DirectoryInfo>()))
                               .Returns(Task.FromResult(set));
                    DocumentSet result = null;
                    instance.Perform.Subscribe(
                        results =>
                        {
                            result = results;
                        });

                    scheduler.AdvanceByMs(500);
                    await instance.Perform.Execute();
                    scheduler.AdvanceByMs(500);
                    Assert.AreEqual(set, result);
                });
        }

        [Test]
        public void CanCancel()
        {
            new TestScheduler().With(
                async scheduler =>
                {
                    instance = new DataSelectViewModel(dataHandler.Object, factory.Object, openFolder.Object);
                    var result = await instance.Cancel.CanExecute;
                    instance.Select.Path = ".";
                    Assert.IsFalse(result);
                    fileManager.Setup(item => item.LoadAll(It.IsAny<DirectoryInfo>()))
                               .Returns(async () => await (Observable.Start(() => set, scheduler).Delay(TimeSpan.FromSeconds(1.0))));
                    scheduler.AdvanceByMs(200);
                    await instance.Perform.Execute();
                    scheduler.AdvanceByMs(200);
                    Assert.IsFalse(token.IsCancellationRequested);
                    result = await instance.Cancel.CanExecute;
                    Assert.IsTrue(result);
                    await instance.Cancel.Execute();
                    scheduler.AdvanceByMs(10);
                    Assert.IsTrue(token.IsCancellationRequested);
                });
        }
    }
}
