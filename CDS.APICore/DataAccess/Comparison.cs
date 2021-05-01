using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.DataAccess
{
    public enum Comparison : byte
    {
        Equal = 1,
        Greater = 2, 
        Lower = 3,
        GreaterThan = 4,
        LowerThan  = 5
    }
}
