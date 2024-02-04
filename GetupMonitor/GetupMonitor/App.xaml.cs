using System.Configuration;
using System.Data;
using System.Windows;

namespace GetupMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ViewModel.GetupMonitor_VM GM_VM;
        public MonitorWindow window;
        private void AppBase_Startup(object sender, StartupEventArgs e)
        {
            this.window = new MonitorWindow();
            this.GM_VM = new ViewModel.GetupMonitor_VM(window);
           
            this.window.Show();
            this.GM_VM.ExecuteStateMachine();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            GM_VM.ViewModelQuit();
        }
    }

}
