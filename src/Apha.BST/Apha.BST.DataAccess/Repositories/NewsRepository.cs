using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.Data.SqlClient;
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
        public async Task AddNewsAsync(News news)
        {
            var parameters = new[]
            {
            new SqlParameter("@Title", news.Title),
            new SqlParameter("@NewsContent", news.NewsContent ?? (object)DBNull.Value),
            new SqlParameter("@DatePublished", news.DatePublished),
            new SqlParameter("@Author", news.Author ?? (object)DBNull.Value)
        };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_News_Add @Title, @NewsContent, @DatePublished, @Author", parameters);
        }

        public async Task<List<News>> GetNewsAsync()
        {
            return await _context.News
                .FromSqlRaw("EXEC sp_GetAllNews")
                .ToListAsync();
        }

        public async Task DeleteNewsAsync(string title)
        {
            var param = new SqlParameter("@Title", SqlDbType.NVarChar, 255)
            {
                Value = title
            };
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_News_Delete @Title", param);
        }
    }
}
