using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class SiteDto
    {
        [Required(ErrorMessage = "Plant / Site No. is required")]
        public string PlantNo { get; set; } = null!;

        [Required(ErrorMessage = "Plant Name is required")]
        public string? Name { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? AddressTown { get; set; }

        public string? AddressCounty { get; set; }

        public string? AddressPostCode { get; set; }

        public string? Telephone { get; set; }

        public string? Fax { get; set; }

        public string? Ahvla { get; set; }       
    }
}
