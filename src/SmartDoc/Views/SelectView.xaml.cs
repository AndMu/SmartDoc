using System.ComponentModel.Composition;
using NLog;
using Wikiled.SmartDoc.Model;
using Wikiled.SmartDoc.ViewModel;

namespace Wikiled.SmartDoc.Views
{
    /// <summary>
    /// Interaction logic for TrainingView.xaml
    /// </summary>
    [Export]
    public partial class SelectView
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public SelectView()
        {
            InitializeComponent();
        }

        [Import]
        public IDataSelectViewModel Model
        {
            get
            {
                return DataContext as IDataSelectViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        private void treeList_ItemsSourceChanged(object sender, DevExpress.Xpf.Grid.ItemsSourceChangedEventArgs e)
        {
            logger.Debug("treeList_ItemsSourceChanged");
            Model.SelectedItems.Clear();
            treeList.View.CheckAllNodes();
        }

        private void View_NodeCheckStateChanged(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeEventArgs e)
        {
            logger.Debug("View_NodeCheckStateChanged");
            var item = (TrainedTreeData)e.Row;
            if (e.Node.IsChecked == true)
            {
                logger.Debug("Selecting: {0}", item.Name);
                Model.SelectedItems.Add(item);
            }
            else
            {
                logger.Debug("Unselecting: {0}", item.Name);
                Model.SelectedItems.Remove(item);
            }
        }
    }
}
