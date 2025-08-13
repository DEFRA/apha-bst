using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public List<SelectListItem> GetUserLevels() => new List<SelectListItem>
        {
           
            new SelectListItem { Value = "1", Text = "Level 1 - Superuser" },
            new SelectListItem { Value = "2", Text = "Level 2 - Data entry" },
            new SelectListItem { Value = "3", Text = "Level 3 - Read only" }
        };
    }
}
