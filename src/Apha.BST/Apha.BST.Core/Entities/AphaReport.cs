using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.Core.Entities
{    
    public class AphaReport
    {
        public string? ID { get; set; }
        public string? Location { get; set; }
        public string? APHA { get; set; } 
    }
}
