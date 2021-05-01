using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface ILocationManager
    {
        void Save(CustomerLocation location);

        void AgregateLocations();

        List<CustomerMostUsedLocation> GetLocations(int customerId);
    }
}
