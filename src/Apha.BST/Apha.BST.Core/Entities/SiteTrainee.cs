﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Core.Entities
{
    public class SiteTrainee
    {
        public int PersonId { get; set; }
        public string? Person { get; set; }
        public bool Cattle { get; set; }
        public bool Sheep { get; set; }
        public bool Goats { get; set; }
    }
}
