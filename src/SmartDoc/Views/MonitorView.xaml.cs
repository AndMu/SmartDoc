using System.ComponentModel.Composition;
using System.Windows.Input;
using DevExpress.Xpf.Grid;
using Wikiled.SmartDoc.ViewModel;

namespace Wikiled.SmartDoc.Views
{
    /// <summary>
    /// Interaction logic for MonitorView.xaml
    /// </summary>
    [Export]
    public partial class MonitorView
    {
        public MonitorView()
        {
            InitializeComponent();
        }

        [Import]
        public IMonitorViewModel Model
        {
            get
            {
                return DataContext as IMonitorViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = tableView.GetRowHandleByMouseEventArgs(e);
            var column = tableView.GetColumnByMouseEventArgs(e);
            if (rowHandle == DataControlBase.InvalidRowHandle ||
                documentsGrid.IsGroupRowHandle(rowHandle) ||
                column.VisibleIndex < 1)
            {
                return;
            }
             
            Model.DoubleClicked();
        }
    }
}
