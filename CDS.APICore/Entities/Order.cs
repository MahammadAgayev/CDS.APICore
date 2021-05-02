using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Created { get; set; }
        public DateTime Inserted { get; set; }
        public Catering Catering { get; set; }

        public List<OrderItem> Items { get; set; }
    }
}
