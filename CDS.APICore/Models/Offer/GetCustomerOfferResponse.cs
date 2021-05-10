using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.Models.Offer
{
    public class GetCustomerOfferResponse
    {
        public string CateringName { get; set; }
        public string AddressText { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}