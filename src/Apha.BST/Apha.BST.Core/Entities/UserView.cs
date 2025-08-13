using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Core.Entities
{
    public class UserView
    {
        public string UserId { get; set; } = null!;
        public string? UserName { get; set; }

        [Column("Loc_ID")]
        public string? UserLoc { get; set; }

        [Column("VLA_Location")]
        public string? VlaLocation { get; set; }
        public byte? UserLevel { get; set; }
        public string? UserLevelName { get; set; }
    }
}
