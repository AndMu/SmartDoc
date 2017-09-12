using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using Prism.Mef;
using Wikiled.MachineLearning.Svm.Logic;
using Wikiled.Sentiment.Analysis.Processing.Splitters;
using Wikiled.SmartDoc.Logic.Learning;
using Wikiled.SmartDoc.Logic.Monitoring;
using Wikiled.SmartDoc.Logic.Pdf;
using Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress;
using Wikiled.SmartDoc.Logic.Results;
using Wikiled.SmartDoc.Model.Loaders;

namespace Wikiled.SmartDoc.Setup
{
    public class Bootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<MainWindow>();
        }

        protected override void ConfigureAggregateCatalog()
        {
            var registration = new RegistrationBuilder();
            registration.ForType<FolderBrowserDialogService>()
                        .Export<IFolderBrowserDialogService>();

            registration.ForType<TrainingManager>()
                        .Export<ITrainingManager>();

            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(FolderBrowserDialogService).Assembly, registration));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(TrainingManager).Assembly, registration));
            base.ConfigureAggregateCatalog();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            LightSplitterHelper helper = new LightSplitterHelper();
            helper.Load();
            var documentParser = new DocumentParser(helper, new DevExpressParserFactory(20));
            var factory = new FileManagerFactory(documentParser);
            Container.ComposeExportedValue<IFileManagerFactory>(factory);
            FileMonitorFactory monitorFactory = new FileMonitorFactory(documentParser);
            Container.ComposeExportedValue<IFileMonitorFactory>(monitorFactory);

            var documentHandler = new DocumentSetHandler(System.IO.Path.Combine(".", "documents.dat"));
            Container.ComposeExportedValue<IDataHandler<DocumentSet>>(documentHandler);

            var trainingResultLoader = new TrainingResultsHandler("Learning");
            Container.ComposeExportedValue<IDataHandler<TrainingResults>>(trainingResultLoader);
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = (MainWindow)Shell;
        }
    }
}
