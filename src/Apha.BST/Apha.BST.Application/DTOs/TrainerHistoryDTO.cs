using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class TrainerHistoryDto
    {
        public int PersonID { get; set; }
        public string? Person { get; set; }
        public string? Role { get; set; }
        public int TrainerID { get; set; }
        public string? TrainingAnimal { get; set; }
        public DateTime TrainingDateTime { get; set; }
        public string? Trainer { get; set; }
    }
}
