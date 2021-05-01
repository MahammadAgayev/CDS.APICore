using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.Entities
{
    public class CustomerLocation
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
