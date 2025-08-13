using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class EditUserViewModel:BaseViewModel
    {
        public string UserId { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string UserLoc { get; set; } = string.Empty;

        [Required]
        public byte UserLevel { get; set; }

        public IEnumerable<SelectListItem> Locations { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> UserLevels { get; set; } = new List<SelectListItem>();
    }
}
