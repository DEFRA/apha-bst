using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.Interfaces;

namespace Apha.BST.Application.Services
{
    public class RoleMappingService : IRoleMappingService
    {
        private static readonly Dictionary<byte, string> _roleMappings = new Dictionary<byte, string>
        {
            {1, "Superuser" },
            {2, "Data entry" },
            {3, "Read only" }
        };
        public async Task<string>  GetRoleName(byte roleId)
        {
            var roleName = _roleMappings.TryGetValue(roleId, out var name) ? name : "Unknown Role";
            return await Task.FromResult(roleName);
            
        }
       
    }
}
