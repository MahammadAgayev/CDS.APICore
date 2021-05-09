using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Constants;
using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Entities;
using CDS.APICore.Entities.Enums;
using CDS.APICore.Models.Agregation;

namespace CDS.APICore.Bussiness
{
    public class AgregationManager : IAgregationManager
    {
        private readonly IDbHelper _db;

        private readonly ITimeManager _timeManager;
        private readonly ICateringManager _cateringManager;
        private readonly IReflectionHelper _reflectionHelper;
        private readonly ICustomerManager _customerManager;
        private readonly ITagManager _tagManager;

        public AgregationManager(IDbHelper db,
            ITimeManager timeManager,
            ICateringManager cateringManager,
            IReflectionHelper reflectionHelper,
            ICustomerManager customerManager,
            ITagManager tagManager)
        {
            _db = db;
            _timeManager = timeManager;
            _cateringManager = cateringManager;
            _reflectionHelper = reflectionHelper;
            _customerManager = customerManager;
            _tagManager = tagManager;
        }

        public void AgregateCaterings(PeriodType periodType, DateTime from)
        {
            using var tx = _db.CreateTransaction();

            string query = this.createCateringAgregationQuery(periodType, from);

            var data = _db.Query(query, tx, new Dictionary<string, object> { { "from", from } });

            foreach (var item in data.Rows.OfType<DataRow>())
            {
                var aggrr = this.getFromDbRow(item);

                int cateringId = item.Field<int>(AgregationConstants.CateringId);

                aggrr.AgregationBy = AgregationBy.Catering;
                aggrr.Tag = _tagManager.Tag(new Dictionary<string, string> { { AgregationConstants.CateringId, cateringId.ToString() } });
                aggrr.Name = $"Aggr_{_cateringManager.Get(cateringId).Name}_{periodType}";
                aggrr.PeriodType = periodType;

                this.insertAggr(aggrr, tx);
            }

            tx.Commit();
        }

        public void AgregateCustomers(PeriodType periodType, DateTime from)
        {
            using var tx = _db.CreateTransaction();

            string query = this.createCustomerAgregationQuery(periodType, from);

            var data = _db.Query(query, tx, new Dictionary<string, object> { { "from", from } });

            foreach (var item in data.Rows.OfType<DataRow>())
            {
                var aggrr = this.getFromDbRow(item);

                int customerId = item.Field<int>(AgregationConstants.CustomerId);

                aggrr.AgregationBy = AgregationBy.Customer;
                aggrr.Tag = _tagManager.Tag(new Dictionary<string, string> { { AgregationConstants.CustomerId, customerId.ToString() } });
                aggrr.Name = $"Aggr_{_customerManager.Get(customerId).IdentityTag}_{periodType}";
                aggrr.PeriodType = periodType;

                this.insertAggr(aggrr, tx);
            }

            tx.Commit();
        }

        public void AgregateCateringCustomers(PeriodType periodType, DateTime from)
        {
            using var tx = _db.CreateTransaction();

            string query = this.createCustomerCateringAgregationQuery(periodType, from);

            var data = _db.Query(query, tx, new Dictionary<string, object> { { "from", from } });

            foreach (var item in data.Rows.OfType<DataRow>())
            {
                var aggrr = this.getFromDbRow(item);

                int customerId = item.Field<int>(AgregationConstants.CustomerId);
                int cateringId = item.Field<int>(AgregationConstants.CateringId);

                aggrr.AgregationBy = AgregationBy.Customer;
                aggrr.Tag = _tagManager.Tag(new Dictionary<string, string> { { AgregationConstants.CustomerId, customerId.ToString() }, 
                    { AgregationConstants.CateringId, cateringId.ToString() } });
                aggrr.Name = $"Aggr_{_customerManager.Get(customerId).IdentityTag}_{periodType}";
                aggrr.PeriodType = periodType;

                this.insertAggr(aggrr, tx);
            }

            tx.Commit();
        }


        private string createQueryDynamic(string tablename, string countableColumn, string dateColumn, PeriodType periodType, DateTime from ,params string[] groupColumns)
        {
            string groupedColumnsQuery = groupColumns is null ? "" : $",{string.Join(",", groupColumns)}";

            return @$"select FLOOR(DATEDIFF(second, '1970-01-01', {dateColumn})/{this.getDiffSecond(periodType)})*{this.getDiffSecond(periodType)} as periodtsart, count(1) as [count], sum({countableColumn}) as [sum], avg({countableColumn}) as [avg], min({countableColumn}) as [min],
                       max({countableColumn}) as [max]
                      {groupedColumnsQuery}
                     from {tablename}
                     group by FLOOR(DATEDIFF(second, '1970-01-01', {dateColumn})/{this.getDiffSecond(periodType)})*{this.getDiffSecond(periodType)} {groupedColumnsQuery}
                     where {dateColumn} > @from";
        }

        private string createCateringAgregationQuery(PeriodType periodType, DateTime from)
        {
            return this.createQueryDynamic("Orders", "TotalAmount", "Created", periodType, from, AgregationConstants.CateringId);
        }

        private string createCustomerAgregationQuery(PeriodType periodType, DateTime from)
        {
            return this.createQueryDynamic("Orders", "TotalAmount", "Created", periodType, from, AgregationConstants.CustomerId);
        }

        private string createCustomerCateringAgregationQuery(PeriodType periodType, DateTime from)
        {
            return this.createQueryDynamic("Orders", "TotalAmount", "Created", periodType, from, AgregationConstants.CustomerId, AgregationConstants.CateringId);
        }

        private int getDiffSecond(PeriodType periodType) => periodType switch
        {
            PeriodType.Daily => 86400,
            PeriodType.Hourly => 3600,
            PeriodType.Monthly => 2592000,
            PeriodType.Yearly => 220752000,
            _ => 86400,
        };

        private void insertAggr(Agregation agregation, IDbTransaction tx)
        {
            var keyValues = _reflectionHelper.GetKeyValue(agregation);

            _db.Insert("Aggregations", tx, keyValues.Where(x => x.Key.ToLowerInvariant() != "id").ToDictionary(x => x.Key, x => x.Value));
        }


        private Agregation getFromDbRow(DataRow row)
        {
            return new Agregation
            {
                PeriodStart = _timeManager.FromUnixTime(row.Field<double>("periodtsart")),
                Count = row.Field<int>("count"),
                Sum = row.Field<decimal>("sum"),
                Average = row.Field<decimal>("avg"),
                Min = row.Field<decimal>("min"),
                Max = row.Field<decimal>("max"),
            };
        }
    }
}
