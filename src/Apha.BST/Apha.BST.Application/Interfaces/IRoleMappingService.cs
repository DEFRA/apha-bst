using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.Interfaces
{
    public interface IRoleMappingService
    {

        public  Task<string> GetRoleName(byte roleId);
       

    }
}
