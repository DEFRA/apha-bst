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
        public string ID { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string APHA { get; set; } = null!;
    }
}
