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
    public class DataEntryRepository : RepositoryBase<DataEntry>, IDataEntryRepository
    {
        public DataEntryRepository(BstContext context): base(context) { }

        public virtual async Task<bool> CanEditPage(string action)
        {
                string? CanWrite = await GetDbSetFor<DataEntry>()
                .Where(p => p.ActiveViewName == action)
                    .Select(p => p.CanWrite)
                    .FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(CanWrite))
                {
                    if (CanWrite == "S")
                        return false; // User can not edit the page                
                    else
                        return true; // User can edit the page               
                }
                else
                {
                    return false;
                }            
        }
    }
}
