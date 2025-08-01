﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Core.Entities;

public partial class Site
{
    [Key]
    public string PlantNo { get; set; } = null!;

    public string? Name { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? AddressTown { get; set; }

    public string? AddressCounty { get; set; }

    public string? AddressPostCode { get; set; }

    public string? Telephone { get; set; }

    public string? Fax { get; set; }

    public string? Ahvla { get; set; }    
}
