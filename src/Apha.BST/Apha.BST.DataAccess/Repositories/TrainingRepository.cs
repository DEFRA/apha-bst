using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class TrainingRepository:ITrainingRepository
    {
        private readonly BSTContext _context;
        private readonly IAuditLogRepository _auditLogRepository;

        public TrainingRepository(BSTContext context, IAuditLogRepository auditLogRepository)
        {
            _context = context;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<List<Trainee>> GetAllTraineesAsync()
        {
            return await _context.Traines
                .FromSqlRaw("EXEC sp_Trainee_Training_Select")
                .ToListAsync();
        }
        public async Task<IEnumerable<TrainerTraining>> GetTrainingByTraineeAsync(string traineeId)
        {
            var param = new SqlParameter("@TraineeID", traineeId);

            return await _context.TrainerTrainings
                //.FromSqlRaw("EXEC sp_Trainee_Training_Select @TraineeID", param)
                .FromSqlRaw("EXEC sp_Training_Select @TraineeID", param)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<TrainerTraining>> GetAllTrainingsAsync()
        {
            var param = new SqlParameter("@TraineeID", DBNull.Value);
            return await _context.TrainerTrainings
                .FromSqlRaw("EXEC sp_Training_Select @TraineeID", param)
                .ToListAsync();
        }

        //For TrainerHistory
        public async Task<IEnumerable<TrainerHistory>> GetTrainerHistoryAsync(int personId, string animalType)
        {
            var personIdParam = new SqlParameter("@PersonID", personId);
            var animalTypeParam = new SqlParameter("@AnimalType", animalType);

            return await _context.Set<TrainerHistory>()
                .FromSqlRaw("EXEC sp_Trainer_TrainedBy_Trained @PersonID, @AnimalType", personIdParam, animalTypeParam)
                .ToListAsync();
        }      


        public async Task<Training?> GetTrainingByKeysAsync(int personId, string species, DateTime dateTrained)
        {
            return await _context.Trainings.FirstOrDefaultAsync(t =>
                t.PersonId == personId &&
                t.TrainingAnimal == species &&
                t.TrainingDateTime == dateTrained);
        }

        public async Task<string> UpdateTrainingAsync(Training training, DateTime dateTrainedOld, string speciesOld)
        {
            var parameters = new[]
            {
                 new SqlParameter("@TraineeID", training.PersonId),
                 new SqlParameter("@TrainerID", training.TrainerId),
                 new SqlParameter("@Species", training.TrainingAnimal),
                 new SqlParameter("@TrainingType", training.TrainingType),
                 new SqlParameter("@DateTrained", training.TrainingDateTime),
                 new SqlParameter("@DateTrainedOld", dateTrainedOld),
                 new SqlParameter("@SpeciesOld", speciesOld)
            };

            string error = null;
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Training_Update @TraineeID, @TrainerID, @Species, @TrainingType, @DateTrained, @DateTrainedOld, @SpeciesOld", parameters);
                return "Training record updated successfully.";
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return $"Error updating training record: {ex.Message}";
            }
            finally
            {
                await _auditLogRepository.AddAuditLogAsync("sp_Training_Update", parameters, "Write", error);
            }
        }


        public async Task<string> AddTrainingAsync(Training training)
        {
            var parameters = new[]
            {
            new SqlParameter("@TraineeID", training.PersonId),
            new SqlParameter("@TrainerID", training.TrainerId),
            new SqlParameter("@Species", training.TrainingAnimal),
            new SqlParameter("@TrainingType", training.TrainingType),
            new SqlParameter("@DateTrained", training.TrainingDateTime),
            new SqlParameter
            {
                ParameterName = "@ReturnCode",
                SqlDbType = SqlDbType.TinyInt,
                Direction = ParameterDirection.Output,
                Value = 0
            }
            };

            string error = null;
            try
            {
                await _context.Database.ExecuteSqlRawAsync(                  
                   "EXEC sp_Training_Add @TraineeID, @TrainerID, @Species, @DateTrained, @TrainingType, @ReturnCode OUT",
                    parameters);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw;
            }            
            var returnCode = (byte)parameters[5].Value;

            if (returnCode == 1)
                return "EXISTS"; 

            return "SUCCESS";             
        }

        public async Task<Persons?> GetPersonByIdAsync(int personId)
        {
            return await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
        }
        
        public async Task<string> DeleteTrainingAsync(int traineeId, string species, DateTime dateTrained)
        {
            string error = null;
            string resultMessage;

            var parameters = new[]
            {
                 new SqlParameter("@TraineeID", traineeId),
                 new SqlParameter("@Species", species),
                 new SqlParameter("@DateTrained", dateTrained)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Training_Delete @TraineeID, @Species, @DateTrained", parameters);    

            resultMessage = $"All trained in {species}  brainstem removal on {dateTrained:G} has been deleted from the database";
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                resultMessage = $"Delete failed: {ex.Message}";
            }
            finally
            {
                await _auditLogRepository.AddAuditLogAsync("sp_Training_Delete", parameters, "Write", error);
            }

            return resultMessage;
        }


    }
}
