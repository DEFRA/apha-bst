using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class SiteReportDto
    {
        public string PlantNo { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Add1 { get; set; } = null!;
        public string Add2 { get; set; } = null!;
        public string Town { get; set; } = null!;
        public string County { get; set; } = null!;
        public string Postcode { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public int People { get; set; }
        public int RunTot { get; set; }
        public int Excel { get; set; }
    }
}
