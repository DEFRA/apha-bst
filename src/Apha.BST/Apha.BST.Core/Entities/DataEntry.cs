﻿using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class DataEntry
{
    public int ActiveView { get; set; }

    public string? ActiveViewName { get; set; }

    public string? CanWrite { get; set; }
}
