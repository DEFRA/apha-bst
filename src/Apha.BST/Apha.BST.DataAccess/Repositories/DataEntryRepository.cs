using System;
using System.Collections.Generic;
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
    public class DataEntryRepository : IDataEntryRepository
    {
        private readonly BstContext _context;

        public DataEntryRepository(BstContext context)
        {
            _context = context;
           
        }

        public async Task<bool> CanEditPage(string action)
        {
            var actionnameParam = new SqlParameter("@ActiveView", action);

            var result = await _context.Database
                .SqlQueryRaw<string>("EXEC sp_DataEntry_Get @ActiveView", actionnameParam)
                .ToListAsync();
            if(result == null || !result.Any())
            {
                return false; // No result found
            }
            else
            {
                // the result is a single string indicating the status
                return result.FirstOrDefault() == "D"; // Return true if status is 'D', false otherwise
            }           

        }
    }
}
