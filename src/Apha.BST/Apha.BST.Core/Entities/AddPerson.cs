using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Core.Entities
{
    public class AddPerson
    {
        public string? Name { get; set; }

        public string? LocationId { get; set; }

        //public int? VlalocationId { get; set; }
        [Required(ErrorMessage = "Plant / Site No. is required")]
        [Key]
        public string PlantNo { get; set; } = null!;
    }
}
