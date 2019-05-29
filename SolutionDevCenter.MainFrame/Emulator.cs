using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Open3270;

namespace SolutionDevCenter.MainFrame
{

    public class Emulator
    {
        internal TNEmulator Emulator3270 { get; private set; }
        private static ConfigurationEmulator _emulatorConfig;
        private static Emulator _emulator;
        private bool isConnected;

        private Emulator()
        {
            Emulator3270 = new TNEmulator();
        }

        public static Emulator GetInstance()
        {
            if (_emulator == null)
                _emulator = new Emulator();

            return _emulator;
        }

        public static ConfigurationEmulator GetConfiguration()
        {
            return _emulatorConfig;
        }

        public static void Configure(ConfigurationEmulator emulatorConfig)
        {
            _emulatorConfig = emulatorConfig;
        }

        public bool Connect()
        {
            try
            {
                if (!isConnected)
                    Emulator3270.Connect(_emulatorConfig.Host, _emulatorConfig.Port, string.Empty);

                isConnected = true;
            }
            catch (Exception ex)
            {
                isConnected = false;
            }

            return isConnected;
        }

        public void Disconnect()
        {
            try
            {
                Emulator3270?.Close();
                Emulator3270 = null;
                _emulator = null;
            }
            finally
            {
                isConnected = false;
            }
        }

        public void Dispose()
        {
            Disconnect();
        }

        public string GetScreen()
        {
            string currentScreen;
            const int retryCount = 3;
            var currentRetry = 0;

            for (; ; )
            {
                try
                {
                    // Calling external service.               
                    currentScreen = Emulator3270.CurrentScreenXML.Dump();
                    // Return or break.
                    break;
                }
                catch (Exception ex)
                {
                    // Trace.TraceError("Operation Exception");

                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    // Determine whether to retry the operation, as well as how
                    // long to wait, based on the retry strategy.
                    if (currentRetry > retryCount)
                    {
                        // If this isn't a transient error
                        // or we shouldn't retry, rethrow the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                // Consider calculating an exponential delay here and
                // using a strategy best suited for the operation and fault.
                Thread.Sleep(100);
            }
            return currentScreen;
        }

        public string GetText(Position position, int length)
        {
            return Emulator3270.GetText(position.Column, position.Row, length);
        }

        public void SendKey(TnKey tnKey)
        {
            var keySent = Emulator3270.SendKey(true, tnKey, _emulatorConfig.TimeOut);

            if (keySent)
            {
                if (!Emulator3270.WaitForHostSettle(200, _emulatorConfig.TimeOut))
                    RaiseException(new Exception("Timeout waitting for host to settle!"));
            }
            else
            {
                RaiseException(new Exception("Timeout sending Key '" + tnKey.ToString() + "'"));
            }
        }

        public void SetText(string text, int linha, int coluna)
        {
            if (!Emulator3270.SetText(text, coluna, linha))
                RaiseException(new Exception("Error setting text '" + text + "' to coords '" + linha + "' and y '" + coluna + "'"));
        }

        public void WaitForText(string text, Position position)
        {
            if (!Emulator3270.WaitForText(position.Row, position.Column, text, _emulatorConfig.TimeOut))
            {
                RaiseException(new Exception(String.Format("Timeout waitting for text '{0}' at {1}/{2}!", text, position.Row, position.Column)));
            }
        }

        public void WaitForScreenToChange()
        {
            if (!Emulator3270.WaitForHostSettle(200, _emulatorConfig.TimeOut))
                RaiseException(new Exception("Timeout waitting for host to settle!"));

            Emulator3270.WaitTillKeyboardUnlocked(_emulatorConfig.TimeOut);
        }

        public void RaiseException(Exception e)
        {
            throw e;
        }



    }
}
