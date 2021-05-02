using System.Collections.Generic;

namespace CDS.APICore.Models.Order
{
    public class AddOrderRequest
    {
        public string CreationDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerIdentityTag { get; set; }
        public string CustomerFirstname { get; set; }
        public string CustomerLastname { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public int CateringId { get; set; }

        public List<AddOrderRequestItem> AddOrderRequestItems { get; set; }
    }
}