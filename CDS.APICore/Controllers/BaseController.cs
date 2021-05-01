
using System.Net;

using CDS.APICore.Entities;

using Microsoft.AspNetCore.Mvc;

namespace CDS.APICore.Controllers
{
    [Controller]
    public class BaseController : ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public Account Account => (Account)HttpContext.Items["Account"];

        protected virtual IActionResult Created()
        {
            return StatusCode((int)HttpStatusCode.Created);
        }
    }
}