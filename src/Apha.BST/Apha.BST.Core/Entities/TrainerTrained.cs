using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Core.Entities
{
    public class TrainerTrained
    {
        public int No { get; set; }
        public string Trainee { get; set; }
        public string Site { get; set; }
        public string Species { get; set; }
        public DateTime DateTrained { get; set; }
    }
}
