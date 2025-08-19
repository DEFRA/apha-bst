using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class SiteTraineeDto
    {
        public int PersonId { get; set; }
        public string? Person { get; set; }
        public bool HasTraining { get; internal set; }
    }
}
