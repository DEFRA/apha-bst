using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess.Repositories;
using Apha.BST.DataAccess.UnitTests.Helpers;

namespace Apha.BST.DataAccess.UnitTests.AccessControlRepositoryTest
{
    public class AbstractAccessControlRepositoryTest : AccessControlRepository
    {
        private readonly IQueryable<UserRoleInfo>? _userRoleInfos;

        public AbstractAccessControlRepositoryTest(
            BstContext context,
            IQueryable<UserRoleInfo>? userRoleInfos = null)
            : base(context)
        {
            _userRoleInfos = userRoleInfos;
        }

        protected override IQueryable<T> GetQueryableResultFor<T>(string sql, params object[] parameters)
        {
            if (typeof(T) == typeof(UserRoleInfo) && _userRoleInfos != null)
                return new TestAsyncEnumerable<UserRoleInfo>(_userRoleInfos).Cast<T>();
            throw new System.NotImplementedException($"No override for type {typeof(T).Name}");
        }
    }
}
