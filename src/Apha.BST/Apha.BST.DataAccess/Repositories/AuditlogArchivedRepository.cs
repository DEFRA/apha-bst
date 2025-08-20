using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.Core.Pagination;
using Apha.BST.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class AuditlogArchivedRepository:IAuditlogArchivedRepository
    {      
       
        private readonly BstContext _context;
        public AuditlogArchivedRepository(BstContext context)
        {
            _context = context;
        }
        public async Task<PagedData<AuditlogArchived>> GetArchiveAuditLogsAsync(PaginationParameters filter, string storedProcedure)
        {
            var query = _context.AuditlogArchiveds.AsQueryable();
            query = query.Where(i => i.Procedure == null
                            || (!i.Procedure.StartsWith("sp_GetAll") && !i.Procedure.StartsWith("sp_Audit_log_Archive")));
            if (!string.IsNullOrEmpty(filter.Search) && filter.Search != "%")
            {
                query = query.Where(i =>
                i.Procedure == filter.Search);
            }

            query = (IQueryable<AuditlogArchived>)ApplySorting(query, filter.SortBy, filter.Descending);
            var totalRecords = await query.CountAsync();
            var auditResult = await query.Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            return new PagedData<AuditlogArchived>(auditResult, totalRecords);

        }
        public async Task<List<string>> GetStoredProcedureNamesAsync()
        {

            var result = await _context.Set<StoredProcedureList>()
                .FromSqlRaw("EXEC sp_AuditArchive_log_SPList")
                .ToListAsync();

            return result?
            .Where(r => r?.Name != null)
            .Select(r => r!.Name!)
            .ToList() ?? new List<string>();

        }
        private static IQueryable ApplySortingByProperty(IQueryable<AuditlogArchived> query, string property, bool descending)
        {
            return property switch
            {
                "user" => ApplyOrder(query, i => i.User, descending),
                "transactiontype" => ApplyOrder(query, i => i.TransactionType, descending),
                "procedure" => ApplyOrder(query, i => i.Procedure, descending),
                "parameters" => ApplyOrder(query, i => i.Parameters, descending),
                "date" => ApplyOrder(query, i => i.Date, descending),

                _ => query
            };
        }
        private static IQueryable ApplyOrder<T>(IQueryable<AuditlogArchived> query, Expression<Func<AuditlogArchived, T>> keySelector, bool descending)
        {
            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }
        private static IQueryable ApplySorting(IQueryable<AuditlogArchived> query, string? sortBy, bool descending)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return query;
            }

            return ApplySortingByProperty(query, sortBy.ToLower(), descending);
        }
    }
}
