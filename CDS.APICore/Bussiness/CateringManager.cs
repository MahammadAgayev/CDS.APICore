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
    public class CateringManager : ICateringManager
    {
        private readonly IDbHelper _db;

        public CateringManager(IDbHelper db)
        {
            _db = db;
        }


        public void Create(Catering catering)
        {
            using var tx = _db.CreateTransaction();

            _db.Insert("Caterings", tx, new Dictionary<string, object>
            {
                { nameof(Catering.Name), catering.Name },
                { nameof(Catering.Comment), catering.Comment },
                { nameof(Catering.AddressText), catering.AddressText },
                { nameof(Catering.Latitude), catering.Latitude },
                { nameof(Catering.Longitude), catering.Longitude },
                { "CategoryId", catering.CateringCategory.Id}
            });

            tx.Commit();
        }

        public void CreateCategory(CateringCategory category)
        {
            using var tx = _db.CreateTransaction();

            _db.Insert("CateringCategories", tx, new Dictionary<string, object>
            {
                { nameof(CateringCategory.Name), category.Name },
                { nameof(CateringCategory.Comment), category.Comment },
                { nameof(CateringCategory.DisplayName), category.DisplayName }
            });

            tx.Commit();
        }

        public List<Catering> Get()
        {
            var data = _db.JoinedGet("Caterings", this.getColumns(), new Join[]
            {
               this.getJoinWithCategory()
            }, null);

            var list = new List<Catering>();

            foreach (var row in data.Rows.OfType<DataRow>())
            {
                list.Add(this.getFromDbRow(row));
            }

            return list;
        }

        public Catering Get(int id)
        {
            var data = _db.JoinedGet("Caterings", this.getColumns(), new Join[]
             {
               this.getJoinWithCategory()
             }, new Dictionary<string, object> { { "Id", id } }, new JoinFilter { Comparison = Comparison.Equal, Name = "Id", TableName = "Caterings" });

            return data.Rows.Count > 0 ? this.getFromDbRow(data.Rows[0]) : null;
        }


        private Catering getFromDbRow(DataRow row)
        {
            var catering = new Catering
            {
                Id = DbConverter.ToInt32(row, nameof(Catering.Id)),
                Name = DbConverter.ToString(row, nameof(Catering.Name)),
                Comment = DbConverter.ToString(row, nameof(Catering.Comment)),
                AddressText = DbConverter.ToString(row, nameof(Catering.AddressText)),
                Latitude = DbConverter.ToDecimal(row, nameof(Catering.Latitude)),
                Longitude = DbConverter.ToDecimal(row, nameof(Catering.Longitude)),
                CateringCategory = new CateringCategory
                {
                    Id = DbConverter.ToInt32(row, "CategoryId"),
                    Name = DbConverter.ToString(row, "CateringName"),
                    Comment = DbConverter.ToString(row, "CateringComment"),
                    DisplayName = DbConverter.ToString(row, "DisplayName"),
                }
            };

            return catering;
        }

        private Join getJoinWithCategory() => new Join
        {
            TableName = "CateringCategories",
            Columns = new string[] { "Name as CateringName", "Comment as CateringComment", "DisplayName" },
            JoinColumn = "Id",
            JoinsToColumn = "CategoryId",
            JoinsToTableName = "Caterings",
        };

        private string[] getColumns() => new string[]
        {
            "Id",
            "Name",
            "Comment",
            "AddressText",
            "Latitude",
            "Longitude",
            "CategoryId"
        };
    }
}