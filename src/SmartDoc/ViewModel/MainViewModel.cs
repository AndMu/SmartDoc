using System.ComponentModel.Composition;
using System.Reflection;

namespace Wikiled.SmartDoc.ViewModel
{
    [Export(typeof(IMainViewModel))]
    public class MainViewModel : IMainViewModel
    {
        public MainViewModel()
        {
            Version = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        public string Version { get; }
    }
}
