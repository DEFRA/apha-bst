using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string?> GetSiteByIdAsync(int personId)
        {
            return await _context.Persons
                                .Where(p => p.PersonId == personId)
                                .Select(p => p.LocationId)
                                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetPersonNameByIdAsync(int personId)
        {
            return await _context.Persons
                                 .Where(p => p.PersonId == personId)
                                 .Select(p => p.Person)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PersonLookup>> GetAllPersonsForDropdownAsync()
        {
            return await _context.PersonLookup
                .FromSqlRaw("EXEC sp_Trainee_Training_Select")
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonDetails>> GetAllPersonByNameAsync(int person)
        {
            var param = new SqlParameter("@Person", person);
            return await _context.PersonDetails
                      .FromSqlRaw("EXEC sp_Trainee_Select @Person", param)
                      .ToListAsync();
        }

        public async Task<IEnumerable<PersonSiteLookup>> GetAllSitesAsync(string plantNo)
        {
            var param = new SqlParameter("@PlantNo", plantNo);
            return await _context.SiteLookup
                .FromSqlRaw("EXEC sp_Sites_Select @PlantNo", param)
                .ToListAsync();
        }
        public async Task<string> AddPersonAsync(AddPerson persons,string userName)
        {
            var parameters = new[]
            {
                new SqlParameter("@Person", persons.Name),
                new SqlParameter("@LocationID", persons.PlantNo)
            };
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Trainee_Add @Person, @LocationID", parameters);
                return "SUCCESS";
            }
            catch
            {
                return $"FAIL";
            }
        }
        public async Task<bool> DeletePersonAsync(int personId)
        {
            var parameters = new[]
            {
                new SqlParameter("@PersonID", personId),
                new SqlParameter
                {
                    ParameterName = "@PersonTraining",
                    SqlDbType = SqlDbType.TinyInt,
                    Direction = ParameterDirection.Output,
                    Value = 0
                }
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Trainee_Delete @PersonID, @PersonTraining OUTPUT", parameters);
                var personTraining = (byte)parameters[1].Value;
                return personTraining == 0;
            }
            catch
            {
                return false;
            }
        }
        public async Task<string> UpdatePersonAsync(EditPerson editPerson)
        {
            var parameters = new[]
           {
            new SqlParameter("@TraineeNo", editPerson.PersonID),
            new SqlParameter("@Trainee", editPerson.Person),
            new SqlParameter("@Plant", editPerson.Name)
            };
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Trainee_Update @TraineeNo, @Trainee, @Plant",
                    parameters);
                return "SUCCESS";
            }
            catch
            {
                return $"FAIL";
            }
        }
    }
}
