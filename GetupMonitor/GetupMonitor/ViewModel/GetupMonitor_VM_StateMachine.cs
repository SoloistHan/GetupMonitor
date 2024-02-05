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
            lastState = bufferState;
            if (lastState != NextState)
                FirstTime = true;

            Thread.Sleep( 100 );
            switch (NextState)
            {
                case GetupMonitorStates.StateIdle:
                    StateIdle(); break;
                case GetupMonitorStates.StateCheckConnection:
                    StateCheckConnection(); break;
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
            if (FirstTime)
            {
                state_Initial(GetupMonitorStates.StateIdle, "系統待命");
                IsIdle = true;
                readToGo = false;
                buttonState(false, false, false);
            }

            if (exitIdle)
            {
                buttonState(false, false, false);
                StateBranch(GetupMonitorStates.StateCheckConnection);
            }

            int buff = -1;            
            bool ck_bt = BTclient.Connected;
            bool ck_MinA0 = int.TryParse(MinimumIR_A0, out buff);
            bool ck_MaxA0 = int.TryParse(MaximumIR_A0, out buff);
            bool ck_MinA1 = int.TryParse(MinimumIR_A1, out buff);
            bool ck_MaxA1 = int.TryParse(MaximumIR_A1, out buff);

            if (!ck_bt)
                OperatorPrompt = "藍芽未連線";
            else if (!ck_MinA0)
                OperatorPrompt = "A0偵測下限須為數字";
            else if (!ck_MaxA0)
                OperatorPrompt = "A0偵測上限須為數字";
            else if (!ck_MinA1)
                OperatorPrompt = "A1偵測下限須為數字";
            else if (!ck_MaxA1)
                OperatorPrompt = "A1偵測上限須為數字";
            
            if (ck_bt && ck_MaxA0 && ck_MaxA1 && ck_MinA0 && ck_MinA1)
            {
                OperatorPrompt = "點擊Run開始偵測";
                buttonState(true, false, false);
            }
            else
                buttonState(false, false, false);

            StateBranch(GetupMonitorStates.StateIdle);
        }

        private void StateCheckConnection()
        {
            if (FirstTime)
            {
                state_Initial(GetupMonitorStates.StateCheckConnection, "檢測藍芽連線");
            }

            if (BTclient.Connected) 
                StateBranch(GetupMonitorStates.StateMonitor);
            else
            {
                // Reconnect
                StateBranch(GetupMonitorStates.StateCheckConnection);
            }
        }

        private void StateMonitor()
        {
            // get value keep between +- 5  maybe 5 times
           
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
