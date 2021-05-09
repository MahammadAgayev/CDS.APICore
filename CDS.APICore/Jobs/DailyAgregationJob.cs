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
        private readonly IAgregationManager _agregationManager;
        private readonly ITimeManager _timeManager;
        
        public DailyAgregationJob(IAgregationManager agregationManager, ITimeManager timeManager)
        {
            _agregationManager = agregationManager;
            _timeManager = timeManager;
        }

        public Task Execute(IJobExecutionContext context)
        {
            if(context.Trigger.JobDataMap.TryGetValue("AgregationType", out var agrrBy))
            {
                if(agrrBy is null)
                {
                    throw new InvalidOperationException("Job trigger agregation type cannot be null");
                }

                AgregationBy agregation = (AgregationBy)agrrBy;

                switch (agregation)
                {
                    case AgregationBy.Customer:
                        _agregationManager.AgregateCustomers(PeriodType.Daily, _timeManager.Now.AddDays(-10));
                        break;
                    case AgregationBy.Catering:
                        _agregationManager.AgregateCaterings(PeriodType.Daily, _timeManager.Now.AddDays(-10));
                        break;
                    case AgregationBy.CateringCategory:
                        break;
                    case AgregationBy.CustomerCatering:
                        _agregationManager.AgregateCateringCustomers(PeriodType.Daily, _timeManager.Now.AddDays(-10));
                        break;
                }
            }

            throw new InvalidOperationException("Job trigger must have a agregation type");
        }
    }
}
