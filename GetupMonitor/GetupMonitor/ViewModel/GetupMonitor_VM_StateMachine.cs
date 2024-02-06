using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
                MainDisplayColor = Brushes.DarkGray;
                IsIdle = true;
                readToGo = false;
                buttonState(false, false, false);
            }

            if (exitIdle)
            {
                IsIdle = false;
                buttonState(false, false, false);
                StateBranch(GetupMonitorStates.StateCheckConnection);
            }

            uint buff = 0;            
            bool ck_bt = BTclient.Connected;
            bool ch_st = uint.TryParse(StableRange, out buff);
            bool ck_MinA0 = uint.TryParse(MinimumIR_A0, out buff);
            bool ck_MaxA0 = uint.TryParse(MaximumIR_A0, out buff);
            bool ck_MinA1 = uint.TryParse(MinimumIR_A1, out buff);
            bool ck_MaxA1 = uint.TryParse(MaximumIR_A1, out buff);

            if (!ck_bt)
                OperatorPrompt = "藍芽未連線";
            else if (!ch_st)
                OperatorPrompt = "穩定值須為正整數";
            else if (!ck_MinA0)
                OperatorPrompt = "A0偵測下限須為正整數";
            else if (!ck_MaxA0)
                OperatorPrompt = "A0偵測上限須為正整數";
            else if (!ck_MinA1)
                OperatorPrompt = "A1偵測下限須為正整數";
            else if (!ck_MaxA1)
                OperatorPrompt = "A1偵測上限須為正整數";
            
            if (ck_bt && ch_st  && ck_MaxA0 && ck_MaxA1 && ck_MinA0 && ck_MinA1 && checkInteger())
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
                MainDisplayColor = Brushes.DarkGray;
                buttonState(false, false, false);
            }

            if (BTclient.Connected) 
                StateBranch(GetupMonitorStates.StateMonitor);
            else
            {
                // Reconnect
                StateBranch(GetupMonitorStates.StateCheckConnection);
            }
        }

        bool passA0 = false, passA1 = false;
        int countingA0 = 0, countingA1 = 0, countingRun = 0;
        Point stableA0, stableA1;
        const string onAir = "系統偵測中";
        private void StateMonitor()
        {
            if (FirstTime)
            {
                state_Initial(GetupMonitorStates.StateMonitor, onAir);
                MainDisplayColor = Brushes.DarkOliveGreen;
            }
            runtimeDisplay(onAir);

            if (countingA0 >= 1)
            {
                if (RawDataIR_A0 >= stableA0.X && RawDataIR_A0 <= stableA0.Y)
                {
                    countingA0++;
                    if (countingA0 >= 5)
                        passA0 = true;
                }
            }
            if (countingA1 >= 1)
            {
                if (RawDataIR_A1 >= stableA1.X && RawDataIR_A1 <= stableA1.Y)
                {
                    countingA1++;
                    if (countingA1 >= 5)
                        passA1 = true;
                }
            }

            if (countingA0 == 0 && RawDataIR_A0 >= CurrentDetect[DetectCriteria.MinA0] && RawDataIR_A0 <= CurrentDetect[DetectCriteria.MaxA0])
            {
                stableA0.X = Convert.ToInt16( RawDataIR_A0 - CurrentDetect[DetectCriteria.StableRange]);
                stableA0.Y = Convert.ToInt16(RawDataIR_A0 + CurrentDetect[DetectCriteria.StableRange]);
                countingA0 = 1;
            }
            if (countingA1 == 0 && RawDataIR_A1 >= CurrentDetect[DetectCriteria.MinA1] && RawDataIR_A1 <= CurrentDetect[DetectCriteria.MaxA1])
            {
                stableA1.X = Convert.ToInt16(RawDataIR_A1 - CurrentDetect[DetectCriteria.StableRange]);
                stableA1.Y = Convert.ToInt16(RawDataIR_A1 + CurrentDetect[DetectCriteria.StableRange]);
                countingA1 = 1;
            }

            CountingA0 = $"{countingA0}/5";
            CountingA1 = $"{countingA1}/5";

    if (countingRun > 5)
            {
                detect_Initial();
                countingRun = 0;
            }
            countingRun++;
            Thread.Sleep(500);

            if (SingleMode && (passA0 || passA1))
                StateBranch(GetupMonitorStates.StateNotify);
            else if (passA0 && passA1)
                StateBranch(GetupMonitorStates.StateNotify);
            else
                StateBranch(GetupMonitorStates.StateMonitor);

        
        }

        bool clickRelease = false;
        SoundPlayer player;
        private void StateNotify()
        {
            if (FirstTime)
            {
                state_Initial(GetupMonitorStates.StateNotify, "系統偵測到異常活動");
                buttonState(false, true, false);
                MainDisplayColor = Brushes.DarkRed;
                OperatorPrompt = "點擊Release重新偵測";
                detect_Initial();

                if (AudioActive)
                {
                    player = new SoundPlayer();
                    player.SoundLocation = Environment.CurrentDirectory + @"\WildRose.wav";
                    player.LoadAsync();
                    player.Play();
                }
            }

            if (MainDisplayColor == Brushes.DarkRed)
                MainDisplayColor = Brushes.DarkGray;
            else
                MainDisplayColor = Brushes.DarkRed;

            if (clickRelease)
            {
                player.Stop();
                player.Dispose();
                clickRelease = false;
                StateBranch(GetupMonitorStates.StateCheckConnection);
            }
            else
                StateBranch(GetupMonitorStates.StateNotify);
        }
        private void StateError()
        {

        }
        private void StateTimeout()
        {

        }
    }
}
