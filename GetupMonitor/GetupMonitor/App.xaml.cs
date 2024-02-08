using System.Configuration;
using System.Data;
using System.Threading;
using System.Windows;

namespace GetupMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {      
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        ViewModel.GetupMonitor_VM GM_VM;
        public MonitorWindow window;
        private void AppBase_Startup(object sender, StartupEventArgs e)
        {
            this.window = new MonitorWindow();
            this.GM_VM = new ViewModel.GetupMonitor_VM(window);
           this.GM_VM.WindowTitle = $"GetupMonitor   Ver. {AppVersion}";
            this.window.Show();
            this.GM_VM.ExecuteStateMachine();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            GM_VM.ViewModelQuit();
        }

        public static string AppVersion = "1.0.0.0";

        // 2024/02/08, Ver. 1.0.0.0, State-machine can't use void method otherwise become recursion then stack overflow.
        //                                                Something need to do are error state and bluetooth reconnect.

    }

}
