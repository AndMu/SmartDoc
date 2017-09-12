using System;
using System.Reactive;
using ReactiveUI;

namespace Wikiled.SmartDoc.ViewModel.Definitions
{
    public interface ISelectableViewModel : IDisposable
    {
        ReactiveCommand<Unit, Unit> Open { get; }

        string Path { get; set; }

        IObservable<string> PathData { get; }
    }
}
