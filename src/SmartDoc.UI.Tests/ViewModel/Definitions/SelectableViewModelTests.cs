using System;
using System.Reactive;
using System.Reactive.Linq;
using DevExpress.Mvvm;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using ReactiveUI.Testing;
using Wikiled.SmartDoc.ViewModel.Definitions;

namespace Wikiled.SmartDoc.Tests.ViewModel.Definitions
{
    [TestFixture]
    public class SelectableViewModelTests
    {
        private SelectableViewModel instance;

        private Mock<IFolderBrowserDialogService> openFolder;

        private Mock<IViewModelPage> parent;

        [SetUp]
        public void Setup()
        {
            openFolder = new Mock<IFolderBrowserDialogService>();
            parent = new Mock<IViewModelPage>();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new SelectableViewModel(null, parent.Object, Observable.Return("Test")));
            instance = new SelectableViewModel(openFolder.Object, parent.Object, Observable.Return("Test"));
            Assert.AreEqual("Test", instance.Path);
        }

          [Test]
        public void CanExecute()
        {
            new TestScheduler().With(
                async scheduler =>
                {
                    var isExecuting = scheduler.CreateColdObservable(
                        new Recorded<Notification<bool>>(0, Notification.CreateOnNext(false)),
                        new Recorded<Notification<bool>>(TimeSpan.FromMilliseconds(200).Ticks, Notification.CreateOnNext(true)));
                    parent.Setup(item => item.IsExecuting).Returns(isExecuting);
                    instance = new SelectableViewModel(openFolder.Object, parent.Object, Observable.Return("Test"));
                    scheduler.AdvanceByMs(100);
                    var result = await instance.Open.CanExecute;
                    Assert.True(result);
                    scheduler.AdvanceByMs(500);
                    result = await instance.Open.CanExecute;
                    Assert.IsFalse(result);
                });
        }
    }
}
