using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class ReportDto
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; } = null!;
        public DateTime GeneratedDate { get; set; }
        public string GeneratedBy { get; set; } = null!;
    }
}
