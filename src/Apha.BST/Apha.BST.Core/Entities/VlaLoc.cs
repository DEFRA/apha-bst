﻿using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class VlaLoc
{
    public string LocId { get; set; } = null!;

    public string? VlaLocation { get; set; }

    public string? Ahvla { get; set; }

    public virtual ICollection<Trainers> TblTrainers { get; set; } = new List<Trainers>();
}
