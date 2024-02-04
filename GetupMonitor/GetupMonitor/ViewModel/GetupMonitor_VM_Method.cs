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
        
        bool stateMachineTrigger = false;
        bool systemMonitorTrigger = false;

        int runningDot = 0;

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
