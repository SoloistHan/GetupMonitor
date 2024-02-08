using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GetupMonitor.ViewModel
{
    internal partial class GetupMonitor_VM
    {
        #region Class Properties
        #endregion

        enum DetectCriteria {StableRange, SampleInterval, MinA0, MaxA0, CountingA0, MinA1, MaxA1, CountingA1}
        Dictionary<DetectCriteria, uint> CurrentDetect;

        BluetoothClient BTclient = new BluetoothClient();
        BluetoothRadio BTradio = BluetoothRadio.Default;

        SoundPlayer sPlayer;

        const string MAJOR_BLUETOOTH = "MAJOR III BLUETOOTH";
        const string HC06 = "HC-06";
         const string WarningAudio = "WarningAudio.wav";

        byte[] CmdGetIR_A0 = { 0xFA, 0xA0, 0xFF };
        byte[] CmdGetIR_A1 = { 0xFA, 0xA1, 0xFF };

        GetupMonitorStates bufferState = GetupMonitorStates.None, lastState = GetupMonitorStates.None;

        bool stateMachineTrigger = false, systemMonitorTrigger = false, getDataTrigger = false ;
        bool FirstTime = true, exitIdle = false, stopCMD = false, readToGo = false, clickRelease = false;

        int runningDot = 0;

        private void state_Initial(GetupMonitorStates CurrentState, string MainDisplay)
        {
            FirstTime = false;
            bufferState = CurrentState;
            MachineState = CurrentState.ToString();
            DetectState = MainDisplay;
        }

        private void buttonState(bool Run, bool Release, bool Ack, bool STOP)
        {
            OkToRun = Run;
            OkToRelease = Release;
            OkToAck = Ack;
            OkToSTOP = STOP;
        }

        private bool checkInteger()
        {
            uint minA0 = Convert.ToUInt16(_MinimumIR_A0);
            uint maxA0 = Convert.ToUInt16(_MaximumIR_A0);
            uint minA1 = Convert.ToUInt16(_MinimumIR_A1);
            uint maxA1 = Convert.ToUInt16(_MaximumIR_A1);

            bool limMinA0 = minA0 < 200;
            bool limMaxA0 = maxA0 < 200;
            bool limMinA1 = minA1 < 200;
            bool limMaxA1 = maxA1 < 200;
            bool rangeA0 = minA0 < maxA0;
            bool rangeA1 = minA1 < maxA1;

            if (!limMaxA0)
                OperatorPrompt = "A0偵測下限須小於200";
            else if (!limMaxA0)
                OperatorPrompt = "A0偵測上限須小於200";
            else if (!limMinA1)
                OperatorPrompt = "A1偵測下限須小於200";
            else if (!limMaxA1)
                OperatorPrompt = "A1偵測上限須小於200";
            else if (!rangeA0)
                OperatorPrompt = "A0偵測下限不可大於偵測上限";
            else if (!rangeA1)
                OperatorPrompt = "A1偵測下限不可大於偵測上限";

            if (limMaxA0 && limMaxA1 && limMinA0 && limMinA1 && rangeA0 && rangeA1)
                return true;
            else
                return false;
        }

        private bool collectCriteria()
        {
            CurrentDetect = new Dictionary<DetectCriteria, uint>();
            try
            {
                CurrentDetect.Add(DetectCriteria.StableRange, Convert.ToUInt16(_StableRange));
                CurrentDetect.Add(DetectCriteria.SampleInterval, Convert.ToUInt16(_SampleInterval));
                CurrentDetect.Add(DetectCriteria.MinA0, Convert.ToUInt16(_MinimumIR_A0));
                CurrentDetect.Add(DetectCriteria.MaxA0, Convert.ToUInt16(_MaximumIR_A0));
                CurrentDetect.Add(DetectCriteria.MinA1, Convert.ToUInt16(_MinimumIR_A1));
                CurrentDetect.Add(DetectCriteria.MaxA1, Convert.ToUInt16(_MaximumIR_A1));

                return true;
            }
            catch(Exception ex)
            {
                OperatorPrompt = ex.Message;
            }
            return false;
        }

        private void detect_Initial()
        {
            passA0 = false;
            passA1 = false;
            countingA0 = 0;
            countingA1 = 0;
            stableA0 = new System.Drawing.Point(0, 0);
            stableA1 = new System.Drawing.Point(0, 0);
        }

        private void getDataIR()
        {
            try
            {
                if (BTclient.Connected)
                {
                    NetworkStream nwStream = BTclient.GetStream();
                    nwStream.ReadTimeout = 1000;

                    byte[] receiveAryA0 = new byte[1024];
                    nwStream.Write(CmdGetIR_A0, 0, CmdGetIR_A0.Length);
                    Thread.Sleep(100);
                    nwStream.Read(receiveAryA0, 0, 1);
                    RawDataIR_A0 = receiveAryA0[0];

                    byte[] receiveAryA1 = new byte[1024];
                    nwStream.Write(CmdGetIR_A1, 0, CmdGetIR_A1.Length);
                    Thread.Sleep(100);
                    nwStream.Read(receiveAryA1, 0, 1);
                    RawDataIR_A1 = receiveAryA1[0];

                    string stop = "";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string runtimeDisplay(string Prefix)
        {
            string outcome = "";
            string dot = "";
            for (int run = 0; run < runningDot; run++)
                dot += ".";
            outcome = $"{Prefix}{dot}";
            if (runningDot > 3)
                runningDot = 0;
            else
                runningDot++;

            return outcome;
        }

        #region Bluetooth Async Connect
        private async void async_bluetoothMonitor()
        {
            systemMonitorTrigger = true;
            await task__bluetoothMonitor();
        }
        private Task<bool> task__bluetoothMonitor()
        {
            return Task.Factory.StartNew(() => bluetoothMonitor());
        }
        private bool bluetoothMonitor()
        {
            while (systemMonitorTrigger)
            {
                if (!BTclient.Connected)
                {
                    
                    BTradio.Mode = RadioMode.Connectable;
                    IReadOnlyCollection<BluetoothDeviceInfo> devicesList = BTclient.DiscoverDevices();

                    foreach (BluetoothDeviceInfo device in devicesList)
                    {
                        if (device != null)
                        {
                            if (device.DeviceName == HC06)
                            {
                                BTclient.Encrypt = false;
                                BTclient.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                                string getDevice = BTclient.RemoteMachineName;

                            }
                        }
                    }
                }
                else
                {
                    systemMonitorTrigger = false;
                }

                Thread.Sleep(100);
            }
            return true;
        }

        #endregion

        private void bluetoothTest()
        {
            BTradio.Mode = RadioMode.Connectable;
            IReadOnlyCollection<BluetoothDeviceInfo> devicesList = BTclient.DiscoverDevices();

            foreach (BluetoothDeviceInfo device in devicesList)
            {
                //device.Refresh();
                if (device != null)
                {
                    if (device.DeviceName == MAJOR_BLUETOOTH)
                    {
                        BTclient.Connect(device.DeviceAddress, BluetoothService.Headset);
                        string getDevice = BTclient.RemoteMachineName;
                    }
                    else if (device.DeviceName == HC06)
                    {
                        BTclient.Encrypt = false;
                        BTclient.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                        string getDevice = BTclient.RemoteMachineName;

                    }
                }
            }

            if (BTclient.Connected)
            {
                try
                {
                    NetworkStream nwStream = BTclient.GetStream();
                    nwStream.ReadTimeout = 1000;
                    byte[] receiveAry = new byte[1024];

                    string message = "Hello";//, Bluetooth!";
                    byte[] messageBuffer = Encoding.ASCII.GetBytes(message);
                    //nwStream.Write(messageBuffer, 0, messageBuffer.Length);
                    nwStream.Write(CmdGetIR_A1, 0, CmdGetIR_A1.Length);
                    Thread.Sleep(100);
                    nwStream.Read(receiveAry, 0, 1);


                    //Thread.Sleep(400);
                    //nwStream.Read(receiveAry, 0, tryToSend.Length);
                    string stop = "";
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                }
                BTclient.Dispose();
            }

        }

    }
}
