using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class Trainees
{
    public int TraineeNo { get; set; }

    public string? Trainee { get; set; }

    public string? PlantNo { get; set; }

    public bool Trained { get; set; }

    public virtual TblSite? PlantNoNavigation { get; set; }

    public virtual ICollection<Trainings> TblTrainings { get; set; } = new List<Trainings>();
}
