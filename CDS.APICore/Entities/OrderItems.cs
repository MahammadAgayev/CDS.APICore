using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.Entities
{
    public class OrderItems
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
    }
}
