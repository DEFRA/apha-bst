﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class TrainingDto
    {
        [Required]
        public int PersonId { get; set; }
        [Required]
        public string TrainingAnimal { get; set; } = null!;
        [Required]
        public DateTime TrainingDateTime { get; set; }
        [Required]
        public string TrainingType { get; set; } = null!;
        [Required]
        public int TrainerId { get; set; }
    }
}
