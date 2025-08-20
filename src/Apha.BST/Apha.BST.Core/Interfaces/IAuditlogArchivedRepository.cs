using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Pagination;

namespace Apha.BST.Core.Interfaces
{
    public interface IAuditlogArchivedRepository
    {
        Task<PagedData<AuditlogArchived>> GetArchiveAuditLogsAsync(PaginationParameters filter, string storedProcedure);
        Task<List<string>> GetStoredProcedureNamesAsync();
    }
}
