using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.Core.Entities
{
    public class PeopleReport
    {
        public int PersonID { get; set; }
        public string Person { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string VLA { get; set; } = null!;
        public string Trainer { get; set; } = null!;
        public string Trainee { get; set; } = null!;
        public string Trained { get; set; } = null!;
    }
}
