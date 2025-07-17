using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Apha.BST.Core.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAuditLogAsync(string procedure, SqlParameter[] parameters, string transactionType, string error = null);
    }
}
