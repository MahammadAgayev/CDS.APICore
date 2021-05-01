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

        public void AgregateLocations()
        {
            //TODO
        }

        public List<CustomerMostUsedLocation> GetLocations(int customerId)
        {
            var data = _db.JoinedGet("CustomerMostUsedLocations",
                new string[] { "Id", "LocationId", "Tag", "ActiveStartTime", "ActiveEndTime" }, this.getJoins(),
                new Dictionary<string, object>
                {
                    { "CustomerId", customerId }
                }, new JoinFilter { TableName = "CustomerLocations", Name = "CustomerId", Comparison = Comparison.Equal });

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
                    TableName = "CustomerLocations",
                    JoinColumn = "Id",
                    JoinsToColumn = "LocationId",
                    JoinsToTableName = "CustomerMostUsedLocations",
                    Columns = new string[] { "CustomerId", "Latitude","Longitude" }
                },
                new Join
                {
                    TableName = "Customers",
                    JoinColumn = "Id",
                    JoinsToColumn = "CustomerId",
                    JoinsToTableName = "CustomerLocations",
                    Columns = new string[] { "Firstname", "Lastname", "Email", "Phone","Created","CommunicationType"},
                }
            };
        }

        private CustomerMostUsedLocation getFromDbRow(DataRow row)
        {
            return new CustomerMostUsedLocation
            {
                Id = Convert.ToInt32(row["Id"]),
                Tag = row["Tag"].ToString(),
                ActiveStartTime = Convert.ToDateTime(row["ActiveStartTime"]),
                ActiveEndTime = Convert.ToDateTime(row["ActiveEndTime"]),
                Location = new CustomerLocation
                {
                    Id = Convert.ToInt32(row["LocationId"]),
                    Latitude = Convert.ToDecimal(row["Latitude"]),
                    Longitude = Convert.ToDecimal(row["Longitude"]),
                    Customer = new Customer
                    {
                        Id = Convert.ToInt32(row["CustomerId"]),
                        FirstName = row["FirstName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        Email = row["Email"].ToString(),
                        Phone = row["Phone"].ToString(),
                        Created = Convert.ToDateTime(row["Created"]),
                        Updated = Convert.ToDateTime(row["Updated"])
                    }
                }
            };
        }
    }
}
