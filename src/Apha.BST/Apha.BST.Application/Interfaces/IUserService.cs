using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;

namespace Apha.BST.Application.Interfaces
{
    public interface IUserService
    {
        Task<string> AddUserAsync(UserDto dto);
        Task<List<VlaLocDto>> GetLocationsAsync();
        Task<List<UserViewDto>> GetUsersAsync(string userId);
        Task<string> DeleteUserAsync(string userId);
        Task<string> UpdateUserAsync(UserDto dto);
        Task<UserDto?> GetUserByIdAsync(string userId);
    }
}
