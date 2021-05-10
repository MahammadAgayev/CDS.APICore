using System;
using System.Collections.Generic;

using CDS.APICore.Entities;
using CDS.APICore.Entities.Enums;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface IAgregationManager
    {
        void AgregateCaterings(PeriodType periodType, DateTime from);
        void AgregateCustomers(PeriodType periodType, DateTime from);
        void AgregateCateringCustomers(PeriodType periodType, DateTime from);
        void AggregateLocations(PeriodType periodType, DateTime from);

        List<Agregation> GetAgregations(PeriodType periodType, DateTime from, AgregationBy agregationBy);
        List<Agregation> GetAgregations(string tag);
    }
}