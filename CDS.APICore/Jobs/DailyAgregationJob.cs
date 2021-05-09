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
        private const string _paramKey = "dailyagrcheckpoint";

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
            if(context.Trigger.JobDataMap.TryGetValue("AgregationType", out var agrrBy))
            {
                if(agrrBy is null)
                {
                    throw new InvalidOperationException("Job trigger agregation type cannot be null");
                }

                var lastdate = _paramManager.GetValue<DateTime>(_paramKey);

                AgregationBy agregation = (AgregationBy)agrrBy;

                switch (agregation)
                {
                    case AgregationBy.Customer:
                        _agregationManager.AgregateCustomers(PeriodType.Daily, lastdate);
                        break;
                    case AgregationBy.Catering:
                        _agregationManager.AgregateCaterings(PeriodType.Daily, lastdate);
                        break;
                    case AgregationBy.CateringCategory:
                        break;
                    case AgregationBy.CustomerCatering:
                        _agregationManager.AgregateCateringCustomers(PeriodType.Daily, lastdate);
                        break;
                }

                _paramManager.SetValue(_paramKey, lastdate.AddDays(1));

                return Task.CompletedTask;
            }

            throw new InvalidOperationException("Job trigger must have a agregation type");
        }
    }
}
