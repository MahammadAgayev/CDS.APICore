
using CDS.APICore.Helpers;
using CDS.APICore.Models.Offer;
using CDS.APICore.Services;

using Microsoft.AspNetCore.Mvc;

namespace CDS.APICore.Controllers
{
    [Authorize]
    public class OfferController : BaseController
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet("getcustomeroffer")]
        public IActionResult GetCustomerOffer([FromQuery] GetCustomerOfferRequest request)
        {
            var resp = _offerService.GetCustomerOffer(request);

            return Ok(resp);
        }
    }
}
