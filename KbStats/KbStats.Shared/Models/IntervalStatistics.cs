using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace KbStats.Models
{
    using Cps = Double;
    using Wpm = Double;

    public class IntervalStatistics : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public TimeSpan Duration => (End - Start);

        public Cps Cps { get; set; }
        public Wpm GrossWpm { get; set; }
        public Wpm NetWpm { get; set; }

        public IntervalStatistics() { }

        public void Reset()
        {
            Start = DateTimeOffset.Now;
            End = DateTimeOffset.Now;

            Cps = 0.0;
            GrossWpm = 0.0;
            NetWpm = 0.0;
        }

        public override string ToString()
        {
            return $@"Id = {Id}
Cpses = {Cps}
GrossWpms = {GrossWpm}
Start = {Start}
End = {End}";
        }
    }
}
