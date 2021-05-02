
using System;
using System.Collections.Generic;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Entities;
using CDS.APICore.Helpers;
using CDS.APICore.Models.Order;

namespace CDS.APICore.Services
{
    public interface IOrderService
    {
        void AddOrder(AddOrderRequest request);
        void AddOrderItemCategory(AddItemCategeoryRequest request);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderManager _orderManager;
        private readonly ICustomerManager _customerManager;
        private readonly ITimeManager _timeManager;
        private readonly ICateringManager _cateringManager;

        public OrderService(IOrderManager orderManager, ICustomerManager customerManager, ITimeManager timeManager, ICateringManager cateringManager)
        {
            _orderManager = orderManager;
            _customerManager = customerManager;
            _timeManager = timeManager;
            _cateringManager = cateringManager;
        }

        public void AddOrder(AddOrderRequest request)
        {
            var customer = _customerManager.Get(request.CustomerIdentityTag);

            if(customer is null)
            {
                customer = new Entities.Customer
                {
                    IdentityTag = request.CustomerIdentityTag,
                    Email = request.CustomerEmail,
                    Phone = request.CustomerPhone,
                    FirstName = request.CustomerFirstname,
                    LastName = request.CustomerLastname,
                    Updated = DateTime.Now,
                    CommunicationType = SystemDefaults.DefaultCommunicationType
                };

                _customerManager.Create(customer);
            }

            var order = new Order
            {
                Created = _timeManager.Parse(request.CreationDate, SystemDefaults.DefaultFormat),
                Customer = customer,
                TotalPrice = request.TotalPrice,
                Catering = _cateringManager.Get(request.CateringId) ?? throw new AppException($"Caterign with '{request.CateringId}' doesn't exists")
            };

            var orderItems = new List<OrderItem>();

            foreach (var item in request.AddOrderRequestItems)
            {
                ItemCategory category;
                if((category = _orderManager.GetCategory(item.CategoryId)) == null)
                {
                    throw new AppException($"Category with '{item.CategoryId}' doesn't exists");
                }

                var t = new OrderItem
                {
                    ItemName = item.Name,
                    Price = item.Price,
                    Order = order,
                    ItemCategory = category
                };

                orderItems.Add(t);
            }

            order.Items = orderItems;

            _orderManager.Create(order);
        }

        public void AddOrderItemCategory(AddItemCategeoryRequest request)
        {
            var category = new ItemCategory
            {
                Name = request.Name,
                Comment = request.Comment,
                DisplayName = request.DisplayName
            };

            _orderManager.CreateItemCategory(category);
        }
    }
}
