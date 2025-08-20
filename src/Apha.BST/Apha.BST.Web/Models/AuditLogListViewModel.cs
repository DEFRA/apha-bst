using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    public class AuditLogListViewModel
    {      
         
        public IEnumerable<SelectListItem> StoredProcedures { get; set; } = new List<SelectListItem>();
        public List<AuditLogViewModel>? AuditLogsResults { get; set; }
        public PaginationModel? Pagination { get; set; }
        public string StoredProcedure { get; set; } = "%";
        public bool SortOrderFor(string column)
        {
            if (Pagination != null)
                return !(Pagination.SortColumn == column && Pagination.SortDirection);
            else
                return true;
        }

    }
}
