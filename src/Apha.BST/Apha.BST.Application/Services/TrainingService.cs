using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using AutoMapper;

namespace Apha.BST.Application.Services
{
    public class TrainingService: ITrainingService
    {
        private readonly ITrainingRepository _trainingRepository;
        private readonly IMapper _mapper;
        public const string Fail = "FAIL";
        public const string Exists = "EXISTS";

        public TrainingService(ITrainingRepository trainingRepository, IMapper mapper)
        {
            _trainingRepository = trainingRepository;
            _mapper = mapper;            
        }
        public async Task<List<TraineeDto>> GetTraineesAsync()
        {
            var persons = await _trainingRepository.GetAllTraineesAsync();
            return _mapper.Map<List<TraineeDto>>(persons);
        }

        public async Task<IEnumerable<TrainerTrainingDto>> GetTrainingByTraineeAsync(string traineeId)
        {           
            var trainings = await _trainingRepository.GetTrainingByTraineeAsync(traineeId);
            return _mapper.Map<IEnumerable<TrainerTrainingDto>>(trainings);
        }
        public async Task<IEnumerable<TrainerTrainingDto>> GetAllTrainingsAsync()
        {
            var trainings = await _trainingRepository.GetAllTrainingsAsync();
            return _mapper.Map<IEnumerable<TrainerTrainingDto>>(trainings);
        }

        //For EditTraining      
        public async Task<TrainingDto?> GetTrainingByKeysAsync(int traineeId, int trainerId, string species, DateTime dateTrained, string trainingType)
        {
            var training = await _trainingRepository.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType);
            return _mapper.Map<TrainingDto>(training);
        }

        public async Task<string> UpdateTrainingAsync(EditTrainingDto dto)
        {
            if (dto.TraineeIdOld == dto.TrainerId)
            {
                return "Trainee and Trainer cannot be the same person.";
            }
            var editTraining = _mapper.Map<EditTraining>(dto);          

            // Fetch names for message
            var trainee = await _trainingRepository.GetPersonByIdAsync(dto.TraineeId);
            var trainer = await _trainingRepository.GetPersonByIdAsync(dto.TrainerId);

            string traineeName = trainee?.Person ?? dto.TraineeId.ToString();
            string trainerName = trainer?.Person ?? dto.TrainerId.ToString();
            var result = await _trainingRepository.UpdateTrainingAsync(editTraining);
            if (result.StartsWith(Fail))
            {
                return $"Save failed.";
            }

            return $"{traineeName} has been trained in {dto.TrainingAnimal} brainstem removal on {dto.TrainingDateTime.ToString("d", CultureInfo.CurrentCulture)} by {trainerName}";

        }

        //For TrainerHistory
        public async Task<IEnumerable<TrainerHistoryDto>> GetTrainerHistoryAsync(int personId, string animalType)
        {
            var history = await _trainingRepository.GetTrainerHistoryAsync(personId, animalType);
            return _mapper.Map<IEnumerable<TrainerHistoryDto>>(history);
        }

        //For TrainerTrained
        public async Task<IEnumerable<TrainerTrainedDto>> GetTrainerTrainedAsync(int trainerId)
        {
            var trained = await _trainingRepository.GetTrainerTrainedAsync(trainerId);
            return _mapper.Map<IEnumerable<TrainerTrainedDto>>(trained);
        }

        public async Task<string> AddTrainingAsync(TrainingDto trainingDto)
        {
            if (trainingDto.PersonId == trainingDto.TrainerId)
            {
                return "Trainee and Trainer cannot be the same person.";
            }
            var training = _mapper.Map<Training>(trainingDto);
            var result = await _trainingRepository.AddTrainingAsync(training);
            // Fetch names from Persons table
            var trainee = await _trainingRepository.GetPersonByIdAsync(trainingDto.PersonId);
            var trainer = await _trainingRepository.GetPersonByIdAsync(trainingDto.TrainerId);

            string traineeName = trainee?.Person ?? trainingDto.PersonId.ToString();
            string trainerName = trainer?.Person ?? trainingDto.TrainerId.ToString();

            if (result == Exists)
            {
                return $"{traineeName} has already trained for {trainingDto.TrainingType} brainstem removal: Cannot save record";
            }

            return $"{traineeName} has been trained in {trainingDto.TrainingType} brainstem removal on {trainingDto.TrainingDateTime.ToString("d", CultureInfo.CurrentCulture)} by {trainerName}";
           
        }
        public async Task<string> DeleteTrainingAsync(int traineeId, string species, DateTime dateTrained)
        {
            var result = await _trainingRepository.DeleteTrainingAsync(traineeId, species, dateTrained);
            var trainee = await _trainingRepository.GetPersonByIdAsync(traineeId);
            string traineeName = trainee?.Person ?? traineeId.ToString();

            if (result.StartsWith(Fail))
            {
                return $"Delete failed.";
            }

            return $"{traineeName} trained in {species} brainstem removal on {dateTrained:dd/MM/yyyy} has been deleted from the database";
        }
    }
}
