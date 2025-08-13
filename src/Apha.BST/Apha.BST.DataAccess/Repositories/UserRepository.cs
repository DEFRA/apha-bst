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
    public class UserRepository : IUserRepository
    {
        private readonly BstContext _context;

        public UserRepository(BstContext context)
        {
            _context = context;
        }
        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }

        public async Task AddUserAsync(User user)
        {
            var parameters = new[]
            {
         new SqlParameter("@UserID", user.UserId),
         new SqlParameter("@UserName", user.UserName ?? (object)DBNull.Value),
         new SqlParameter("@User_Loc", user.UserLoc ?? (object)DBNull.Value),
         new SqlParameter("@UserLevel", user.UserLevel ?? (object)DBNull.Value)
     };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_User_Add @UserID, @UserName, @User_Loc, @UserLevel", parameters);
        }

        public async Task<List<VlaLocView>> GetLocationsAsync()
        {
            return await _context.Set<VlaLocView>()
                .FromSqlRaw("EXEC sp_VLALocation_Select")
                .ToListAsync();
        }

        public async Task<List<UserView>> GetUsersAsync(string userId)
        {
            var param = new SqlParameter("@User", userId ?? "All users");
            return await _context.Set<UserView>()
                .FromSqlRaw("EXEC sp_User_SelectAll @User", param)
                .ToListAsync();
        }


        public async Task DeleteUserAsync(string userId)
        {
            var param = new SqlParameter("@UserID", userId);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_User_Delete @UserID", param);
        }

        public async Task UpdateUserAsync(User user)
        {
            var parameters = new[]
            {
        new SqlParameter("@UserID", user.UserId),
        new SqlParameter("@UserName", user.UserName ?? (object)DBNull.Value),
        new SqlParameter("@User_Loc", user.UserLoc ?? (object)DBNull.Value),
        new SqlParameter("@UserLevel", user.UserLevel ?? (object)DBNull.Value),
    };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_User_Update @UserID, @UserName, @User_Loc, @UserLevel", parameters);
        }


    }
}
