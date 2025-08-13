using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class UserDto
    {
        public string UserId { get; set; } = null!;

        public string? UserName { get; set; }

        [Display(Name = "VLA Location")]
        public string? UserLoc { get; set; }

        [Display(Name = "User Level")]
        public byte? UserLevel { get; set; }
    }
}
