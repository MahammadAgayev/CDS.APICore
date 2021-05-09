using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.DataAccess;
using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness
{
    public class LocationManager : ILocationManager
    {
        private readonly IDbHelper _db;
        public LocationManager(IDbHelper db)
        {
            _db = db;
        }

        public List<CustomerMostUsedLocation> GetLocations(int customerId)
        {
            var data = _db.JoinedGet("CustomerMostUsedLocations",
                new string[] { "Id", "Latitude", "Longitude", "ActiveStartTime", "ActiveEndTime" }, this.getJoins(),
                new Dictionary<string, object>
                {
                    { "CustomerId", customerId }
                }, new JoinFilter { TableName = "Customers", Name = "Id", Comparison = Comparison.Equal });

            var list = new List<CustomerMostUsedLocation>();

            foreach (var item in data.Rows.OfType<DataRow>())
            {
                list.Add(this.getFromDbRow(item));
            }

            return list;
        }

        public void Save(CustomerLocation location)
        {
            using var tx = _db.CreateTransaction();

            _db.Insert("CustomerLocations", tx, new Dictionary<string, object>
            {
                { nameof(CustomerLocation.Id), location.Id },
                { nameof(CustomerLocation.Latitude), location.Latitude },
                { nameof(CustomerLocation.Longitude), location.Longitude },
                { "CustomerId", location.Customer.Id }
            });

            tx.Commit();
        }


        private Join[] getJoins()
        {
            return new Join[]
            {
                new Join
                {
                    TableName = "Customers",
                    JoinColumn = "Id",
                    JoinsToColumn = "CustomerId",
                    JoinsToTableName = "CustomerMostUsedLocations",
                    Columns = new string[] { "Firstname", "Lastname", "Email", "Phone","Created","CommunicationType", "IdentityTag"},
                }
            };
        }

        private CustomerMostUsedLocation getFromDbRow(DataRow row)
        {
            return new CustomerMostUsedLocation
            {
                Id = row.Field<int>("Id"),
                ActiveStartTime = row.Field<DateTime>("ActiveStartTime"),
                ActiveEndTime = row.Field<DateTime>("ActiveEndTime"),
                Latitude = row.Field<decimal>("Latitude"),
                Longitude = row.Field<decimal>("Longitude"),
                Customer = new Customer
                {
                    Id = row.Field<int>("CustomerId"),
                    FirstName = row.Field<string>("FirstName"),
                    LastName = row.Field<string>("LastName"),
                    Email = row.Field<string>("Email"),
                    Phone = row.Field<string>("Phone"),
                    Created = row.Field<DateTime>("Created"),
                    Updated = row.Field<DateTime>("Updated")
                }
            };
        }
    }
}
