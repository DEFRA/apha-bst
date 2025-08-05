using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Core.Interfaces
{
    public interface IDataEntryRepository
    {
        Task<bool> CanEditPage(string action);
    }
}
