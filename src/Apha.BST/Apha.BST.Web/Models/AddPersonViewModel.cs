using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class AddPersonViewModel:BaseViewModel
    {
       
        [Required(ErrorMessage = "Please enter Person's Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Select a site to save person")]
        public string PlantNo { get; set; } = null!;
      
       
        public string? LocationId { get; set; }
        public IEnumerable<SelectListItem> Sites { get; set; } = new List<SelectListItem>();
    }
}
