using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class TrainerReportDto
    {
        public string ID { get; set; } = null!;
        public string Trainer { get; set; } = null!;
        public int Trained { get; set; }
        public int RunTot { get; set; }
        public int Excel { get; set; }
    }
}
