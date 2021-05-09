using System;
using System.Globalization;

using CDS.APICore.Bussiness.Abstraction;

namespace CDS.APICore.Bussiness
{
    public class TimeManager : ITimeManager
    {
        private readonly DateTime _unix = new DateTime(1970,1,1,0,0,0,0, System.DateTimeKind.Utc);


        public DateTime Now => DateTime.Now;

        public DateTime FromUnixTime(double unixTimestamp)
        {
            return _unix.AddSeconds(unixTimestamp).ToLocalTime();
        }

        public DateTime Parse(string rawdate, string format)
        {
            return DateTime.ParseExact(rawdate, format, CultureInfo.InvariantCulture);
        }
    }
}