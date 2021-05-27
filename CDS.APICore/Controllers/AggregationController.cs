using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDS.APICore.Models.Agregation;
using CDS.APICore.Services;
using Microsoft.AspNetCore.Mvc;

namespace CDS.APICore.Controllers
{
    public class AggregationController : BaseController
    {
        private readonly IAggregationService _aggregationService;

        public AggregationController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [HttpGet("getaggregations")]
        public IActionResult GetAggregations([FromQuery]GetAggregationDataRequest request)
        {
            return Ok(_aggregationService.GetAggregationData(request));
        }
    }
}
