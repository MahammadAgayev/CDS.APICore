using System;

using CDS.APICore.Entities.Enums;

namespace CDS.APICore.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IdentityTag { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public CommunicationType CommunicationType { get; set; } 
    }
}
