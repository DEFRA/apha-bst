using Apha.BST.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class AddUserViewModel:BaseViewModel
    {
        [Required(ErrorMessage = "Add User Email")]
        [CustomEmail]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Add new user name")]
        public string UserName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Add User Level")]
        public byte UserLevel { get; set; } = 0;

        [Required(ErrorMessage = "Add new user location.")]
        public string UserLoc { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> UserLevels { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Locations { get; set; } = new List<SelectListItem>();
    }
}
