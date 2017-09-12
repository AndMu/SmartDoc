using System.ComponentModel.Composition;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Views;

namespace Wikiled.SmartDoc.Setup
{
    [Module(ModuleName = "RootModule")]
    [ModuleExport(typeof(RootModule))]
    public class RootModule : IModule
    {
        private readonly IRegionManager manager;

        [ImportingConstructor]
        public RootModule(IRegionManager manager)
        {
            Guard.NotNull(() => manager, manager);
            this.manager = manager;
        }

        public void Initialize()
        {
            manager.RegisterViewWithRegion("MainRegion", typeof(SelectView));
            manager.RegisterViewWithRegion("MainRegion", typeof(LearningView));
            manager.RegisterViewWithRegion("MainRegion", typeof(MonitorView));
        }
    }
}
