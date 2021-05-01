using System.ComponentModel.DataAnnotations;

namespace CDS.APICore.Models.Catering
{
    public class AddCategoryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public string DisplayName { get; set; }
    }
}
