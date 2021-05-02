using CDS.APICore.Models.Account;
using CDS.APICore.Services;

using Microsoft.AspNetCore.Mvc;

namespace CDS.APICore.Controllers
{
    [ApiController]
    public class AccountController : BaseController
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest request)
        {
            var response = _accountService.Authenticate(request);

            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            _accountService.Register(request);

            return Created();
        }
    }
}
