using System.ComponentModel.Composition;
using Wikiled.SmartDoc.ViewModel;

namespace Wikiled.SmartDoc.Views
{
    /// <summary>
    /// Interaction logic for LearningView.xaml
    /// </summary>
    [Export]
    public partial class LearningView
    {
        public LearningView()
        {
            InitializeComponent();
        }

        [Import]
        public ITrainingViewModel Model
        {
            get
            {
                return DataContext as ITrainingViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
