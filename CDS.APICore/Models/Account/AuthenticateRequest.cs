using System.ComponentModel.DataAnnotations;

namespace CDS.APICore.Models.Account
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
