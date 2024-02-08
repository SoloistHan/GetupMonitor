using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace GetupMonitor.ViewModel
{
    public enum LedColor { Red = -1, None = 0, Green = 1 }
    internal partial class GetupMonitor_VM
    {
        private void pageMain_Initial()
        {
            AudioActive = Convert.ToBoolean( configDic[Audio_Active]);
            SingleMode = Convert.ToBoolean(configDic[Single_Mode]);
            StableRange = configDic[StableValue];
            SampleInterval = configDic[Sample_Interval];
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
                if (_IsIdle != value)
                {
                    _IsIdle = value;
                    NotifyPropertyChanged("IsIdle");
                }             
            }
        }

        private string _WindowTitle = "-1";
        public string WindowTitle
        {
            get { return _WindowTitle; }
            set
            {
                if (_WindowTitle != value)
                {
                    _WindowTitle = value;
                    NotifyPropertyChanged("WindowTitle");
                }
            }
        }

        #region Bluetooth
        private string _BlueToothState = "BlueToothState";
        public string BlueToothState
        {
            get { return _BlueToothState; }
            set 
            {
                if (_BlueToothState != value)
                {
                _BlueToothState = value;
                NotifyPropertyChanged("BlueToothState");
 }
            }
        }
        private LedColor _TitleLED = LedColor.Red;
        public LedColor TitleLED
        {
            get { return _TitleLED; }
            set
            {
                if (_TitleLED != value) {
                _TitleLED = value;
                NotifyPropertyChanged("TitleLED");
 }
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
                if (_DateDisplay != value)
                {
                    _DateDisplay = value;
                    NotifyPropertyChanged("DateDisplay");
                }
            }
        }

        private string _MachineState = "MachineState";
        public string MachineState
        {
            get { return _MachineState; }
            set
            {
                if (_MachineState != value)
                {
                _MachineState = value;
                NotifyPropertyChanged("MachineState");
 }
            }
        }
        private string _OperatorPrompt = "OperatorPrompt";
        public string OperatorPrompt
        {
            get { return _OperatorPrompt; }
            set
            {
                if (_OperatorPrompt != value)
                { 
                _OperatorPrompt = value;
                NotifyPropertyChanged("OperatorPrompt");
}
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
                if (_OkToRun != value)
                { 
                _OkToRun = value;
                NotifyPropertyChanged("OkToRun");
}
            }
        }
        private bool _OkToSTOP = false;
        public bool OkToSTOP
        {
            get { return _OkToSTOP; }
            set
            {
                if (_OkToSTOP != value)
                {
                _OkToSTOP = value;
                NotifyPropertyChanged("OkToSTOP");
 }
            }
        }

        private bool _OkToRelease = false;
        public bool OkToRelease
        {
            get { return _OkToRelease; }
            set
            {
                if (_OkToRelease != value)
                { 
                _OkToRelease = value;
                NotifyPropertyChanged("OkToRelease");
}
            }
        }

        private bool _OkToAck = false;
        public bool OkToAck
        {
            get { return _OkToAck; }
            set
            {
                if (_OkToAck != value)
                {
                _OkToAck = value;
                NotifyPropertyChanged("OkToAck");
                }
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
            if (collectCriteria())
                exitIdle = true;
            else
                exitIdle = false;
        }

        private ICommand _JustSTOP;
        public ICommand JustSTOP
        {
            get
            {
                if (null == _JustSTOP)
                {
                    _JustSTOP = new RelayCommand(
                        param => stopActive(),
                        param => true);
                }
                return _JustSTOP;
            }
        }
        private void stopActive()
        {
            stopCMD = true;
        }

        private ICommand _ReleaseCommand;
        public ICommand ReleaseCommand
        {
            get
            {
                if (null == _ReleaseCommand)
                {
                    _ReleaseCommand = new RelayCommand(
                        param => releaseCheck(),
                        param => true);
                }
                return _ReleaseCommand;
            }
        }
        private void releaseCheck()
        {
            clickRelease = true;
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
                if (_AudioActive != value)
                { 
                _AudioActive = value;
                NotifyPropertyChanged("AudioActive");
}
            }
        }
        private bool _SingleMode = false;
        public bool SingleMode
        {
            get { return _SingleMode; }
            set
            {
                if (_SingleMode != value)
                {

               
                _SingleMode = value;
                NotifyPropertyChanged("SingleMode");
 }
            }
        }

        private string _StableRange = "-1";
        public string StableRange
        {
            get { return _StableRange; }
            set
            {
                if (_StableRange != value)
                { 
                _StableRange = value;
                NotifyPropertyChanged("StableRange");
                }
            }
        }

        private string _SampleInterval = "-1";
        public string SampleInterval
        {
            get { return _SampleInterval; }
            set
            {
                if (_SampleInterval != value)
                {
                _SampleInterval = value;
                NotifyPropertyChanged("SampleInterval");
                }
            }
        }

        private string _MinimumIR_A0 = "-1";
        public string MinimumIR_A0
        {
            get { return _MinimumIR_A0; }
            set
            {
                if (_MinimumIR_A0 != value)
                { 
                _MinimumIR_A0 = value;
                NotifyPropertyChanged("MinimumIR_A0");
                }
            }
        }
        private string _MaximumIR_A0 = "-1";
        public string MaximumIR_A0
        {
            get { return _MaximumIR_A0; }
            set
            {
                if (_MaximumIR_A0 != value)
                {

                _MaximumIR_A0 = value;
                NotifyPropertyChanged("MaximumIR_A0");
                }

            }
        }
        private string _MinimumIR_A1 = "-1";
        public string MinimumIR_A1
        {
            get { return _MinimumIR_A1; }
            set
            {
                if (_MinimumIR_A1 != value)
                {

                _MinimumIR_A1 = value;
                NotifyPropertyChanged("MinimumIR_A1");
                }
            }
        }
        private string _MaximumIR_A1 = "";
        public string MaximumIR_A1
        {
            get { return _MaximumIR_A1; }
            set
            {
                if (_MaximumIR_A1 != value)
                {

                _MaximumIR_A1 = value;
                NotifyPropertyChanged("MaximumIR_A1");
                }
            }
        }

        private int _RawDataIR_A0 = -1;
        public int RawDataIR_A0
        {
            get { return _RawDataIR_A0; }
            set
            {
                if (_RawDataIR_A0 != value)
                {

                _RawDataIR_A0 = value;
                NotifyPropertyChanged("RawDataIR_A0");
                }
            }
        }

        private string _CountingA0 = "0/5";
        public string CountingA0
        {
            get { return _CountingA0; }
            set
            {
                if (_CountingA0 != value)
                {

                _CountingA0 = value;
                NotifyPropertyChanged("CountingA0");
                }
            }
        }

        private int _RawDataIR_A1 = -1;
        public int RawDataIR_A1
        {
            get { return _RawDataIR_A1; }
            set
            {
                if (_RawDataIR_A1 != value)
                {

                _RawDataIR_A1 = value;
                NotifyPropertyChanged("RawDataIR_A1");
                }
            }
        }

        private string _CountingA1 = "0/5";
        public string CountingA1
        {
            get { return _CountingA1; }
            set
            {
                if (_CountingA1 != value)
                {

                _CountingA1 = value;
                NotifyPropertyChanged("CountingA1");
                }
            }
        }

        private string _DetectState = "系統待命";
        public string DetectState
        {
            get { return _DetectState; }
            set
            {
                if (_DetectState != value)
                {

                _DetectState = value;
                NotifyPropertyChanged("DetectState");
                }
            }
        }

        private Brush _MainDisplayColor = Brushes.DarkGray;
        public Brush MainDisplayColor
        {
            get { return _MainDisplayColor; }
            set
            {
                if (_MainDisplayColor != value)
                {

                _MainDisplayColor = value;
                NotifyPropertyChanged("MainDisplayColor");
                }
            }
        }

    }
}
