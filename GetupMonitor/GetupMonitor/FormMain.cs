using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GetupMonitor
{
    public partial class FormMain : Form
    {
        BluetoothClient BTclient = new BluetoothClient();
        BluetoothRadio BTradio = BluetoothRadio.Default;  

        const string MAJOR_BLUETOOTH = "MAJOR III BLUETOOTH";
        const string HC06 = "HC-06";
        public FormMain()
        {
            InitializeComponent();
            bluetoothTest();

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
                    byte[] tryToSend = { 0x01, 0x11 };
                    byte[] receiveAry = new byte[1024];

                    string message = "Hello, Bluetooth!";
                    byte[] messageBuffer = Encoding.ASCII.GetBytes(message);
                    nwStream.Write(messageBuffer, 0, messageBuffer.Length);

                    //nwStream.Write(tryToSend, 0, tryToSend.Length);
                    //Thread.Sleep(400);
                    //nwStream.Read(receiveAry, 0, tryToSend.Length);
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
