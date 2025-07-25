using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Core.Entities
{
    public class EditTraining
    {
        public int PersonId { get; set; }
        public int TrainerId { get; set; }
        public string TrainingType { get; set; } = null!;
        public string TrainingAnimal { get; set; } = null!;
        public DateTime TrainingDateTime { get; set; }

        // Old values for identifying the record to update
        public int TrainerIdOld { get; set; }
        public string TrainingAnimalOld { get; set; } = null!;
        public DateTime TrainingDateTimeOld { get; set; }
    }
}
