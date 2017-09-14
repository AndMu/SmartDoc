using System.Reactive.Concurrency;
using System.Windows;
using DevExpress.Xpf.Core;
using ReactiveUI;
using Wikiled.SmartDoc.Setup;
using Wikiled.SmartDoc.Views.SplashScreen;

namespace Wikiled.SmartDoc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private Bootstrapper bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RxApp.MainThreadScheduler = new DispatcherScheduler(Dispatcher);
            DXSplashScreen.Show<StartScreen>();
            ApplicationThemeHelper.UpdateApplicationThemeName();
            bootstrapper = new Bootstrapper();
            bootstrapper.Run();
            Current.MainWindow.Show();
            DXSplashScreen.Close();
        }

        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {
        }
    }
}
