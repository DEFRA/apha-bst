﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.Core.Entities
{
    public class TrainerTrained
    {
        public int TraineeNo { get; set; }
        public string? Trainee { get; set; }
        public string? Site { get; set; }
        public string? SpeciesTrained { get; set; }
        public DateTime DateTrained { get; set; }
    }
}
