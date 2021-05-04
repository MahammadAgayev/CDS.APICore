using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Entities.Enums;
using CDS.APICore.Models.Agregation;

namespace CDS.APICore.Bussiness
{
    public class AgregationManager : IAgregationManager
    {
        private readonly IDbHelper _db;
        public AgregationManager(IDbHelper db)
        {
            _db = db;
        }

        public void Aggregate(AgregationSettings settings)
        {
            
        }

        private string createQuery(AgregationSettings settings) => settings.AgregationBy switch
        {
            //AgregationBy.Catering => this.createDynamicQuery("Caterings", "TotalAmount", "")
        };

        private string createDynamicQuery(string tablename, string countableColumn, string groupColumn, PeriodType periodType)
        {
            return @$"select count(1) as [count], sum({countableColumn}) as [sum], avg({countableColumn}) as [avg], min({countableColumn}) as [min],
                     max({countableColumn}) as max from {tablename}
                     group by FLOOR(DATEDIFF(second, '1970-01-01', {groupColumn})/{this.getDiffSecond(periodType)})*{this.getDiffSecond(periodType)}";
        }


        private int getDiffSecond(PeriodType periodType) => periodType switch
        {
            PeriodType.Daily => 86400,
            PeriodType.Hourly => 3600,
            PeriodType.Monthly => 2592000,
            PeriodType.Yearly => 220752000,
            _ => 86400,
        };
    }
}
