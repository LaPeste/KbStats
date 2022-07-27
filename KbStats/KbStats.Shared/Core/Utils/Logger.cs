using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KbStats.Core.Utils
{
    public static class Logger
    {
        public static void PrintLog(string message)
        {
            Debug.WriteLine($"Log  -  {message}");
        }

        public static void PrintDebug(string message)
        {
            Debug.WriteLine($"Debug  -  {message}");
        }

        public static void PrintWarning(string message)
        {
            Console.WriteLine($"Log  -  {message}");
        }

        public static void PrintError(string message)
        {
            Console.WriteLine($"Log  -  {message}");
        }
    }
}
