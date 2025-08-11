using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Core.Entities
{
    public class PersonSiteLookup
    {
        [Key]
        public string PlantNo { get; set; } = null!;
        public required string Name { get; set; }
    }
}
