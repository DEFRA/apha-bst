using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly BstContext _context;
        public AuditLogRepository(BstContext context)
        {
            _context = context;
        }

        public async Task AddAuditLogAsync(string procedure, SqlParameter[] parameters, string transactionType,string userName, string? error = null)
        {
            var paramString = new StringBuilder();
            if (!string.IsNullOrEmpty(error))
            {
                paramString.Append("Error occured in SP: " + error + "\r\n");
            }

            foreach (var param in parameters)
            {
                if (param != null)
                {
                    paramString.Append(param.ParameterName + ":");
                    if (param.Value != null && param.Value != DBNull.Value)
                        paramString.Append(param.Value.ToString());
                    paramString.Append(";");
                }
            }

            var auditParams = new[]
            {
                 new SqlParameter("@Procedure", SqlDbType.VarChar, 100) { Value = procedure },
                 new SqlParameter("@Parameters", SqlDbType.Text) { Value = paramString.ToString() },
                 new SqlParameter("@User", SqlDbType.VarChar, 50) { Value =userName},
                 new SqlParameter("@TransactionType", SqlDbType.VarChar, 10) { Value = transactionType }
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_Audit_Log @Procedure, @Parameters, @User, @TransactionType", auditParams);
        }
    }
}
