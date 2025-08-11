using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Core.Entities
{
    public class PersonLookup
    {
        [Key]
        public int PersonID { get; set; }
        public string? Person { get; set; }
    }
}