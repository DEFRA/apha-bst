using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Repositories;
using Microsoft.Data.SqlClient;

namespace Apha.BST.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task WriteAuditLogAsync(string procedure, SqlParameter[] parameters, string transactionType, string error = null)
        {
            await _auditLogRepository.AddAuditLogAsync(procedure, parameters, transactionType, error);
        }
    }
}
