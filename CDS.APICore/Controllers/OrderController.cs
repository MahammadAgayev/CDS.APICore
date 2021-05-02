
using CDS.APICore.Helpers;
using CDS.APICore.Models.Order;
using CDS.APICore.Services;

using Microsoft.AspNetCore.Mvc;

namespace CDS.APICore.Controllers
{
    [Authorize]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService _ordersevice;

        public OrderController(IOrderService orderService)
        {
            _ordersevice = orderService;
        }

        [HttpPost("addorder")]
        public IActionResult AddOrder(AddOrderRequest request)
        {
            _ordersevice.AddOrder(request);

            return Created();
        }

        [HttpPost("addorderitemcategory")]
        public IActionResult AddOrderItemCatgory(AddItemCategeoryRequest request)
        {
            _ordersevice.AddOrderItemCategory(request);

            return Created();
        }

    }
}
