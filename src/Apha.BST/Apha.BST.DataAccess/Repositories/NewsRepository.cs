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
    public class NewsRepository : RepositoryBase<News>,INewsRepository
    {
        public NewsRepository(BstContext context) : base(context) { }       

        public async Task<IEnumerable<News>> GetLatestNewsAsync()
        {
            // Use the base GetQueryableResult method
            return await GetQueryableResult("EXEC sp_GetNews").ToListAsync();
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

            await ExecuteSqlAsync(
                "EXEC sp_News_Add @Title, @NewsContent, @DatePublished, @Author", parameters);
        }

        public async Task<List<News>> GetNewsAsync()
        {
            // Use the base GetQueryableResult method to execute the stored procedure
            return await GetQueryableResult("EXEC sp_GetAllNews").ToListAsync();
        }

        public async Task DeleteNewsAsync(string title)
        {
            var param = new SqlParameter("@Title", SqlDbType.NVarChar, 255)
            {
                Value = title
            };
            await ExecuteSqlAsync("EXEC sp_News_Delete @Title", param);
        }
    }
}
