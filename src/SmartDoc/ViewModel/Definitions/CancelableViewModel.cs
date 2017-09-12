using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using ReactiveUI;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Model.Loaders;

namespace Wikiled.SmartDoc.ViewModel.Definitions
{
    public abstract class CancelableViewModel<T, TItem> : ReactiveObject, ICancelableViewModel<T, TItem>
        where T : class
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDataHandler<T> dataHandler;

        private ObservableAsPropertyHelper<bool> canEdit;

        private CancellationTokenSource cancelToken;

        private ObservableAsPropertyHelper<Visibility> isActive;

        private ObservableAsPropertyHelper<Visibility> isBusy;

        private ObservableAsPropertyHelper<T> result;

        private ObservableAsPropertyHelper<TItem[]> treeData;

        private IObservable<bool> isExecuting;

        protected CancelableViewModel(IDataHandler<T> dataHandler)
        {
            Guard.NotNull(() => dataHandler, dataHandler);
            this.dataHandler = dataHandler;
        }

        public abstract string Name { get; }

        public IObservable<bool> IsExecuting
        {
            get => isExecuting;
            set => this.RaiseAndSetIfChanged(ref isExecuting, value);
        }

        public Visibility BusyVisibility => isBusy.Value;

        public ReactiveCommand<Unit, T> Cancel { get; private set; }

        public CancellationToken Token => CancelToken?.Token ?? CancellationToken.None;

        public CancellationTokenSource CancelToken
        {
            get => cancelToken;
            set => this.RaiseAndSetIfChanged(ref cancelToken, value);
        }

        public bool CanEdit => canEdit.Value;

        public TItem CurrentItem { get; set; }

        public ReactiveCommand<Unit, T> Perform { get; private set; }

        public T Result => result?.Value;

        public ObservableCollection<TItem> SelectedItems { get; } = new ObservableCollection<TItem>();

        public TItem[] TreeData => treeData?.Value;

        public Visibility Visibility => isActive?.Value ?? Visibility.Collapsed;

        protected abstract TItem ConstructTree(T value);

        protected abstract Task<T> LoadLogic(CancellationToken token);

        protected virtual Task<T> LoadData()
        {
            return dataHandler.Load(Token);
        }

        protected void InitMain(IObservable<bool> canExecute, IObservable<Visibility> visibility)
        {
            canExecute = canExecute.ObserveOn(RxApp.MainThreadScheduler);
            visibility = visibility.ObserveOn(RxApp.MainThreadScheduler);
            Cancel = ReactiveCommand.CreateFromTask(
                _ =>
                {
                    OnCancel();
                    return Task.FromResult(default(T));
                },
                this.WhenAny(x => x.CancelToken, x => x.Value != null).ObserveOn(RxApp.MainThreadScheduler));

            Perform = ReactiveCommand.CreateFromObservable(Load, canExecute);
            Perform.ThrownExceptions.Subscribe(
                ex =>
                {
                    Logger.Error(ex);
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                });

            var data = LoadData().ToObservable()
                                 .Concat(Perform);

            // add this which if executing is TRUE, then we reset value
            var merged = data.Merge(
                Perform.IsExecuting.Where(item => item)
                       .Select(item => default(T)))
                       .ObserveOn(RxApp.MainThreadScheduler);

            // fire event - reset data if control is hidden
            merged = merged.Merge(
                visibility.Where(item => item == Visibility.Collapsed)
                          .Select(item => default(T)));

            visibility.Where(item => item == Visibility.Collapsed).Subscribe(_ => Reset());

            // also reset if cancel pressed
            merged = merged.Merge(Cancel);

            merged.ToProperty(this, model => model.Result, out result);

            isBusy = Perform.IsExecuting
                            .Select(item => item ? Visibility.Visible : Visibility.Collapsed)
                            .ToProperty(this, model => model.BusyVisibility);

            isActive = visibility.StartWith(Visibility.Collapsed)
                                 .Select(item => item)
                                 .ToProperty(this, model => model.Visibility);

            treeData = merged
                .Select(item => item == null ? new TItem[] { } : new[] { ConstructTree(item) })
                .ToProperty(this, model => model.TreeData);

            canEdit = Perform.IsExecuting
                            .Select(item => !item)
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .ToProperty(this, model => model.CanEdit);

            IsExecuting = Perform.IsExecuting;
        }

        protected virtual void Reset()
        {
            OnCancel();
        }

        private IObservable<T> Load()
        {
            CancelToken = new CancellationTokenSource();
            var subject = new Subject<T>();
            // Run something in the background
            Observable.Start(
                async () =>
                {
                    try
                    {
                        var data = await LoadTask();
                        subject.OnNext(data);
                        subject.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        subject.OnError(ex);
                    }
                },
                RxApp.TaskpoolScheduler);

            return subject;
        }

        private async Task<T> LoadTask()
        {
            try
            {
                var data = await LoadLogic(Token);
                await dataHandler.Save(data, Token);
                return data;
            }
            catch (TaskCanceledException)
            {
                Logger.Debug("Task cancelled");
                return default(T);
            }
            finally
            {
                CancelToken = null;
            }
        }

        private void OnCancel()
        {
            CancelToken?.Cancel();
            CancelToken = null;
        }
    }
}
