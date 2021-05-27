using System;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Helpers;
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
            return Task.CompletedTask;

            var lastdate = this.getDateVal(_paramKey);

            _agregationManager.AggregateLocations(Entities.Enums.PeriodType.Hourly, lastdate);

            this.setValue(_paramKey, _timeManager.Now);

            return Task.CompletedTask;
        }


        private DateTime getDateVal(string key)
        {
            string val = _paramManager.GetValue(key);

            return _timeManager.Parse(val, SystemDefaults.DefaultFormat);
        }

        private void setValue(string key, DateTime value)
        {
            _paramManager.SetValue(key, value.ToString(SystemDefaults.DefaultFormat));
        }
    }
}
