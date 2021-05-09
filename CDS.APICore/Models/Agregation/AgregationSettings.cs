
using CDS.APICore.Entities.Enums;

namespace CDS.APICore.Models.Agregation
{
    public class AgregationSettings
    {
        public string Name { get; set; }
        public AgregationBy AgregationBy { get; set; }
        public PeriodType PeriodType { get; set; }
    }
}
