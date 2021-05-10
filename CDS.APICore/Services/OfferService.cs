using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Constants;
using CDS.APICore.Helpers;
using CDS.APICore.Models.Offer;

namespace CDS.APICore.Services
{
    public interface IOfferService
    {
        GetCustomerOfferResponse GetCustomerOffer(GetCustomerOfferRequest request);
    }

    public class OfferService : IOfferService
    {
        private readonly IAgregationManager _agregationManager;
        private readonly ICustomerManager _customerManager;
        private readonly ITagManager _tagManager;
        private readonly ITimeManager _timeManager;
        private readonly ICateringManager _cateringManager;

        public OfferService(IAgregationManager agregationManager, 
            ICustomerManager customerManager, 
            ITagManager tagManager,
            ITimeManager timeManager,
            ICateringManager cateringManager)
        {
            _agregationManager = agregationManager;
            _customerManager = customerManager;
            _tagManager = tagManager;
            _timeManager = timeManager;
            _cateringManager = cateringManager;
        }

        public GetCustomerOfferResponse GetCustomerOffer(GetCustomerOfferRequest request)
        {
            var customer = _customerManager.Get(request.IdentityTag);

            if(customer is null)
            {
                throw new AppException("Customer not found");
            }

            var agregations = _agregationManager.GetAgregations(Entities.Enums.PeriodType.Daily, _timeManager.Now.AddYears(-1), Entities.Enums.AgregationBy.CustomerCatering);

            var customerAgregations = agregations.Where(x => _tagManager.UnTag(x.Tag)[AgregationConstants.CustomerId] == customer.Id.ToString());

            var customerAgrrGrouped = customerAgregations.GroupBy(x => x.Tag);

            //If weekend we are calculating by sum else is count. Because most money usage for catering are on weekend
            if(_timeManager.CheckIfTodayWeekend())
            {
                customerAgrrGrouped = customerAgrrGrouped.OrderBy(x => x.Sum(a => a.Sum));
            }
            else
            {
                customerAgrrGrouped = customerAgrrGrouped.OrderBy(x => x.Sum(a => a.Count));
            }

            var agregation = customerAgrrGrouped.FirstOrDefault().Key;

            int cateringId = _tagManager.UnTag<int>(agregation,AgregationConstants.CateringId);

            var catering = _cateringManager.Get(cateringId);

            return new GetCustomerOfferResponse
            { 
                CateringName = catering.Name,
                AddressText  = catering.AddressText,
                Latitude = catering.Latitude.Value,
                Longitude = catering.Latitude.Value
            };
        }
    }
}
