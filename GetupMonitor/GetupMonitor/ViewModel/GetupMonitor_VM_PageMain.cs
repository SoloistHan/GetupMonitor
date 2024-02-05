using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetupMonitor.ViewModel
{
    public enum LedColor { Red = -1, None = 0, Green = 1 }
    internal partial class GetupMonitor_VM
    {
        private void pageMain_Initial()
        {
            AudioActive = Convert.ToBoolean( configDic[Audio_Active]);
            SingleMode = Convert.ToBoolean(configDic[Single_Mode]);
            MinimumIR_A0 =  configDic[Minimum_A0];
            MaximumIR_A0 = configDic[Maximum_A0];
            MinimumIR_A1 = configDic[Minimum_A1];
            MaximumIR_A1 = configDic[Maximum_A1];
        }

        private bool _IsIdle = true;
        public bool IsIdle
        {
            get { return _IsIdle; }
            set
            {
                _IsIdle = value;
                NotifyPropertyChanged("IsIdle");
            }
        }
        #region Bluetooth
        private string _BlueToothState = "BlueToothState";
        public string BlueToothState
        {
            get { return _BlueToothState; }
            set 
            {
                _BlueToothState = value;
                NotifyPropertyChanged("BlueToothState");
            }
        }
        private LedColor _TitleLED = LedColor.Red;
        public LedColor TitleLED
        {
            get { return _TitleLED; }
            set
            {
                _TitleLED = value;
                NotifyPropertyChanged("TitleLED");
            }
        }
        #endregion

        #region Blue Bar
        private string _DateDisplay = DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss");
        public string DateDisplay
        {
            get { return _DateDisplay; }
            set
            {
                _DateDisplay = value;
                NotifyPropertyChanged("DateDisplay");
            }
        }

        private string _MachineState = "MachineState";
        public string MachineState
        {
            get { return _MachineState; }
            set
            {
                _MachineState = value;
                NotifyPropertyChanged("MachineState");
            }
        }
        private string _OperatorPrompt = "OperatorPrompt";
        public string OperatorPrompt
        {
            get { return _OperatorPrompt; }
            set
            {
                _OperatorPrompt = value;
                NotifyPropertyChanged("OperatorPrompt");
            }
        }
        #endregion

        #region Button
        private bool _OkToRun = false;
        public bool OkToRun
        {
            get { return _OkToRun; }
            set
            {
                _OkToRun = value;
                NotifyPropertyChanged("OkToRun");
            }
        }

        private bool _OkToRelease = false;
        public bool OkToRelease
        {
            get { return _OkToRelease; }
            set
            {
                _OkToRelease = value;
                NotifyPropertyChanged("OkToRelease");
            }
        }

        private bool _OkToAck = false;
        public bool OkToAck
        {
            get { return _OkToAck; }
            set
            {
                _OkToAck = value;
                NotifyPropertyChanged("OkToAck");
            }
        }

        private ICommand _RunTrigger;
        public ICommand RunTrigger
        {
            get
            {
                if (null == _RunTrigger)
                {
                    _RunTrigger = new RelayCommand(
                        param => runActive(),
                        param => true);
                }
                return _RunTrigger;
            }
        }
        private void runActive()
        {
            exitIdle = true;
        }

        private ICommand _ReleaseCommand;
        public ICommand ReleaseCommand
        {
            get
            {
                if (null == _ReleaseCommand)
                {
                    _ReleaseCommand = new RelayCommand(
                        param => testActive(),
                        param => true);
                }
                return _ReleaseCommand;
            }
        }
        private void testActive()
        {
            //exitIdle = true;
        }

        private ICommand _AckFaultStateMachine;
        public ICommand AckFaultStateMachine
        {
            get
            {
                if (_AckFaultStateMachine == null)
                {
                    _AckFaultStateMachine = new RelayCommand(
                        param => LeaveErrorOrAbortState(),
                        param => true);
                }
                return _AckFaultStateMachine;
            }
        }
        private void LeaveErrorOrAbortState()
        {
           // ackFault = true;
        }
        #endregion
              

        private bool _AudioActive = false;
        public bool AudioActive
        {
            get { return _AudioActive; }
            set
            {
                _AudioActive = value;
                NotifyPropertyChanged("AudioActive");
            }
        }
        private bool _SingleMode = false;
        public bool SingleMode
        {
            get { return _SingleMode; }
            set
            {
                _SingleMode = value;
                NotifyPropertyChanged("SingleMode");
            }
        }

        private string _MinimumIR_A0 = "-1";
        public string MinimumIR_A0
        {
            get { return _MinimumIR_A0; }
            set
            {
                _MinimumIR_A0 = value;
                NotifyPropertyChanged("MinimumIR_A0");
            }
        }
        private string _MaximumIR_A0 = "-1";
        public string MaximumIR_A0
        {
            get { return _MaximumIR_A0; }
            set
            {
                _MaximumIR_A0 = value;
                NotifyPropertyChanged("MaximumIR_A0");
            }
        }
        private string _MinimumIR_A1 = "-1";
        public string MinimumIR_A1
        {
            get { return _MinimumIR_A1; }
            set
            {
                _MinimumIR_A1 = value;
                NotifyPropertyChanged("MinimumIR_A1");
            }
        }
        private string _MaximumIR_A1 = "";
        public string MaximumIR_A1
        {
            get { return _MaximumIR_A1; }
            set
            {
                _MaximumIR_A1 = value;
                NotifyPropertyChanged("MaximumIR_A1");
            }
        }

        private int _RawDataID_A0 = -1;
        public int RawDataID_A0
        {
            get { return _RawDataID_A0; }
            set
            {
                _RawDataID_A0 = value;
                NotifyPropertyChanged("RawDataID_A0");
            }
        }
        private int _RawDataID_A1 = -1;
        public int RawDataID_A1
        {
            get { return _RawDataID_A1; }
            set
            {
                _RawDataID_A1 = value;
                NotifyPropertyChanged("RawDataID_A1");
            }
        }

        private string _DetectState = "系統待命";
        public string DetectState
        {
            get { return _DetectState; }
            set
            {
                _DetectState = value;
                NotifyPropertyChanged("DetectState");
            }
        }

    }
}
