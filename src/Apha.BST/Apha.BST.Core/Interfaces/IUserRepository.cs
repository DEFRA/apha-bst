using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;

namespace Apha.BST.Core.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<List<VlaLocView>> GetLocationsAsync();
        Task<bool> UserExistsAsync(string userId);
        Task<List<UserView>> GetUsersAsync(string userId);
        Task DeleteUserAsync(string userId);
        Task UpdateUserAsync(User user);
    }
}
