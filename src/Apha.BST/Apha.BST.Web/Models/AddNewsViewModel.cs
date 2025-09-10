using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class AddNewsViewModel:BaseViewModel
    {
        [Required(ErrorMessage = "Complete news title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Complete news content")]
        public string NewsContent { get; set; } = string.Empty;

        [Required(ErrorMessage = "Complete date and time")]
        [Display(Name = "News date and time")]
        public string? DatePublished { get; set; }

        [Display(Name = "Use current date and time")]
        public bool UseCurrentDateTime { get; set; } = false;

        [Required(ErrorMessage = "Select News Person")]
        public string Author { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
        public DateTime CurrentDateTime { get; set; } = DateTime.Now;
    }
}
