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
        public void ExecuteStateMachine()
        {
            async_GetupMonitor();
        }

        private async void async_GetupMonitor()
        {
            stateMachineTrigger = true;
            await task__GetupMonitor();
        }
        private Task<bool> task__GetupMonitor()
        {
            return Task.Factory.StartNew(() =>  GetupMonitor_StateMachine() );
        }
        private bool GetupMonitor_StateMachine()
        {
            while ( stateMachineTrigger ) 
            {
                StateBranch(GetupMonitorStates.StateIdle);
            }
            return true;
        }

        private void StateBranch(GetupMonitorStates NextState)
        {
            Thread.Sleep( 100 );
            switch (NextState)
            {
                case GetupMonitorStates.StateIdle:
                    StateIdle(); break;
                case GetupMonitorStates.StateStartUp:
                    StateStartUp(); break;
                case GetupMonitorStates.StateMonitor:
                    StateMonitor(); break;
                case GetupMonitorStates.StateNotify:
                    StateNotify(); break;
                case GetupMonitorStates.StateError:
                    StateError(); break;
                case GetupMonitorStates.StateTimeout:
                    StateTimeout(); break;
            }
           
        }
        private void StateIdle()
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

            StateBranch(GetupMonitorStates.StateIdle);
        }
        private void StateStartUp()
        {

        }
        private void StateMonitor()
        {

        }
        private void StateNotify()
        {

        }
        private void StateError()
        {

        }
        private void StateTimeout()
        {

        }
    }
}
