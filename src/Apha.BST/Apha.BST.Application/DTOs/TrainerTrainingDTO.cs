using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class TrainerTrainingDto
    {
        public int PersonID { get; set; }
        public string? Person { get; set; }
        public string? TrainingAnimal { get; set; }
        public string? TrainingType { get; set; }
        public string? Name { get; set; } // Site Name
        public DateTime TrainingDateTime { get; set; }
        public int TraineeId { get; set; }
    }
}
