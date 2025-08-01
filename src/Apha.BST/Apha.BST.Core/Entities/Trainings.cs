﻿using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class Trainings
{
    public int TraineeNo { get; set; }

    public int? TrainerId { get; set; }

    public string DateTrained { get; set; } = null!;

    public string SpeciesTrained { get; set; } = null!;

    public virtual Trainees TraineeNoNavigation { get; set; } = null!;

    public virtual Trainers? Trainer { get; set; }
}
