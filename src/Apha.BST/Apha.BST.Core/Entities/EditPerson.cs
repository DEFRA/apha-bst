using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Core.Entities
{
    public class EditPerson
    {
        public string? Name { get; set; }
        [Key]
        public int PersonID { get; set; }
       
        public string Person { get; set; } = null!;
    }
}
