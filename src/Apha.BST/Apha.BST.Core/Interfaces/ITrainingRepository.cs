﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;

namespace Apha.BST.Core.Interfaces
{
    public interface ITrainingRepository
    {
        Task<List<Trainee>> GetAllTraineesAsync();

        //For EditTraining    
        Task<Training?> GetTrainingByKeysAsync(int traineeId, int trainerId, string species, DateTime dateTrained, string trainingType);
        Task<string> UpdateTrainingAsync(EditTraining editTraining);

        //TrainerHistory
        Task<IEnumerable<TrainerHistory>> GetTrainerHistoryAsync(int personId, string animalType);
        
        //For TrainerTrained
        Task<IEnumerable<TrainerTrained>> GetTrainerTrainedAsync(int trainerId);
        Task<string> AddTrainingAsync(Training training);
        Task<Persons?> GetPersonByIdAsync(int personId);
        Task<IEnumerable<TrainerTraining>> GetTrainingByTraineeAsync(string traineeId);
        Task<IEnumerable<TrainerTraining>> GetAllTrainingsAsync();
        Task<string> DeleteTrainingAsync(int traineeId, string species, DateTime dateTrained);
    }
}
