using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Constants;
using CDS.APICore.DataAccess;
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

                aggrr.PeriodStart = _timeManager.FromUnixTime(item.Field<int>("periodtsart"));
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

                aggrr.PeriodStart = _timeManager.FromUnixTime(item.Field<int>("periodtsart"));
                aggrr.PeriodStart = _timeManager.FromUnixTime(item.Field<int>("periodtsart"));
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

                aggrr.PeriodStart = _timeManager.FromUnixTime(item.Field<int>("periodtsart"));
                aggrr.AgregationBy = AgregationBy.CustomerCatering;
                aggrr.Tag = _tagManager.Tag(new Dictionary<string, string> 
                {
                    { AgregationConstants.CustomerId, customerId.ToString() },
                    { AgregationConstants.CateringId, cateringId.ToString() }
                });
                aggrr.Name = $"Aggr_{_customerManager.Get(customerId).IdentityTag}_{periodType}";
                aggrr.PeriodType = periodType;

                this.insertAggr(aggrr, tx);
            }

            tx.Commit();
        }

        public void AggregateLocations(PeriodType periodType, DateTime from)
        {
            using var tx = _db.CreateTransaction();

            string query = this.createLocationAgregationQuery(periodType, from);

            var data = _db.Query(query, tx, new Dictionary<string, object> { { "from", from } });

            foreach (var item in data.Rows.OfType<DataRow>())
            {
                var locData = this.getLocationFromDbRow(item);

                locData.Customer = _customerManager.Get(item.Field<int>("CustomerId"));

                _db.Insert("CustomerMostUsedLocations", tx, _reflectionHelper.GetKeyValue(locData));
            }

            tx.Commit();
        }

        public List<Agregation> GetAgregations(PeriodType periodType, DateTime from, AgregationBy agregationBy)
        {
            var data = _db.SimpleGet("Agregations", _reflectionHelper.GetPropNames(typeof(Agregation)), new Dictionary<string, object>
            {
                { nameof(Agregation.PeriodType), periodType },
                { nameof(Agregation.AgregationBy), agregationBy },
                { nameof(Agregation.PeriodStart), from}
            }, new Filter { Comparison = Comparison.Equal, Name = nameof(Agregation.PeriodType)},
               new Filter { Comparison = Comparison.Equal, Name = nameof(Agregation.AgregationBy) },
               new Filter { Comparison = Comparison.GreaterThan, Name = nameof(Agregation.PeriodStart) });

            var agregations = new List<Agregation>();

            foreach (var item in data.Rows.OfType<DataRow>())
            {
                var agrr = this.getFromDbRow(item);

                agrr.PeriodStart = item.Field<DateTime>("PeriodStart");
                agrr.PeriodType = item.Field<PeriodType>("PeriodType");
                agrr.Tag = item.Field<string>("Tag");
                agrr.AgregationBy = item.Field<AgregationBy>("AgregationBy");
                agrr.Id = item.Field<int>("Id");

                agregations.Add(agrr);
            }

            return agregations;
        }

        public List<Agregation> GetAgregations(string tag)
        {
            var data = _db.SimpleGet("Agregations", _reflectionHelper.GetPropNames(typeof(Agregation)), new Dictionary<string, object>
            {
                { nameof(Agregation.Tag), tag },
            }, new Filter { Comparison = Comparison.Equal, Name = nameof(Agregation.Tag) });

            var agregations = new List<Agregation>();

            foreach (var item in data.Rows.OfType<DataRow>())
            {
                var agrr = this.getFromDbRow(item);

                agrr.PeriodStart = item.Field<DateTime>("PeriodStart");
                agrr.PeriodType = item.Field<PeriodType>("PeriodType");
                agrr.Tag = item.Field<string>("Tag");
                agrr.AgregationBy = item.Field<AgregationBy>("AgregationBy");
                agrr.Id = item.Field<int>("Id");

                agregations.Add(agrr);
            }

            return agregations;
        }



        private string createQueryDynamic(string tablename, string countableColumn, string dateColumn, PeriodType periodType, DateTime from ,params string[] groupColumns)
        {
            string groupedColumnsQuery = groupColumns is null ? "" : $",{string.Join(",", groupColumns)}";

            return @$"select FLOOR(DATEDIFF(second, '1970-01-01', {dateColumn})/{this.getDiffSecond(periodType)})*{this.getDiffSecond(periodType)} as periodtsart, count(1) as [count], sum({countableColumn}) as [sum], avg({countableColumn}) as [average], min({countableColumn}) as [min],
                     max({countableColumn}) as [max]
                     {groupedColumnsQuery}
                     from {tablename}
                     where {dateColumn} > @from
                     group by FLOOR(DATEDIFF(second, '1970-01-01', {dateColumn})/{this.getDiffSecond(periodType)})*{this.getDiffSecond(periodType)} {groupedColumnsQuery}";
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

        private string createLocationAgregationQuery(PeriodType periodType, DateTime from)
        {
            return @$"select FLOOR(DATEDIFF(second, '1970-01-01', Created)/{this.getDiffSecond(periodType)})*{this.getDiffSecond(periodType)} as periodtsart, 
                      avg(Latitude) avglatitude,
                      avg(Longitude) avglongitude,
                     CustomerId,
                     from CustomerLocations
                     where Created > @from
                     group by FLOOR(DATEDIFF(second, '1970-01-01', Created)/{this.getDiffSecond(periodType)})*{this.getDiffSecond(periodType)}, CustomerId";
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
            agregation.Created = DateTime.Now;

            var keyValues = _reflectionHelper.GetKeyValue(agregation);

            _db.Insert("Agregations", tx, keyValues.Where(x => x.Key.ToLowerInvariant() != "id").ToDictionary(x => x.Key, x => x.Value));
        }


        private Agregation getFromDbRow(DataRow row)
        {
            return new Agregation
            {
                Count = row.Field<int>("count"),
                Sum = row.Field<decimal>("sum"),
                Average = row.Field<decimal>("average"),
                Min = row.Field<decimal>("min"),
                Max = row.Field<decimal>("max"),
            };
        }

        private CustomerMostUsedLocation getLocationFromDbRow(DataRow row)
        {
            return new CustomerMostUsedLocation
            {
                Latitude = row.Field<decimal>("avglatitude"),
                Longitude = row.Field<decimal>("avglongitude"),
                ActiveStartTime = _timeManager.FromUnixTime(row.Field<int>("periodtsart"))
            };
        }
    }
}
