﻿using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class SiteViewModel
    {
        [Required(ErrorMessage = "Plant / Site No. is required")]
        public string PlantNo { get; set; } = null!;

        [Required(ErrorMessage = "Plant Name is required")]
        public required string Name { get; set; }

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
