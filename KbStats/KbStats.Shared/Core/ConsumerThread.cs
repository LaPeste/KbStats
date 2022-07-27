#define DEBUG_EXTRA
//#define DEBUG_BASIC

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Realms;
using Windows.System;
using System.Reflection;

namespace KbStats.Core
{
    using Models;
    using Utils;

    using Cps = Double;
    using Wpm = Double;

    internal static class ConsumerThread
    {
        private static DateTimeOffset _startedSampling = new DateTimeOffset();
        private static DateTimeOffset _lastKeyPressedTime = new DateTimeOffset();
        private static readonly TimeSpan _timeBeforeClosingInterval = new TimeSpan(0, 0, 3);
        private static readonly TimeSpan _samplingLengths = Constants.SamplingLength;
        private static bool _typing = false;

        private static List<Cps> _cps = new List<Cps>();
        private static List<Wpm> _wpm = new List<Wpm>();

        private static int _keyPressedCounter = 0;

        // created db can be found at C:\Users\andre\AppData\Local\Packages\45862e12-ca4c-4955-9674-772c808a0410_a9ne5m0wk73p0\LocalState
#if DEBUG_BASIC
        
        private static InMemoryConfiguration config = new InMemoryConfiguration("debug");
        private static Realm realm = Realm.GetInstance(config);
#elif DEBUG_EXTRA
        private static RealmConfiguration config = new RealmConfiguration(Constants.RealmPath);
        private static Realm _realm = Realm.GetInstance(config);
#else
        private static readonly Realm realm = Realm.GetInstance(config);
#endif

        public static void MainLoop(object cancToken)
        {
            CancellationToken token = (CancellationToken)cancToken;
            var queue = CommunicationLayer.KeyPressedQueue;
            var allResults = _realm.All<IntervalStatistics>();
            int _intervalId = allResults.Count();
            var timeout = Constants.ThreadLooplength;

            while (!token.IsCancellationRequested)
            {
                //blocking timed call
                VirtualKey keycode;
                if (queue.TryTake(out keycode, timeout, token))
                {
                    var now = DateTimeOffset.Now;

                    if (!_typing)
                    {
                        // special case if interval started with invalid key, it's not considered started
                        if (!IsValideKeyPressed(keycode))
                        {
                            continue;
                        }
                        _startedSampling = now;
                        _keyPressedCounter = 0;
                        _typing = true;
                    }

                    if (IsValideKeyPressed(keycode))
                    {
                        _keyPressedCounter++;
                    }

                    timeout = (int)(_samplingLengths - (now - _startedSampling)).TotalMilliseconds;
                    if (timeout < 0)
                    {
                        timeout = 0;
                    }

                    _lastKeyPressedTime = now;
                }

                // if it's time to save, do so and reset timer to default value
                if (_typing && ((DateTimeOffset.Now - _startedSampling) >= _samplingLengths))
                {
                    _typing = false;
                    try
                    {
                        _realm.Write(() =>
                        {
                            var interval = new IntervalStatistics()
                            {
                                Id = _intervalId++,
                                Start = _startedSampling,
                                End = _lastKeyPressedTime
                            };
                            CalculateAllStatistics(interval, _samplingLengths);
                            _realm.Add(interval);

                        });
                    }
                    catch (Exception e)
                    {
                        Logger.PrintError($"While adding an interval in the database, exception {e} happened.");
                    }

                    timeout = Constants.ThreadLooplength;

#if DEBUG_BASIC
                    Debug.WriteLine("");
                    Debug.WriteLine("=======================");
                    foreach (var inter in allResults)
                    {
                        Debug.WriteLine("");
                        Debug.WriteLine(inter.ToString());
                    }
                    Debug.WriteLine("=======================");
                    Debug.WriteLine("");
#endif
                }
            }
        }

        private static void CalculateAllStatistics(IntervalStatistics interval, TimeSpan samplingLength)
        {
            interval.Cps = CalculateCps(samplingLength);
            interval.GrossWpm = CalculateGrossWpms(samplingLength);
            interval.NetWpm = CalculateNetWpm(samplingLength);
        }

        private static bool IsValideKeyPressed(VirtualKey key)
        {
            // TODO for now always true, in the future it will filter
            return true;
        }

        private static double CalculateNetWpm(TimeSpan samplingLength)
        {
            //TODO this needs uncorrected errors. I'll postpone this
            return 0;
        }

        private static double CalculateGrossWpms(TimeSpan samplingLength)
        {
            return (_keyPressedCounter / 5.0) / samplingLength.TotalMinutes;
        }

        private static double CalculateCps(TimeSpan samplingLength)
        {
            return (double)_keyPressedCounter / samplingLength.TotalSeconds;
        }
    }
}
