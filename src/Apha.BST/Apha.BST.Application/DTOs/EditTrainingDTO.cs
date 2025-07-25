using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class EditTrainingDTO
    {       
        public int PersonId { get; set; }
        public string TrainingAnimal { get; set; }
        public DateTime TrainingDateTime { get; set; }

        // Old values to find the correct record to update
        public string TrainingAnimalOld { get; set; }
        public DateTime TrainingDateTimeOld { get; set; }

        // Optional fields depending on your DB/table
        public string Site { get; set; }
        public int TrainerId { get; set; }
        public string TrainingType { get; set; }
    }
}
