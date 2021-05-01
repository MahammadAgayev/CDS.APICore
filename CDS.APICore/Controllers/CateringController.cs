
using CDS.APICore.Helpers;
using CDS.APICore.Models.Catering;
using CDS.APICore.Services;

using Microsoft.AspNetCore.Mvc;

namespace CDS.APICore.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class CateringController : BaseController
    {
        private readonly ICateringService _cateringService;

        public CateringController(ICateringService cateringService)
        {
            _cateringService = cateringService;
        }

        [HttpPost("addcategory")]
        public IActionResult AddCategory(AddCategoryRequest request)
        {
            _cateringService.AddCategory(request);

            return Created();
        }

        [HttpPost("addcatering")]
        public IActionResult AddCatering(AddCateringRequest request)
        {
            _cateringService.AddCatering(request);

            return Created();
        }
    }
}
