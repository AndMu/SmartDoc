using System;
using System.ComponentModel;
using System.Windows;

namespace Wikiled.SmartDoc.ViewModel.Definitions
{
    public interface IViewModelPage : INotifyPropertyChanged
    {
        string Name { get; }

        IObservable<bool> IsExecuting { get; }

        Visibility Visibility { get; }
    }
}