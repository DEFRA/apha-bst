using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;

namespace Apha.BST.Core.Interfaces
{
    public interface IPersonsRepository
    {
        Task<IEnumerable<Persons>> GetAllAsync();
    }
}
