using System;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Entities.Enums;

using Quartz;

namespace CDS.APICore.Jobs
{
    [DisallowConcurrentExecution]
    public class DailyAgregationJob : IJob
    {
        private const string _paramKeyCatering = "dailyaggrcateringcheckpoint";
        private const string _paramKeyCustomer = "dailyaggrcustomercheckpoint";
        private const string _paramKeyCustomerCatering = "dailyaggrcustomercateringcheckpoint";

        private readonly IAgregationManager _agregationManager;
        private readonly ITimeManager _timeManager;
        private readonly IParamManager _paramManager;

        public DailyAgregationJob(IAgregationManager agregationManager, ITimeManager timeManager, IParamManager paramManager)
        {
            _agregationManager = agregationManager;
            _timeManager = timeManager;
            _paramManager = paramManager;
        }

        public Task Execute(IJobExecutionContext context)
        {
            if (context.Trigger.JobDataMap.TryGetValue("AgregationType", out var agrrBy))
            {
                if (agrrBy is null)
                {
                    throw new InvalidOperationException("Job trigger agregation type cannot be null");
                }


                AgregationBy agregation = (AgregationBy)agrrBy;

                switch (agregation)
                {
                    case AgregationBy.Customer:
                        var culastdate = _paramManager.GetValue<DateTime>(_paramKeyCustomer);
                        _agregationManager.AgregateCustomers(PeriodType.Daily, culastdate);
                        _paramManager.SetValue(_paramKeyCustomer, culastdate.AddDays(1));
                        break;
                    case AgregationBy.Catering:
                        var calastdate = _paramManager.GetValue<DateTime>(_paramKeyCatering);
                        _agregationManager.AgregateCaterings(PeriodType.Daily, calastdate);
                        _paramManager.SetValue(_paramKeyCatering, calastdate.AddDays(1));
                        break;
                    case AgregationBy.CateringCategory:
                        break;
                    case AgregationBy.CustomerCatering:
                        var cclastdate = _paramManager.GetValue<DateTime>(_paramKeyCustomerCatering);
                        _agregationManager.AgregateCateringCustomers(PeriodType.Daily, cclastdate);
                        _paramManager.SetValue(_paramKeyCustomerCatering, cclastdate.AddDays(1));
                        break;
                }

                return Task.CompletedTask;
            }

            throw new InvalidOperationException("Job trigger must have a agregation type");
        }
    }
}
