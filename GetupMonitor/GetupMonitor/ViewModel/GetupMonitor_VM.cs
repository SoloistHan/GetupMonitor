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
using ConfigBuild_dotNet;
using System.Windows.Controls;
using System.Windows;
using System.Media;
using System.Diagnostics;

namespace GetupMonitor.ViewModel
{
    public enum GetupMonitorStates { StateBranch, StateIdle, StateCheckConnection, StateMonitor, StateNotify, StateSTOP, StateError, StateTimeout,None }
    internal partial class GetupMonitor_VM : INotifyPropertyChanged
    {
        MonitorWindow _MonitorWindow;        
        public GetupMonitor_VM(MonitorWindow View)
        {
            _MonitorWindow = View;
            _MonitorWindow.DataContext = this;
            
           
            readConfig();
            pageMain_Initial();
            async_bluetoothMonitor();
            async_getDataLoop();
            monitor_Initial();
        }

        const string Audio_Active = "Audio_Active", Single_Mode = "Single_Mode", StableValue = "StableValue", Sample_Interval = "SampleInterval";
        const string Minimum_A0 = "Minimum_A0", Maximum_A0 = "Maximum_A0", Minimum_A1 = "Minimum_A1", Maximum_A1 = "Maximum_A1";

        ConfigBuild cb = new ConfigBuild();
        DateTime logCounting;
        DispatcherTimer runtimeTimer;
        Dictionary<string, string> configDic = new Dictionary<string, string>();
        Dictionary<string, int> subItem = new Dictionary<string, int>();
        private void readConfig()
        {
            try
            {
                configDic.Add(Audio_Active, "true");
                configDic.Add(Single_Mode, "false");
                configDic.Add(StableValue, "3");
                configDic.Add(Sample_Interval, "300");
                configDic.Add(Minimum_A0, "1");
                configDic.Add(Maximum_A0, "10");
                configDic.Add(Minimum_A1, "1");
                configDic.Add(Maximum_A1, "10");

                Dictionary<string, string> _configDic = new Dictionary<string, string>(configDic);
                foreach (KeyValuePair<string, string> getconfig in configDic)
                    _configDic[getconfig.Key] = cb.ReadConfig(getconfig.Key, configDic);

                configDic = new Dictionary<string, string>(_configDic);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + $"\nConfigBuild error : {ConfigBuild.ErrorMessage}\n Head Convert error : {HeadFileConvert.ErrorMessage}");
            }
        }

        private void monitor_Initial()
        {
            logCounting = DateTime.Now;
            runtimeTimer = new DispatcherTimer();
            runtimeTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            runtimeTimer.Tick += new EventHandler(runtimeMonitor);
            runtimeTimer.Start();
        }

        private void runtimeMonitor(object sender, EventArgs e)
        {
            try
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
                    //getDataIR();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        #region Async Get Data IR

        private async void async_getDataLoop()
        {
            getDataTrigger = true;
            await task_getDataLoop();
        }
        private Task<bool> task_getDataLoop()
        {
            return Task.Factory.StartNew(() => getDataLoop());
        }
        private bool getDataLoop()
        {
            while(getDataTrigger)
            {                
                getDataIR();
            }
            return true;
        }
        #endregion


        public void ViewModelQuit()
        {
            stateMachineTrigger = false;
            systemMonitorTrigger = false;
            getDataTrigger = false;
        }

       

        #region INotifyPropertyChanged Implementation
        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the NotifyPropertyChanged method to raise the event 
        protected void NotifyPropertyChanged(string name)
        {
            this.VerifyPropertyName(name);
            PropertyChangedEventHandler handler = PropertyChanged;
            
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(name);
                this.PropertyChanged(this, e);
                //handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion INotifyPropertyChanged Implementation
        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public virtual void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides
    }
}
