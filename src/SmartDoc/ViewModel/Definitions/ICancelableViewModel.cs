using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows;
using ReactiveUI;

namespace Wikiled.SmartDoc.ViewModel.Definitions
{
    public interface ICancelableViewModel<T, TItem> : IViewModelPage
        where T : class 
    {
        T Result { get; }

        ObservableCollection<TItem> SelectedItems { get; } 

        TItem[] TreeData { get; }

        TItem CurrentItem { get; set; }

        Visibility BusyVisibility { get; }

        ReactiveCommand<Unit, T> Perform { get; }

        ReactiveCommand<Unit, T> Cancel { get; }
    }
}
