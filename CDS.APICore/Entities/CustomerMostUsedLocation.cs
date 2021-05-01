using System;

namespace CDS.APICore.Entities
{
    public class CustomerMostUsedLocation
    {
        public int Id { get; set; }
        public CustomerLocation Location { get; set; }
        public string Tag { get; set; }
        public DateTime ActiveStartTime { get; set; }
        public DateTime ActiveEndTime { get; set; }
    }
}
