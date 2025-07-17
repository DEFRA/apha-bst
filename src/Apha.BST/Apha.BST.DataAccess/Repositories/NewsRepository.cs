using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.DataAccess.Data;

namespace Apha.BST.DataAccess.Repositories
{
    public class NewsRepository
    {
        private readonly BSTContext _context;
        public NewsRepository(BSTContext context)
        {
            _context = context;
        }
    }
}
