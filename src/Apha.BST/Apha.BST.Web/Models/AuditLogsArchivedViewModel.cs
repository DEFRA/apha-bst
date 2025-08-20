namespace Apha.BST.Web.Models
{
    public class AuditLogsArchivedViewModel
    {
        public string? Procedure { get; set; }

        public string? Parameters { get; set; }

        public string? User { get; set; }

        public DateTime? Date { get; set; }

        public string? TransactionType { get; set; }
    }
}
