using System.ComponentModel.Composition;
using Wikiled.SmartDoc.ViewModel;

namespace Wikiled.SmartDoc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [Import]
        public IMainViewModel Model
        {
            get
            {
                return DataContext as IMainViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
