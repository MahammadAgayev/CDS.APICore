using System;

using CDS.APICore.Entities.Enums;

namespace CDS.APICore.Entities
{
    public class Agregation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public AgregationBy AgregationBy { get; set; }
        public PeriodType PeriodType { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime Created { get; set; }
    }
}
