using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class EditPersonDto
    {
        public string? Name { get; set; }
        public int PersonID { get; set; }
       
        public string Person { get; set; }=null!;
    }
}
