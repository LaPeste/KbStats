using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Realms;

namespace KbStats.Core.Utils
{
    public static class Constants
    {
        public const string RealmPath = "kbStats.realm";
        public static readonly RealmConfiguration RealmConfiguration = new RealmConfiguration(RealmPath);
        public const int ThreadLooplength = Timeout.Infinite; // in millisecond
        public static readonly TimeSpan SamplingLength = new TimeSpan(0, 0, 5);
    }
}
