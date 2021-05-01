using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDS.APICore.Models.Account
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
