using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Models.Agregation;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface IAgregationManager
    {
        void Aggregate(AgregationSettings settings);
    }
}
