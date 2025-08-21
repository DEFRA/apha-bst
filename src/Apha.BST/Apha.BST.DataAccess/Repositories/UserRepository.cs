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
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {   
        public UserRepository(BstContext context) : base(context) { }
        public async Task<bool> UserExistsAsync(string userId)
        {
            return await GetDbSetFor<User>().AnyAsync(u => u.UserId == userId);
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

            await ExecuteSqlAsync(
                "EXEC sp_User_Add @UserID, @UserName, @User_Loc, @UserLevel", parameters);
        }

        public async Task<List<VlaLocView>> GetLocationsAsync()
        {
            return await GetQueryableResultFor<VlaLocView>("EXEC sp_VLALocation_Select")
                .ToListAsync();
        }

        public async Task<List<UserView>> GetUsersAsync(string userId)
        {
            var param = new SqlParameter("@User", userId ?? "All users");
            return await GetQueryableResultFor<UserView>("EXEC sp_User_SelectAll @User", param)
                .ToListAsync();
        }


        public async Task DeleteUserAsync(string userId)
        {
            var param = new SqlParameter("@UserID", userId);
            await ExecuteSqlAsync("EXEC sp_User_Delete @UserID", param);
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

            await ExecuteSqlAsync("EXEC sp_User_Update @UserID, @UserName, @User_Loc, @UserLevel", parameters);
        }


    }
}
