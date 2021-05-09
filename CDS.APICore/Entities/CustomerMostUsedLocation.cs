using System;

namespace CDS.APICore.Entities
{
    public class CustomerMostUsedLocation
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime ActiveStartTime { get; set; }
        public DateTime ActiveEndTime { get; set; }
    }
}
