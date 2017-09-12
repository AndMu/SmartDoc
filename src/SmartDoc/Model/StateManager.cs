using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Wikiled.SmartDoc.Annotations;

namespace Wikiled.SmartDoc.Model
{
    [Export(typeof(StateManager))]
    public class StateManager : INotifyPropertyChanged
    {
        private bool isBusy;

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                if (isBusy != value)
                {
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}