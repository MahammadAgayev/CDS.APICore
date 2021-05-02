
using CDS.APICore.Helpers;
using CDS.APICore.Models.Location;
using CDS.APICore.Services;

using Microsoft.AspNetCore.Mvc;

namespace CDS.APICore.Controllers
{
    [ApiController]
    [Authorize]
    public class LocationController : BaseController
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost("addlocation")]
        public IActionResult AddLocation(AddLocationRequest request)
        {
            _locationService.AddLocation(request, Account);

            return Created();
        }
    }
}
