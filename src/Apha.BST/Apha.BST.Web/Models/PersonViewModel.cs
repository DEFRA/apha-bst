using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class PersonViewModel
    {
        [Required(ErrorMessage = "Id is required")]
        public int PersonId { get; set; } 
        public string? Person { get; set; }
        public string? LocationId { get; set; }
        public int? VlalocationId { get; set; }
        public string? Name { get; set; }
        public string? PlantNo { get; set; }

    }
}
