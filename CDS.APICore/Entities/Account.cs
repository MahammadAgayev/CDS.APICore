using System;

namespace CDS.APICore.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
