using System;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface ITimeManager
    {
        DateTime Parse(string rawdate, string format);
        DateTime Now { get; }
    }
}
