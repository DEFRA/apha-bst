using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class EditPersonViewModel:BaseViewModel
    {
        

        public string? Name { get; set; }
        public required int PersonId { get; set; }

        [Required(ErrorMessage = "Please enter person name")]
        public string Person { get; set; } = null!;
      
        public IEnumerable<SelectListItem> SiteOptions { get; set; } = new List<SelectListItem>();
    }
}
