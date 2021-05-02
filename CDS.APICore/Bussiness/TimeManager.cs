using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;

namespace CDS.APICore.Bussiness
{
    public class TimeManager : ITimeManager
    {
        public DateTime Now => DateTime.Now;

        public DateTime Parse(string rawdate, string format)
        {
            return DateTime.ParseExact(rawdate, format, CultureInfo.InvariantCulture);
        }
    }
}
