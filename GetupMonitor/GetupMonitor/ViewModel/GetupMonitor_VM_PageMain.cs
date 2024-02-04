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
        private bool _IsRunning = false;
        public bool IsRunning
        {
            get { return _IsRunning; }
            set
            {
                _IsRunning = value;
                NotifyPropertyChanged("IsRunning");
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

        private bool _OkToTest = false;
        public bool OkToTest
        {
            get { return _OkToTest; }
            set
            {
                _OkToTest = value;
                NotifyPropertyChanged("OkToTest");
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
           // exitIdle = true;
        }

        private ICommand _TestCommand;
        public ICommand TestCommand
        {
            get
            {
                if (null == _TestCommand)
                {
                    _TestCommand = new RelayCommand(
                        param => testActive(),
                        param => true);
                }
                return _TestCommand;
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

        private int _MinimumIR = -1;
        public int MinimumIR
        {
            get { return _MinimumIR; }
            set
            {
                _MinimumIR = value;
                NotifyPropertyChanged("MinimumIR");
            }
        }
        private int _MaximumIR = -1;
        public int MaximumIR
        {
            get { return _MaximumIR; }
            set
            {
                _MaximumIR = value;
                NotifyPropertyChanged("MaximumIR");
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

    }
}
