using System;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Entities.Enums;
using CDS.APICore.Helpers;
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
                        var culastdate = this.getDateVal(_paramKeyCustomer);
                        _agregationManager.AgregateCustomers(PeriodType.Daily, culastdate);
                        this.setValue(_paramKeyCustomer, _timeManager.Now);
                        break;
                    case AgregationBy.Catering:
                        var calastdate = this.getDateVal(_paramKeyCatering);
                        _agregationManager.AgregateCaterings(PeriodType.Daily, calastdate);
                        this.setValue(_paramKeyCatering, _timeManager.Now);
                        break;
                    case AgregationBy.CateringCategory:
                        break;
                    case AgregationBy.CustomerCatering:
                        var cclastdate = this.getDateVal(_paramKeyCustomerCatering);
                        _agregationManager.AgregateCateringCustomers(PeriodType.Daily, cclastdate);
                        this.setValue(_paramKeyCustomerCatering, _timeManager.Now);
                        break;
                }

                return Task.CompletedTask;
            }

            throw new InvalidOperationException("Job trigger must have a agregation type");
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
