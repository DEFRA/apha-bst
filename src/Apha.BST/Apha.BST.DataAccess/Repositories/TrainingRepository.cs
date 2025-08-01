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
        private readonly BstContext _context;       

        public TrainingRepository(BstContext context)
        {
            _context = context;           
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

        //For TrainerTrained
        public async Task<IEnumerable<TrainerTrained>> GetTrainerTrainedAsync(int trainerId)
        {
            var param = new SqlParameter("@Trainer", trainerId);
            return await _context.Set<TrainerTrained>()
                .FromSqlRaw("EXEC sp_Trainer_Has_Trained @Trainer", param)
                .ToListAsync();
        }

        public async Task<Training?> GetTrainingByKeysAsync(int traineeId, int trainerId, string species, DateTime dateTrained, string trainingType)
        {
            DateTime fromDate = dateTrained.Date;
            DateTime toDate = fromDate.AddDays(1);

            return await _context.Trainings.FirstOrDefaultAsync(t =>
                t.PersonId == traineeId &&
                t.TrainerId == trainerId &&
                t.TrainingAnimal == species &&
                t.TrainingDateTime >= fromDate &&
                t.TrainingDateTime < toDate &&
                t.TrainingType == trainingType);
        }

        public async Task<string> UpdateTrainingAsync(EditTraining editTraining)
        {
            var parameters = new[]
            {
            new SqlParameter("@TraineeID", editTraining.TraineeIdOld),
            new SqlParameter("@DateTrained", editTraining.TrainingDateTime),
            new SqlParameter("@DateTrainedOld", editTraining.TrainingDateTimeOld),
            new SqlParameter("@Species", editTraining.TrainingAnimal),
            new SqlParameter("@SpeciesOld", editTraining.TrainingAnimalOld),
            new SqlParameter("@Trainer",  editTraining.TrainerId),
            new SqlParameter("@TrainerOld", editTraining.TrainerIdOld),
            new SqlParameter("@TrainingType", editTraining.TrainingType)           
            };            
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Training_Update @TraineeID, @DateTrained, @DateTrainedOld, @Species, @SpeciesOld, @Trainer, @TrainerOld, @TrainingType",
                    parameters);
                return "SUCCESS";

            }
            catch
            {
                               
                return $"FAIL";
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
           
            try
            {
                await _context.Database.ExecuteSqlRawAsync(                  
                   "EXEC sp_Training_Add @TraineeID, @TrainerID, @Species, @DateTrained, @TrainingType, @ReturnCode OUT",
                    parameters);
            }
            catch(Exception ex)
            {              
                throw ex;
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
            var parameters = new[]
            {
                 new SqlParameter("@TraineeID", traineeId),
                 new SqlParameter("@Species", species),
                 new SqlParameter("@DateTrained", dateTrained)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Training_Delete @TraineeID, @Species, @DateTrained", parameters);
                return "SUCCESS";
            }
            catch 
            {
                           
                return $"FAIL";
            }          
        }


    }
}
