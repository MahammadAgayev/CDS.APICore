using System;
using System.Collections.Generic;
using System.Data;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.DataAccess;
using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness
{
    public class CustomerManager : ICustomerManager
    {
        private readonly IDbHelper _db;

        public CustomerManager(IDbHelper db)
        {
            _db = db;
        }

        public void Create(Customer customer)
        {
            using var tx = _db.CreateTransaction();

            var row = _db.Insert("Customers", tx, new Dictionary<string, object>
            {
                { nameof(Customer.FirstName), customer.FirstName },
                { nameof(Customer.LastName), customer.LastName },
                { nameof(Customer.Phone), customer.Phone },
                { nameof(Customer.Email), customer.Email },
                { nameof(Customer.CommunicationType), customer.CommunicationType },
                { nameof(Customer.Created), DateTime.Now},
                { nameof(Customer.Updated), DateTime.Now },
                { nameof(Customer.IdentityTag), customer.IdentityTag }
            });

            customer.Id = Convert.ToInt32(row["Id"]);

            tx.Commit();
        }

        public Customer Get(int id)
        {
            var data = _db.SimpleGet("Customers", _db.GetColumns(typeof(Customer)), new Dictionary<string, object>
            {
                { "Id", id }
            }, new Filter { Comparison = Comparison.Equal, Name = "Id" });

            if (data.Rows.Count == 0)
                return null;

            return this.getFromDbRow(data.Rows[0]);
        }

        public Customer Get(string identitytag)
        {
            DataTable data = null;

            data = _db.SimpleGet("Customers", _db.GetColumns(typeof(Customer)), new Dictionary<string, object>
            {
                { "IdentityTag",  identitytag}
            }, new Filter { Comparison = Comparison.Equal, Name = "IdentityTag" });

            if(data.Rows.Count == 0)
            {
                return null;
            }

            return this.getFromDbRow(data.Rows[0]);
        }


        private Customer getFromDbRow(DataRow row)
        {
            return new Customer
            {
                Id = Convert.ToInt32(row["Id"]),
                FirstName = row["Firstname"].ToString(),
                LastName = row["Lastname"].ToString(),
                Email = row["Email"].ToString(),
                Phone = row["Phone"].ToString(),
                Created = Convert.ToDateTime(row["Created"]),
                Updated = Convert.ToDateTime(row["Updated"])
            };
        }
    }
}
