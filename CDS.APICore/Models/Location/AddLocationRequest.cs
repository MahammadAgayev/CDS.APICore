using System.ComponentModel.DataAnnotations;

namespace CDS.APICore.Models.Location
{
    public class AddLocationRequest
    {
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public string CustomerIdentityTag { get; set; }
    }
}