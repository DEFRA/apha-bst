using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class RepositoryBase<TEntity> where TEntity : class
    {
        protected readonly BstContext _context;

        public RepositoryBase(BstContext context)
        {
            _context = context;
        }

        protected virtual IQueryable<TEntity> GetQueryableResult(string sql, params object[] parameters)
        {
            return _context.Set<TEntity>().FromSqlRaw(sql, parameters);
        }

        protected virtual Task<int> ExecuteSqlAsync(string sql, params object[] parameters)
        {
            return _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }
}
