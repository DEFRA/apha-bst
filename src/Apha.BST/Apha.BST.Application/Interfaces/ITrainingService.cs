using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;

namespace Apha.BST.Application.Interfaces
{
    public interface ITrainingService
    {
        Task<List<PersonsDTO>> GetTraineesAsync();

        //For EditTraining
        Task<EditTrainingDTO?> GetTrainingByKeysAsync(int personId, string species, DateTime dateTrained);
        Task<string> UpdateTrainingAsync(EditTrainingDTO dto);

        //TrainerHistory
        Task<IEnumerable<TrainerHistoryDTO>> GetTrainerHistoryAsync(int personId, string animalType);        

        Task<string> AddTrainingAsync(TrainingDTO trainingDto);
        Task<IEnumerable<TrainerTrainingDTO>> GetTrainingByTraineeAsync(string traineeId);
        Task<IEnumerable<TrainerTrainingDTO>> GetAllTrainingsAsync();
        Task<string> DeleteTrainingAsync(int traineeId, string species, DateTime dateTrained);
    }
}
