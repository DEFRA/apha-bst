using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class Training
{
    public int PersonId { get; set; }

    public string TrainingAnimal { get; set; } = null!;

    public DateTime TrainingDateTime { get; set; }

    public string TrainingType { get; set; } = null!;

    public int TrainerId { get; set; }
}

