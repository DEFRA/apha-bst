using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess.Repositories;

namespace Apha.BST.DataAccess.UnitTests.SiteRepositoryTest
{
    public class TestSiteRepository : SiteRepository
    {
        private readonly IQueryable<SiteTrainee> _trainees;

        public TestSiteRepository(BstContext context, IAuditLogRepository auditLogRepo, IQueryable<SiteTrainee> trainees)
            : base(context, auditLogRepo)
        {
            _trainees = trainees;
        }

        protected override IQueryable<T> GetQueryableResultFor<T>(string sql, params object[] parameters)
        {
            if (typeof(T) == typeof(SiteTrainee))
                return (IQueryable<T>)_trainees;
            throw new NotImplementedException($"No override for type {typeof(T).Name}");
        }
    }
}
