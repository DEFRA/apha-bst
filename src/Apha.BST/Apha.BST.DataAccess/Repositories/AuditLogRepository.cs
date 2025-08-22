using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.Core.Pagination;
using Apha.BST.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class AuditLogRepository : RepositoryBase<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(BstContext context) : base(context) { }
       
        public virtual async Task AddAuditLogAsync(string procedure, SqlParameter[] parameters, string transactionType, string userName, string? error = null)
        {
            var paramString = new StringBuilder();
            if (!string.IsNullOrEmpty(error))
            {
                paramString.Append("Error occured in SP: " + error + "\r\n");
            }

            parameters.Where(param => param != null)
           .ToList()
           .ForEach(param =>
           {
               paramString.Append(param.ParameterName + ":");
               if (param.Value != null && param.Value != DBNull.Value)
                   paramString.Append(param.Value.ToString());
               paramString.Append(';');
           });

            var auditParams = new[]
            {
          new SqlParameter("@Procedure", SqlDbType.VarChar, 100) { Value = procedure },
          new SqlParameter("@Parameters", SqlDbType.Text) { Value = paramString.ToString() },
          new SqlParameter("@User", SqlDbType.VarChar, 50) { Value =userName},
          new SqlParameter("@TransactionType", SqlDbType.VarChar, 10) { Value = transactionType }
     };

            await ExecuteSqlAsync("EXEC sp_Audit_Log @Procedure, @Parameters, @User, @TransactionType", auditParams);
        }
        
        public async Task<PagedData<AuditLog>> GetAuditLogsAsync(PaginationParameters filter, string storedProcedure)
        {
            var query = GetDbSetFor<AuditLog>();

            query = query.Where(i => i.Procedure == null
                             || (!i.Procedure.StartsWith("sp_GetAll") && !i.Procedure.StartsWith("sp_Audit_log_Archive")));
            if (!string.IsNullOrEmpty(filter.Search) && filter.Search != "%")
            {
                query = query.Where(i =>
                i.Procedure == filter.Search);
            }

            query = (IQueryable<AuditLog>)ApplySorting(query, filter.SortBy, filter.Descending);
            var totalRecords = await query.CountAsync();
            var auditResult = await query.Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            return new PagedData<AuditLog>(auditResult, totalRecords);

        }
        public async Task<List<string>> GetStoredProcedureNamesAsync()
        {

            var result = await GetQueryableResultFor<StoredProcedureList>("EXEC sp_Audit_log_SPList")
                .ToListAsync();

            return result?
            .Where(r => r?.Name != null)
            .Select(r => r!.Name!)
            .ToList() ?? new List<string>();

        }
        public async Task ArchiveAuditLogAsync(string userName)
        {

            string? error = null;
            string storedProcedure = "sp_Audit_log_Archive";
            try
            {
                await ExecuteSqlAsync("EXEC " + storedProcedure);
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                throw;
            }
            finally
            {
                // Audit log for sp_Sites_Add, unless it's sp_Audit_log_DELETE or sp_Usage_Insert
                if (!string.Equals(storedProcedure, "sp_Audit_log_DELETE", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(storedProcedure, "sp_Usage_Insert", StringComparison.OrdinalIgnoreCase))
                {
                    await AddAuditLogAsync(
                        storedProcedure,
                       Array.Empty<SqlParameter>(),
                        "Write",
                        userName,
                        error
                    );
                }
            }
        }
        private static IQueryable ApplySortingByProperty(IQueryable<AuditLog> query, string property, bool descending)
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
        private static IQueryable ApplyOrder<T>(IQueryable<AuditLog> query, Expression<Func<AuditLog, T>> keySelector, bool descending)
        {
            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }
        private static IQueryable ApplySorting(IQueryable<AuditLog> query, string? sortBy, bool descending)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return query;
            }

            return ApplySortingByProperty(query, sortBy.ToLower(), descending);
        }
    }
}
