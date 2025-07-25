using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;

namespace Apha.BST.Core
{
    public class AddTrainingResult
    {
        public Training Training { get; set; } = null!;
        public byte ReturnCode { get; set; }
    }
}
