using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GetupMonitor.ViewModel
{
    internal partial class GetupMonitor_VM
    {
        #region Class Properties
        #endregion

        BluetoothClient BTclient = new BluetoothClient();
        BluetoothRadio BTradio = BluetoothRadio.Default;

        const string MAJOR_BLUETOOTH = "MAJOR III BLUETOOTH";
        const string HC06 = "HC-06";
        
        byte[] CmdGetIR_A0 = { 0xFA, 0xA0, 0xFF };
        byte[] CmdGetIR_A1 = { 0xFA, 0xA1, 0xFF };

        GetupMonitorStates bufferState = GetupMonitorStates.None, lastState = GetupMonitorStates.None;

        bool stateMachineTrigger = false, systemMonitorTrigger = false, getDataTrigger = false ;
        bool FirstTime = true, exitIdle = false, readToGo = false;

        int runningDot = 0;

        private void state_Initial(GetupMonitorStates CurrentState, string MainDisplay)
        {
            FirstTime = false;
            bufferState = CurrentState;
            MachineState = CurrentState.ToString();
            DetectState = MainDisplay;
        }

        private void buttonState(bool Run, bool Release, bool Ack)
        {
            OkToRun = Run;
            OkToRelease = Release;
            OkToAck = Ack;
        }

        //private string checkColumn()
        //{
        //    if ()
        //}

        private void getDataIR()
        {
            if (BTclient.Connected)
            {
                try
                {
                    NetworkStream nwStream = BTclient.GetStream();
                    nwStream.ReadTimeout = 1000;

                    byte[] receiveAryA0 = new byte[1024];
                    nwStream.Write(CmdGetIR_A0, 0, CmdGetIR_A0.Length);
                    Thread.Sleep(100);
                    nwStream.Read(receiveAryA0, 0, 1);
                    RawDataID_A0 = receiveAryA0[0];

                    byte[] receiveAryA1 = new byte[1024];
                    nwStream.Write(CmdGetIR_A1, 0, CmdGetIR_A1.Length);
                    Thread.Sleep(100);
                    nwStream.Read(receiveAryA1, 0, 1);
                    RawDataID_A1 = receiveAryA1[0];

                    string stop = "";
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                }
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
