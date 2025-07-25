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
        //public async Task<IEnumerable<Site>> GetAllAsync()
        //{
        //    return await _context.Sites.ToListAsync();
        //}

        public async Task<IEnumerable<Site>> GetAllSitesAsync(string plantNo)
        {
            var param = new SqlParameter("@PlantNo", plantNo);
            return await _context.Sites
                .FromSqlRaw("EXEC sp_Sites_Select @PlantNo", param)
                .ToListAsync();
        }
        ////Working code
        //public async Task<List<SiteTrainee>> GetSiteTraineesAsync(string plantNo)
        //{
        //    var result = new List<SiteTrainee>();
        //    using (var connection = _context.Database.GetDbConnection())
        //    {
        //        await connection.OpenAsync();
        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = "sp_Site_Trainee_Get";
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.Add(new SqlParameter("@PlantNo", plantNo));

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    result.Add(new SiteTrainee
        //                    {
        //                        PersonId = reader.GetInt32(reader.GetOrdinal("PersonID")),
        //                        Person = reader.IsDBNull(reader.GetOrdinal("Person")) ? null : reader.GetString(reader.GetOrdinal("Person")),
        //                        Cattle = reader.GetBoolean(reader.GetOrdinal("Cattle")),
        //                        Sheep = reader.GetBoolean(reader.GetOrdinal("Sheep")),
        //                        Goats = reader.GetBoolean(reader.GetOrdinal("Goats"))
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}
        public async Task<List<SiteTrainee>> GetSiteTraineesAsync(string plantNo)
        {
            var param = new SqlParameter("@PlantNo", plantNo);
            return await _context.SiteTrainees
                .FromSqlRaw("EXEC sp_Site_Trainee_Get @PlantNo", param)
                .ToListAsync();
        }


        //public async Task<Site> AddSiteAsync(Site site)
        //{
        //    _context.Sites.Add(site);
        //    await _context.SaveChangesAsync();
        //    return site;
        //}

        public async Task<AddSiteResult> AddSiteAsync(Site site)
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
            //if (returnCode == 1)
            //    throw new Exception("Site already exists.");

            //return site;
            return new AddSiteResult
            {
                Site = site,
                ReturnCode = returnCode
            };
        }


        //public async Task<bool> DeleteTraineeAsync(int personId)
        //{
        //    var trainee = await _context.Persons.FirstOrDefaultAsync(t => t.PersonId == personId);
        //    if (trainee == null)
        //        return false;

        //    _context.Persons.Remove(trainee);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}
        //    public async Task<bool> DeleteTraineeAsync(int personId)
        //    {
        //        var hasTraining = false;
        //        string? error = null; // Fix 1: Declare error variable
        //        int rowsAffected = 0; // Fix 2: Declare rowsAffected variable

        //        var parameters = new[]
        //        {
        //    new SqlParameter("@TraineeID", personId),
        //    new SqlParameter("@PersonTraining", hasTraining)
        //};

        //        try
        //        {
        //            await _context.Database.ExecuteSqlRawAsync("EXEC sp_Trainee_Delete @TraineeID, @PersonTraining", parameters);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            error = ex.ToString();
        //            return false;
        //            // Optionally: log or handle error
        //            throw;
        //        }
        //        finally
        //        {
        //            // Audit log for sp_Trainee_Delete, unless it's sp_Audit_log_DELETE or sp_Usage_Insert
        //            if (!string.Equals("sp_Trainee_Delete", "sp_Audit_log_DELETE", StringComparison.OrdinalIgnoreCase) &&
        //                !string.Equals("sp_Trainee_Delete", "sp_Usage_Insert", StringComparison.OrdinalIgnoreCase))
        //            {
        //                await _auditLogRepository.AddAuditLogAsync(
        //                    "sp_Trainee_Delete",
        //                    parameters,
        //                    "Delete",
        //                    error
        //                );
        //            }
        //        }

        //        return rowsAffected > 0;
        //    }

        //    public async Task<bool> DeleteTraineeAsync(int personId)
        //    {
        //        var hasTraining = false;
        //        string? error = null; // Fix 1: Declare error variable
        //        int rowsAffected = 0; // Fix 2: Declare rowsAffected variable

        //        var parameters = new[]
        //        {
        //    new SqlParameter("@TraineeID", personId),
        //    new SqlParameter("@PersonTraining", hasTraining)
        //};

        //        try
        //        {
        //            rowsAffected = await _context.Database.ExecuteSqlRawAsync(
        //                "EXEC sp_Trainee_Delete @TraineeID, @PersonTraining", parameters);
        //            return rowsAffected > 0;
        //        }
        //        catch (Exception ex)
        //        {
        //            error = ex.ToString(); // Capture error for audit log
        //            return false;
        //        }
        //        finally
        //        {
        //            if (!string.Equals("sp_Trainee_Delete", "sp_Audit_log_DELETE", StringComparison.OrdinalIgnoreCase) &&
        //                !string.Equals("sp_Trainee_Delete", "sp_Usage_Insert", StringComparison.OrdinalIgnoreCase))
        //            {
        //                await _auditLogRepository.AddAuditLogAsync(
        //                    "sp_Trainee_Delete",
        //                    parameters,
        //                    "Delete",
        //                    error
        //                );
        //            }
        //        }
        //    }

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






        //public async Task<IEnumerable<Site>> GetAllSitesAsync()
        //{
        //    return await _context.Sites.ToListAsync();
        //}
        //public async Task<bool> CreateSiteAsync(Site site)
        //{
        //    try
        //    {
        //        _context.Sites.Add(site);
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
    }
}
