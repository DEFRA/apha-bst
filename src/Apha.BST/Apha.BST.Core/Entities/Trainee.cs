﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.Core.Entities
{
        [Keyless]
        public class Trainee
        {
            public int PersonId { get; set; }

            public string? Person { get; set; }
        }
}