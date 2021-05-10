using System;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface ITimeManager
    {
        DateTime Parse(string rawdate, string format);
        DateTime Now { get; }

        DateTime FromUnixTime(double unixTimestamp);

        bool CheckIfWeekend(DateTime date);
        bool CheckIfTodayWeekend() => this.CheckIfWeekend(this.Now);

    }
}
