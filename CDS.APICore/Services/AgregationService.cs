using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;

namespace CDS.APICore.Services
{
    public interface IAgregationService
    {
    }

    public class AgregationService :  IAgregationService
    {
        private readonly IAgregationManager _agregationManager;

        public AgregationService(IAgregationManager agregationManager)
        {
            _agregationManager = agregationManager;

            
        }
    }
}