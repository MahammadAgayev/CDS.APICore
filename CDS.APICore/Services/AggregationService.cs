using System;
using System.Linq;
using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Constants;
using CDS.APICore.Entities;
using CDS.APICore.Models.Agregation;

namespace CDS.APICore.Services
{
    public interface IAggregationService
    {
        object GetAggregationData(GetAggregationDataRequest request);
    }

    public class AggregationService : IAggregationService
    {
        private readonly IAgregationManager _agregationManager;
        private readonly ITimeManager _timeManager;
        private readonly ITagManager _tagManager;
        private readonly ICateringManager _cateringManager;
        private readonly ICustomerManager _customerManager;

        public AggregationService(IAgregationManager agregationManager, ITimeManager timeManager, ITagManager tagManager, ICateringManager cateringManager, ICustomerManager customerManager)
        {
            _agregationManager = agregationManager;
            _timeManager = timeManager;
            _tagManager = tagManager;
            _cateringManager = cateringManager;
            _customerManager = customerManager;
        }

        public object GetAggregationData(GetAggregationDataRequest request)
        {
            var resp = _agregationManager.GetAgregations(Entities.Enums.PeriodType.Daily, this.getFromDate(request.TimeRangeType), request.AgregationBy);

            switch (request.AgregationBy)
            {
                case Entities.Enums.AgregationBy.Customer:
                    return new GetCustomerAggregationDataResponse
                    {
                        AggregationCustomerDatas = resp.Select(x => new AggregationCustomerDataItem
                        {
                            AggregationDataItem = this.convert(x),
                            AggregationCustomerData = this.convert(_customerManager.Get(_tagManager.UnTag<int>(x.Tag, AgregationConstants.CustomerId)))
                        }).ToList()
                    };
                case Entities.Enums.AgregationBy.Catering:
                    return new GetCateringAggregationDataResponse
                    {
                        AggregationCateringDataItems = resp.Select(x => new AggregationCateringDataItem
                        {
                            AggregationDataItem = this.convert(x),
                            AggregationCateringData = this.convert(_cateringManager.Get(_tagManager.UnTag<int>(x.Tag, AgregationConstants.CateringId)))
                        }).ToList()
                    };
                case Entities.Enums.AgregationBy.CustomerCatering:
                    return new GetCustomerCateringAggregationDataResponse
                    {
                        AggregationCustomerAndCateringDatas = resp.Select(x => new AggregationCustomerAndCateringDataItem
                        {
                            AggregationDataItem = this.convert(x),
                            AggregationCateringData = this.convert(_cateringManager.Get(_tagManager.UnTag<int>(x.Tag, AgregationConstants.CateringId))),
                            AggregationCustomerData = this.convert(_customerManager.Get(_tagManager.UnTag<int>(x.Tag, AgregationConstants.CustomerId)))
                        }).ToList()
                    };
            }

            return null;
        }

        private DateTime getFromDate(AggregationRequestTimeRangeType timeRangeType)
        {
            switch (timeRangeType)
            {
                case AggregationRequestTimeRangeType.LastDay:
                    return _timeManager.Now.AddDays(-1);
                case AggregationRequestTimeRangeType.LastWeek:
                    return _timeManager.Now.AddDays(-7);
                case AggregationRequestTimeRangeType.LastMonth:
                    return _timeManager.Now.AddMonths(-1);
                case AggregationRequestTimeRangeType.LastYear:
                    return _timeManager.Now.AddYears(-1);

                default:
                    return _timeManager.Now.AddDays(-1);
            }
        }

        private AggregationDataItem convert(Agregation x)
        {
            return new AggregationDataItem
            {
                Sum = x.Sum,
                PeriodStart = x.PeriodStart,
                PeriodType = x.PeriodType.ToString(),
                AgregationBy = x.AgregationBy.ToString(),
                Average = x.Average,
                Count = x.Count,
                Created = x.Created,
                Max = x.Max,
                Min = x.Min
            };
        }

        private AggregationCustomerData convert(Customer x)
        {
            return new AggregationCustomerData
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Phone = x.Phone,
                IdentityTag = x.IdentityTag
            };
        }

        private AggregationCateringData convert(Catering x)
        {
            return new AggregationCateringData
            {
                Id = x.Id,
                Name = x.Name,
                AddressText = x.AddressText,
                CategoryComment = x.CateringCategory.Comment,
                CategoryName = x.CateringCategory.Name,
                CategoryDisplayName = x.CateringCategory.DisplayName,
                Comment = x.Comment,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            };
        }
    }
}
