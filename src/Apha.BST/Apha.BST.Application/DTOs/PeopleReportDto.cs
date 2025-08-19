using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class PeopleReportDto
    {
        public string PersonId { get; set; } = null!;
        public string Person { get; set; } = null!;
        public string? LocationId { get; set; }              
        public string? AphaLocation { get; set; }             
        public string? Trainer { get; set; }                 
        public string? Trainee { get; set; }                  
        public string? Trained { get; set; }
    }
}
