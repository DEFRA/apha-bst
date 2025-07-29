using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Apha.BST.Core;

namespace Apha.BST.DataAccess.Repositories
{
    public class SiteRepository:ISiteRepository
    {
        private readonly BSTContext _context;
        private readonly IAuditLogRepository _auditLogRepository;
        public SiteRepository(BSTContext context, IAuditLogRepository auditLogRepository)
        {
            _context = context;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IEnumerable<Site>> GetAllSitesAsync(string plantNo)
        {
            var param = new SqlParameter("@PlantNo", plantNo);
            return await _context.Sites
                .FromSqlRaw("EXEC sp_Sites_Select @PlantNo", param)
                .ToListAsync();
        }
        public async Task<List<SiteTrainee>> GetSiteTraineesAsync(string plantNo)
        {
            var param = new SqlParameter("@PlantNo", plantNo);
            return await _context.SiteTrainees
                .FromSqlRaw("EXEC sp_Site_Trainee_Get @PlantNo", param)
                .ToListAsync();
        }
        
        public async Task<string> AddSiteAsync(Site site)
        {
            var parameters = new[]
            {
        new SqlParameter("@PlantNo", site.PlantNo),
        new SqlParameter("@Name", site.Name),
        new SqlParameter("@Add1", site.AddressLine1 ?? (object)DBNull.Value),
        new SqlParameter("@Add2", site.AddressLine2 ?? (object)DBNull.Value),
        new SqlParameter("@AddTown", site.AddressTown ?? (object)DBNull.Value),
        new SqlParameter("@AddCounty", site.AddressCounty ?? (object)DBNull.Value),
        new SqlParameter("@AddPCode", site.AddressPostCode ?? (object)DBNull.Value),
        new SqlParameter("@AddTel", site.Telephone ?? (object)DBNull.Value),
        new SqlParameter("@AddFax", site.Fax ?? (object)DBNull.Value),
        new SqlParameter("@AddAHVLA", site.Ahvla),
        new SqlParameter
        {
            ParameterName = "@ReturnCode",
            SqlDbType = SqlDbType.TinyInt,
            Direction = ParameterDirection.InputOutput,
            Value = 0
        }
    };
            string error = null;
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Sites_Add @PlantNo, @Name, @Add1, @Add2, @AddTown, @AddCounty, @AddPCode, @AddTel, @AddFax, @AddAHVLA, @ReturnCode OUT", parameters);
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                // Optionally: send email notification here if required (omitted for brevity)
                throw;
            }
            finally
            {
                // Audit log for sp_Sites_Add, unless it's sp_Audit_log_DELETE or sp_Usage_Insert
                if (!string.Equals("sp_Sites_Add", "sp_Audit_log_DELETE", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals("sp_Sites_Add", "sp_Usage_Insert", StringComparison.OrdinalIgnoreCase))
                {
                    await _auditLogRepository.AddAuditLogAsync(
                        "sp_Sites_Add",
                        parameters,
                        "Write",
                        error
                    );
                }
            }
            var returnCode = (byte)parameters[10].Value;          
            if (returnCode == 1)
                return "EXISTS"; // Code 1 = Site already exists

            return "SUCCESS"; // Code 0 = Insert success
        }

        public async Task<string?> GetPersonNameByIdAsync(int personId)
        {
            return await _context.Persons
                                 .Where(p => p.PersonId == personId)
                                 .Select(p => p.Person)
                                 .FirstOrDefaultAsync();
        }


        public async Task<bool> DeleteTraineeAsync(int personId)
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

            string error = null;
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Trainee_Delete @PersonID, @PersonTraining OUTPUT", parameters);

                var personTraining = (byte)parameters[1].Value;

                // If person has training records, return false (can't delete)
                return personTraining == 0;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                // Log error or handle accordingly
                return false;
            }
            finally
            {
                await _auditLogRepository.AddAuditLogAsync("sp_Trainee_Delete", parameters, "Write", error);
            }
        }
    }
}
