using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GetupMonitor.ViewModel
{
    internal partial class GetupMonitor_VM
    {

        GetupMonitorStates StateControl = GetupMonitorStates.StateIdle;

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
            try
            {
                while (stateMachineTrigger)
                {
                    lastState = bufferState;
                    if (lastState != StateControl)
                        FirstTime = true;

                    if (stopCMD)
                    {
                        StateControl = GetupMonitorStates.StateSTOP;
                        FirstTime = true;
                        stopCMD = false;
                    }

                    Thread.Sleep(100);
                    switch (StateControl)
                    {
                        case GetupMonitorStates.StateIdle:
                            StateControl = StateIdle();
                            break;
                        case GetupMonitorStates.StateCheckConnection:
                            StateControl = StateCheckConnection(); break;
                        case GetupMonitorStates.StateMonitor:
                            StateControl = StateMonitor(); break;
                        case GetupMonitorStates.StateNotify:
                            StateControl = StateNotify(); break;
                        case GetupMonitorStates.StateSTOP:
                            StateControl = StateSTOP(); break;
                        case GetupMonitorStates.StateError:
                            StateControl = StateError(); break;
                        case GetupMonitorStates.StateTimeout:
                            StateControl = StateTimeout(); break;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        private GetupMonitorStates StateIdle()
        {
            if (FirstTime)
            {
                state_Initial(GetupMonitorStates.StateIdle, "系統待命");
                MainDisplayColor = Brushes.DarkGray;
                IsIdle = true;
                readToGo = false;
                buttonState(false, false, false, false);
            }

            if (exitIdle)
            {
                IsIdle = false;
                buttonState(false, false, false, false);
                return (GetupMonitorStates.StateCheckConnection);
            }

            uint buff = 0;            
            bool ck_bt = BTclient.Connected;
            bool ch_st = uint.TryParse(StableRange, out buff);
            bool ch_IT = uint.TryParse(SampleInterval, out buff);
            bool ck_MinA0 = uint.TryParse(MinimumIR_A0, out buff);
            bool ck_MaxA0 = uint.TryParse(MaximumIR_A0, out buff);
            bool ck_MinA1 = uint.TryParse(MinimumIR_A1, out buff);
            bool ck_MaxA1 = uint.TryParse(MaximumIR_A1, out buff);

            if (!ck_bt)
                OperatorPrompt = "藍芽未連線";
            else if (!ch_st)
                OperatorPrompt = "穩定值須為正整數";
            else if (!ch_IT)
                OperatorPrompt = "取樣間隔須為正整數";
            else if (!ck_MinA0)
                OperatorPrompt = "A0偵測下限須為正整數";
            else if (!ck_MaxA0)
                OperatorPrompt = "A0偵測上限須為正整數";
            else if (!ck_MinA1)
                OperatorPrompt = "A1偵測下限須為正整數";
            else if (!ck_MaxA1)
                OperatorPrompt = "A1偵測上限須為正整數";
            
            if (ck_bt && ch_st  && ch_IT && ck_MaxA0 && ck_MaxA1 && ck_MinA0 && ck_MinA1 && checkInteger())
            {
               
                OperatorPrompt = "點擊Run開始偵測";

                buttonState(true, false, false, false);
            }
            else
                buttonState(false, false, false, false);

            return (GetupMonitorStates.StateIdle);
        }

        private GetupMonitorStates StateCheckConnection()
        {
            if (FirstTime)
            {
                state_Initial(GetupMonitorStates.StateCheckConnection, "檢測藍芽連線");
                MainDisplayColor = Brushes.DarkGray;
                buttonState(false, false, false, true);
            }

            if (BTclient.Connected)
                return (GetupMonitorStates.StateMonitor);
            else
            {
                //TODO => Reconnect
                return (GetupMonitorStates.StateCheckConnection);
            }
        }

        bool passA0 = false, passA1 = false;
        int countingA0 = 0, countingA1 = 0, countingRun = 0;
        System.Drawing.Point stableA0, stableA1;
        const string onAir = "系統偵測中";
        private GetupMonitorStates StateMonitor()
        {
            try
            {
                if (FirstTime)
                {
                    state_Initial(GetupMonitorStates.StateMonitor, onAir);
                    MainDisplayColor = Brushes.DarkOliveGreen;
                    OperatorPrompt = onAir;
                }
                DetectState = runtimeDisplay(onAir);

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
                    stableA0.X = Convert.ToInt16(RawDataIR_A0 - CurrentDetect[DetectCriteria.StableRange]);
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
                Thread.Sleep(Convert.ToInt32( CurrentDetect[DetectCriteria.SampleInterval]));

                if (SingleMode && (passA0 || passA1))
                    return (GetupMonitorStates.StateNotify);
                else if (passA0 && passA1)
                    return (GetupMonitorStates.StateNotify);
                else
                    return (GetupMonitorStates.StateMonitor);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return GetupMonitorStates.StateError;
            }
        }
       
        private GetupMonitorStates StateNotify()
        {
            if (FirstTime)
            {
                state_Initial(GetupMonitorStates.StateNotify, "系統偵測到異常活動");
                buttonState(false, true, false, false);
                MainDisplayColor = Brushes.DarkRed;
                OperatorPrompt = "點擊Release重新偵測";
                detect_Initial();

                if (AudioActive)
                {
                    sPlayer = new SoundPlayer();
                    sPlayer.SoundLocation = Environment.CurrentDirectory + $"\\{WarningAudio}";
                    sPlayer.LoadAsync();
                    sPlayer.Play();
                }
            }

            if (MainDisplayColor == Brushes.DarkRed)
                MainDisplayColor = Brushes.DarkGray;
            else
                MainDisplayColor = Brushes.Red;

            if (clickRelease)
            {
                sPlayer.Stop();
                sPlayer.Dispose();
                clickRelease = false;
                return (GetupMonitorStates.StateCheckConnection);
            }
            else
                return (GetupMonitorStates.StateNotify);
        }

        private GetupMonitorStates StateSTOP()
        {
            if (FirstTime)
            {
                state_Initial(GetupMonitorStates.StateSTOP, "停止取樣");
                OperatorPrompt = "等待系統待命";
                exitIdle = false;
                MainDisplayColor = Brushes.DarkGray;
            }

            return GetupMonitorStates.StateIdle;
        }

        private GetupMonitorStates StateError()
        {
            return GetupMonitorStates.StateIdle;
        }
        private GetupMonitorStates StateTimeout()
        {
            return GetupMonitorStates.StateIdle;
        }
    }
}
