using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly BstContext _context;
        public NewsRepository(BstContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<News>> GetLatestNewsAsync()
        {
            return await _context.News
                 .FromSqlRaw("EXEC sp_GetNews")
                 .ToListAsync();
        }
    }
}
