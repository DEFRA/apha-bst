using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class EditUserViewModel:BaseViewModel
    {
        public string UserId { get; set; } = null!;

        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a user location.")]
        public string UserLoc { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "User Level is required")]
        public byte UserLevel { get; set; } = 0;

        public IEnumerable<SelectListItem> Locations { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> UserLevels { get; set; } = new List<SelectListItem>();
    }
}
