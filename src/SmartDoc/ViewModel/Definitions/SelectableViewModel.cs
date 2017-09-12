using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DevExpress.Mvvm;
using ReactiveUI;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.SmartDoc.ViewModel.Definitions
{
    public class SelectableViewModel : ReactiveObject, ISelectableViewModel
    {
        private readonly IViewModelPage parent;

        private readonly Subject<string> pathResults = new Subject<string>();

        private readonly IFolderBrowserDialogService openFolder;

        private string path;

        public SelectableViewModel(IFolderBrowserDialogService openFolder, IViewModelPage parent = null, IObservable<string> path = null)
        {
            Guard.NotNull(() => openFolder, openFolder);
            this.parent = parent;
            this.openFolder = openFolder;
            if (parent != null)
            {
                var canSelectSearch = this.WhenAnyObservable(x => x.parent.IsExecuting)
                                          .Select(x => !x)
                                          .ObserveOn(RxApp.MainThreadScheduler);
                Open = ReactiveCommand.Create(OpenLogic, canSelectSearch);
            }
            else
            {
                Open = ReactiveCommand.Create(OpenLogic);
            }

            path?.Subscribe(value => Path = value);
        }

        public ReactiveCommand<Unit, Unit> Open { get; }

        public string Path
        {
            get => path;
            set
            {
                pathResults.OnNext(value);
                this.RaiseAndSetIfChanged(ref path, value);
            }
        }

        public IObservable<string> PathData => pathResults.AsObservable();

        public void Dispose()
        {
            pathResults.OnCompleted();
            pathResults.Dispose();
        }

        private void OpenLogic()
        {
            openFolder.StartPath = Path;
            if (openFolder.ShowDialog())
            {
                Path = openFolder.ResultPath;
            }
        }
    }
}
