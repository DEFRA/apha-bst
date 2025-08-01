﻿using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class AuditlogArchived
{
    public string? Procedure { get; set; }

    public string? Parameters { get; set; }

    public string? User { get; set; }

    public DateTime? Date { get; set; }

    public string? TransactionType { get; set; }
}
