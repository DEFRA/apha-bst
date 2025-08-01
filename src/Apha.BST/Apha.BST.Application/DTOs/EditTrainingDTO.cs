﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class EditTrainingDto
    {
        public int TraineeId { get; set; }
        public int TrainerId { get; set; }
        public required string TrainingType { get; set; }
        public required string TrainingAnimal { get; set; }
        public DateTime TrainingDateTime { get; set; }

        // Old values for identifying the record to update
        public int TraineeIdOld { get; set; }
        public int TrainerIdOld { get; set; }
        public required string TrainingAnimalOld { get; set; }
        public DateTime TrainingDateTimeOld { get; set; }
    }
}
