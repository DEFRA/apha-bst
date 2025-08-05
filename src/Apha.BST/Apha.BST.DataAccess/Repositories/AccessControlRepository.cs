using System.Collections.Generic;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class AccessControlRepository : IAccessControlRepository
    {
        private readonly BstContext _context;
        public AccessControlRepository(BstContext context)
        {
            _context = context;

        }
        public async Task<(byte? RoleId, string? Username)?> GetRoleIdAndUsernameByEmailAsync(string email)
        {
            var param = new SqlParameter("@UserID", email);
            var result = await _context.UserRoleInfos
                .FromSqlRaw("EXEC sp_User_Get @UserID", param)
                .AsNoTracking()
               .ToListAsync();

            if (result.Count > 0)
            {
                var item = result[0];
                if (item == null)
                {
                    return null;
                }
                return (item.UserLevel, item.UserName);
            }
            return null;



        }

    }
}
