using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly BstContext _context;
        public PersonsRepository(BstContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Persons>> GetAllAsync()
        {
            return await _context.Persons.ToListAsync();
        }
    }
}
