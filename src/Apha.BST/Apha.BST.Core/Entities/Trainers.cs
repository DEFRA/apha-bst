using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class Trainers
{
    public int TrainerId { get; set; }

    public string? Trainer { get; set; }

    public string? LocId { get; set; }

    public int? PersonId { get; set; }

    public virtual VlaLoc? Loc { get; set; }

    public virtual ICollection<Trainings> TblTrainings { get; set; } = new List<Trainings>();
}
