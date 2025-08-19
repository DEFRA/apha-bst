using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.Core.Entities
{
    [Keyless]
    public class Report
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; } = null!;
        public DateTime GeneratedDate { get; set; }
        public string GeneratedBy { get; set; } = null!;
    }
}
