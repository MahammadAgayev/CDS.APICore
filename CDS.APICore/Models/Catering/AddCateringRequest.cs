using System.ComponentModel.DataAnnotations;

namespace CDS.APICore.Models.Catering
{
    public class AddCateringRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string Comment { get; set; }
        [Required]
        public string AddressText { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
    }
}
