using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GetupMonitor.ViewModel
{
    public enum GetupMonitorStates { StateBranch, StateIdle, StateStartUp, StateMonitor, StateNotify, StateError, StateTimeout }
    internal partial class GetupMonitor_VM : INotifyPropertyChanged
    {
        MonitorWindow _MonitorWindow;
        public GetupMonitor_VM(MonitorWindow View)
        {
            _MonitorWindow = View;
            _MonitorWindow.DataContext = this;
            async_bluetoothMonitor();
            monitor_Initial();
        }      
      
        BluetoothClient BTclient = new BluetoothClient();
        BluetoothRadio BTradio = BluetoothRadio.Default;


        const string MAJOR_BLUETOOTH = "MAJOR III BLUETOOTH";
        const string HC06 = "HC-06";
        byte[] CmdGetIR_A0 = { 0xFA, 0xA0, 0xFF };
        byte[] CmdGetIR_A1 = { 0xFA, 0xA1, 0xFF };


        DateTime logCounting;
        DispatcherTimer runtimeTimer;
        private void monitor_Initial()
        {
            logCounting = DateTime.Now;
            runtimeTimer = new DispatcherTimer();
            runtimeTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            runtimeTimer.Tick += runtimeMonitor;
            runtimeTimer.Start();
        }

        private void runtimeMonitor(object sender, EventArgs e)
        {
            DateDisplay = DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss");
            bool over1s = string.Compare(DateTime.Now.ToString("hh:mm:ss"), logCounting.ToString("hh:mm:ss")) != 0;
            TimeSpan over5s = new TimeSpan(DateTime.Now.Ticks - logCounting.Ticks);

            if (!BTclient.Connected)
            {
                TitleLED = LedColor.Red;
                BlueToothState = runtimeDisplay("Searching HC-06");
            }
            else
            {
                TitleLED = LedColor.Green;
                BlueToothState = "HC-06 Connected";
            }
            //if ((CurrentStateName == SystemStates.StateFunctionTest.ToString() || CurrentStateName == SystemStates.StateFinish.ToString())
            //    && over1s)
            //{
            //    writeLog();
            //    logCounting = DateTime.Now;
            //}
            //else if (over5s.TotalSeconds >= 5)
            //{
            //    writeLog();
            //    logCounting = DateTime.Now;
            //}

        }

        public void ViewModelQuit()
        {
            stateMachineTrigger = false;
            systemMonitorTrigger = false;
        }

       

        #region INotifyPropertyChanged Implementation
        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the NotifyPropertyChanged method to raise the event 
        protected void NotifyPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion INotifyPropertyChanged Implementation
    }
}
