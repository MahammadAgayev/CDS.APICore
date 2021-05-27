using System;

namespace CDS.APICore.Models.Agregation
{
    public class AggregationCustomerAndCateringDataItem
    {
        public AggregationDataItem AggregationDataItem { get; set; }
        public AggregationCateringData  AggregationCateringData { get; set; }
        public AggregationCustomerData AggregationCustomerData { get; set; }
    }

    public class AggregationCustomerDataItem
    {
        public AggregationDataItem AggregationDataItem { get; set; }
        public AggregationCustomerData AggregationCustomerData { get; set; }
    }

    public class AggregationCateringDataItem 
    {
        public AggregationDataItem AggregationDataItem { get; set; }
        public AggregationCateringData AggregationCateringData { get; set; }
    }

    public class AggregationDataItem
    {
        public int Count { get; set; }
        public decimal Average { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Sum { get; set; }
        public string AgregationBy { get; set; }
        public string PeriodType { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime Created { get; set; }
    }

    public class AggregationCateringData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string CategoryComment { get; set; }
        public string CategoryDisplayName { get; set; }
        public string Comment { get; set; }
        public string AddressText { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class AggregationCustomerData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IdentityTag { get; set; }
    }
}
