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
        Task<List<TraineeDto>> GetTraineesAsync();

        //For EditTraining
        Task<TrainingDto?> GetTrainingByKeysAsync(int traineeId, int trainerId, string species, DateTime dateTrained, string trainingType);
        Task<string> UpdateTrainingAsync(EditTrainingDto dto);

        //TrainerHistory
        Task<IEnumerable<TrainerHistoryDto>> GetTrainerHistoryAsync(int personId, string animalType);

        //For TrainerTrained
        Task<IEnumerable<TrainerTrainedDto>> GetTrainerTrainedAsync(int trainerId);
        Task<string> AddTrainingAsync(TrainingDto trainingDto);
        Task<IEnumerable<TrainerTrainingDto>> GetTrainingByTraineeAsync(string traineeId);
        Task<IEnumerable<TrainerTrainingDto>> GetAllTrainingsAsync();
        Task<string> DeleteTrainingAsync(int traineeId, string species, DateTime dateTrained);
    }
}
