using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.DTOs
{
    public class AuditLogArchivedDto
    {
        public string? Procedure { get; set; }

        public string? Parameters { get; set; }

        public string? User { get; set; }

        public DateTime? Date { get; set; }

        public string? TransactionType { get; set; }
    }
}
