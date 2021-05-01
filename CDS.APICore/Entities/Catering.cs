using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.Entities
{
    public class Catering
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CateringCategory CateringCategory { get; set; }
        public string Comment { get; set; }
        public string AddressText { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
