using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace Apha.BST.DataAccess.Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly BstContext _context;
        private readonly IAuditLogRepository _auditLogRepository;
        public const string Success = "SUCCESS";
       
        public PersonsRepository(BstContext context, IAuditLogRepository auditLogRepository)
        {
            _context = context;
            _auditLogRepository = auditLogRepository;
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
            return await _context.PersonLookups
                .FromSqlRaw("EXEC sp_Trainee_Training_Select")
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonDetail>> GetAllPersonByNameAsync(int person)
        {
            var param = new SqlParameter("@Person", person);
            return await _context.PersonDetails
                      .FromSqlRaw("EXEC sp_Trainee_Select @Person", param)
                      .ToListAsync();
        }

        public async Task<IEnumerable<PersonSiteLookup>> GetAllSitesAsync(string plantNo)
        {
            var param = new SqlParameter("@PlantNo", plantNo);
            return await _context.PersonSiteLookups
                .FromSqlRaw("EXEC sp_Sites_Select @PlantNo", param)
                .ToListAsync();
        }
        public async Task<string> AddPersonAsync(AddPerson persons, string userName)
        {
            string? error = null;
           
            var parameters = new[]
            {
                new SqlParameter("@Person", persons.Name),
                new SqlParameter("@LocationID", persons.PlantNo)
            };
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Trainee_Add @Person, @LocationID", parameters);
               
            }
            catch (Exception ex)
            {
               
                error = ex.ToString();
                throw;
            }
            finally
            {
                // Audit log for sp_Trainee_Add, unless it's sp_Audit_log_DELETE or sp_Usage_Insert
                if (!string.Equals("sp_Trainee_Add", "sp_Audit_log_DELETE", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals("sp_Trainee_Add", "sp_Usage_Insert", StringComparison.OrdinalIgnoreCase))
                {
                    await _auditLogRepository.AddAuditLogAsync(
                        "sp_Trainee_Add",
                        parameters,
                        "Write",
                        userName,
                        error
                    );
                }

            }

           return Success;
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

            
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Trainee_Delete @PersonID, @PersonTraining OUTPUT", parameters);
                var personTraining = (byte)parameters[1].Value;
                return personTraining == 0;
            
        }
        public async Task<string> UpdatePersonAsync(EditPerson editPerson)
        {
            var parameters = new[]
           {
            new SqlParameter("@TraineeNo", editPerson.PersonID),
            new SqlParameter("@Trainee", editPerson.Person),
            new SqlParameter("@Plant", editPerson.Name)
            };
           
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Trainee_Update @TraineeNo, @Trainee, @Plant",
                    parameters);
            return Success;
           
        }
    }
}
