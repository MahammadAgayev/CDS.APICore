﻿using System;
using System.Collections.Generic;
using System.Data;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.DataAccess;
using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness
{
    public class OrderManager : IOrderManager
    {
        private readonly IDbHelper _db;

        public OrderManager(IDbHelper db)
        {
            _db = db;
        }

        public void Create(Order order)
        {
            using var tx = _db.CreateTransaction();

            var dt = _db.Insert("Orders", tx, new Dictionary<string, object>
            {
                { nameof(Order.Inserted), DateTime.Now },
                { nameof(Order.Created), order.Created },
                { nameof(Order.TotalPrice), order.TotalPrice },
                { "CateringId", order.Catering.Id },
                { "CustomerId", order.Customer.Id }
            });

            order.Id = Convert.ToInt32(dt["Id"]);

            foreach (var item in order.Items)
            {
                _db.Insert("OrderItems", tx, new Dictionary<string, object>
                {
                    {  nameof(OrderItem.ItemName), item.ItemName},
                    { nameof(OrderItem.Price), item.Price },
                    { "OrderId", item.Order.Id },
                    { "CategoryId", item.ItemCategory.Id }
                });
            }

            tx.Commit();
        }

        public void CreateItemCategory(ItemCategory category)
        {
            using var tx = _db.CreateTransaction();

            _db.Insert("ItemCategories", tx, new Dictionary<string, object>
            {
                { nameof(ItemCategory.Name), category.Name },
                { nameof(ItemCategory.Comment), category.Comment },
                { nameof(ItemCategory.DisplayName), category.DisplayName }
            });

            tx.Commit();
        }

        public ItemCategory GetCategory(int id)
        {
            var data = _db.SimpleGet("ItemCategories", _db.GetColumns(typeof(ItemCategory)), new Dictionary<string, object>
            {
                { "Id" ,  id}
            }, new Filter { Comparison = Comparison.Equal, Name = "Id" });

            return data.Rows.Count > 0 ? this.getFromDbRow(data.Rows[0]) : null;
        }


        private ItemCategory getFromDbRow(DataRow row)
        {
            return new ItemCategory
            {
                Id = DbConverter.ToInt32(row, "Id"), 
                Comment = DbConverter.ToString(row, "Comment"),
                DisplayName = DbConverter.ToString(row, "DisplayName"),
                Name = DbConverter.ToString(row, "Name")
            };
        }
    }
}