using CDS.APICore.Entities.Enums;

namespace CDS.APICore.Models.Agregation
{
    public class GetAggregationDataRequest
    {
        public AgregationBy AgregationBy { get; set; }

        public AggregationRequestTimeRangeType TimeRangeType { get; set; }
    }
}
