using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;

namespace KbStats.Core
{
    using Utils;

    /// <summary>
    /// Just hosts a queue where the the native code sends the keypress code
    /// </summary>
    public static class CommunicationLayer
    {
        internal static BlockingCollection<VirtualKey> KeyPressedQueue { get; set; }
        private static Task _consumerThread = null;
        private static CancellationTokenSource _cancToken;

        private static bool _shiftHeld = false;

        static CommunicationLayer()
        {
            KeyPressedQueue = new BlockingCollection<VirtualKey>(new ConcurrentQueue<VirtualKey>());
            _cancToken = new CancellationTokenSource();
        }

        public static void ExitApplication()
        {
            _cancToken.Cancel();
            _consumerThread.Wait();
            KeyPressedQueue.Dispose();
        }

        /// <summary>
        /// Needs to be called by the native code to queue a keypress
        /// </summary>
        public static void AddKeyPressed(VirtualKey key)
        {
            if (_consumerThread == null)
            {
                _consumerThread = Task.Factory.StartNew(ConsumerThread.MainLoop, _cancToken.Token, TaskCreationOptions.LongRunning);
            }

            //try
            //{
            //var internalKey = ConvertNativeKeyCode(keyPressed);
            Logger.PrintLog(key.ToString());
            KeyPressedQueue.Add(key);
            //}
            //catch (Exception e)
            //{
            //    Logger.PrintError($"Exception {e} was thrown for key pressed {keyPressed}. Skipping...");
            //}
        }

        /// <summary>
        /// Converts from OS specific key to internal keycode
        /// </summary>
        /// <exception cref="System.Exception">Thrown when a character isn't recognized</exception>
        /// <param name="nativeKeyCode">KeyCode used in the native OS</param>
        /// <returns>Internal <see cref="KeyCode"/></returns>
        private static KeyCode ConvertNativeKeyCode(int nativeKeyCode)
        {
            // windows keycode conversion
            if (nativeKeyCode == 65) return KeyCode.A;
            else if (nativeKeyCode == 66) return KeyCode.B;
            else if (nativeKeyCode == 67) return KeyCode.C;
            else if (nativeKeyCode == 68) return KeyCode.D;
            else if (nativeKeyCode == 69) return KeyCode.E;
            else if (nativeKeyCode == 70) return KeyCode.F;
            else if (nativeKeyCode == 71) return KeyCode.G;
            else if (nativeKeyCode == 72) return KeyCode.H;
            else if (nativeKeyCode == 73) return KeyCode.I;
            else if (nativeKeyCode == 74) return KeyCode.J;
            else if (nativeKeyCode == 75) return KeyCode.K;
            else if (nativeKeyCode == 76) return KeyCode.L;
            else if (nativeKeyCode == 77) return KeyCode.M;
            else if (nativeKeyCode == 78) return KeyCode.N;
            else if (nativeKeyCode == 79) return KeyCode.O;
            else if (nativeKeyCode == 80) return KeyCode.P;
            else if (nativeKeyCode == 81) return KeyCode.Q;
            else if (nativeKeyCode == 82) return KeyCode.R;
            else if (nativeKeyCode == 83) return KeyCode.S;
            else if (nativeKeyCode == 84) return KeyCode.T;
            else if (nativeKeyCode == 85) return KeyCode.U;
            else if (nativeKeyCode == 86) return KeyCode.V;
            else if (nativeKeyCode == 87) return KeyCode.W;
            else if (nativeKeyCode == 88) return KeyCode.X;
            else if (nativeKeyCode == 89) return KeyCode.Y;
            else if (nativeKeyCode == 90) return KeyCode.Z;

            else if (nativeKeyCode == 32) return KeyCode.Space;

            else if (nativeKeyCode == 48) return KeyCode.Number0;
            else if (nativeKeyCode == 49) return KeyCode.Number1;
            else if (nativeKeyCode == 50) return KeyCode.Number2;
            else if (nativeKeyCode == 51) return KeyCode.Number3;
            else if (nativeKeyCode == 52) return KeyCode.Number4;
            else if (nativeKeyCode == 53) return KeyCode.Number5;
            else if (nativeKeyCode == 54) return KeyCode.Number6;
            else if (nativeKeyCode == 55) return KeyCode.Number7;
            else if (nativeKeyCode == 56) return KeyCode.Number8;
            else if (nativeKeyCode == 57) return KeyCode.Number9;

            else if (nativeKeyCode == 96) return KeyCode.NumberPad0;
            else if (nativeKeyCode == 97) return KeyCode.NumberPad1;
            else if (nativeKeyCode == 98) return KeyCode.NumberPad2;
            else if (nativeKeyCode == 99) return KeyCode.NumberPad3;
            else if (nativeKeyCode == 100) return KeyCode.NumberPad4;
            else if (nativeKeyCode == 101) return KeyCode.NumberPad5;
            else if (nativeKeyCode == 102) return KeyCode.NumberPad6;
            else if (nativeKeyCode == 103) return KeyCode.NumberPad7;
            else if (nativeKeyCode == 104) return KeyCode.NumberPad8;
            else if (nativeKeyCode == 105) return KeyCode.NumberPad9;

            else if (nativeKeyCode == 186)
            {
                return !_shiftHeld ? KeyCode.Semicolumn : KeyCode.Column;
            }
            else if (nativeKeyCode == 188) return KeyCode.Comma;
            else if (nativeKeyCode == 190) return KeyCode.FullStop;

            else if (nativeKeyCode == 16)
            {
                _shiftHeld = !_shiftHeld;
                Logger.PrintLog($"_shiftHeld = {_shiftHeld}");
                return KeyCode.Shift;
            }
            else if (nativeKeyCode == 160)
            {
                _shiftHeld = !_shiftHeld;
                return KeyCode.LeftShift;
            }
            else if (nativeKeyCode == 161)
            {
                _shiftHeld = !_shiftHeld;
                return KeyCode.RightShift;
            }

            else throw new Exception($"the character with code {nativeKeyCode} isn't recognized");
        }
    }

    internal enum KeyCode
    {
        A = 0,
        B,
        C,
        D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        Space,
        Number0, Number1, Number2, Number3, Number4, Number5, Number6, Number7, Number8, Number9,
        NumberPad0, NumberPad1, NumberPad2, NumberPad3, NumberPad4, NumberPad5, NumberPad6, NumberPad7, NumberPad8, NumberPad9,
        Comma, FullStop, Column, Semicolumn,
        Shift, LeftShift, RightShift,
    }
}
