using System;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;

using Quartz;

namespace CDS.APICore.Jobs
{
    [DisallowConcurrentExecution]
    public class DailyLocationAgregationJob : IJob
    {
        private const string _paramKey = "dailyaggrlocationcheckpoint";

        private readonly IAgregationManager _agregationManager;
        private readonly ITimeManager _timeManager;
        private readonly IParamManager _paramManager;

        public DailyLocationAgregationJob(IAgregationManager agregationManager, ITimeManager timeManager, IParamManager paramManager)
        {
            _agregationManager = agregationManager;
            _timeManager = timeManager;
            _paramManager = paramManager;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var lastdate = _paramManager.GetValue<DateTime>(_paramKey);

            _agregationManager.AggregateLocations(Entities.Enums.PeriodType.Hourly, lastdate);

            _paramManager.SetValue(_paramKey, lastdate.AddDays(1));

            return Task.CompletedTask;
        }
    }
}
