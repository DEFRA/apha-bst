using System;
using System.Collections.Generic;
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

        public TrainingService(ITrainingRepository trainingRepository, IMapper mapper)
        {
            _trainingRepository = trainingRepository;
            _mapper = mapper;            
        }
        public async Task<List<PersonsDTO>> GetTraineesAsync()
        {
            var persons = await _trainingRepository.GetAllTraineesAsync();
            return _mapper.Map<List<PersonsDTO>>(persons);
        }

        public async Task<IEnumerable<TrainerTrainingDTO>> GetTrainingByTraineeAsync(string traineeId)
        {
            if (!int.TryParse(traineeId, out int id))
            {
                id = 0; // Treat invalid or "All" input as all records
            }
            var trainings = await _trainingRepository.GetTrainingByTraineeAsync(traineeId);
            return _mapper.Map<IEnumerable<TrainerTrainingDTO>>(trainings);
        }
        public async Task<IEnumerable<TrainerTrainingDTO>> GetAllTrainingsAsync()
        {
            var trainings = await _trainingRepository.GetAllTrainingsAsync();
            return _mapper.Map<IEnumerable<TrainerTrainingDTO>>(trainings);
        }

        //For EditTraining      
        public async Task<EditTrainingDTO?> GetTrainingByKeysAsync(int personId, string species, DateTime dateTrained)
        {
            var training = await _trainingRepository.GetTrainingByKeysAsync(personId, species, dateTrained);
            return _mapper.Map<EditTrainingDTO>(training);
        }

        public async Task<string> UpdateTrainingAsync(EditTrainingDTO dto)
        {
            var training = _mapper.Map<Training>(dto);
            return await _trainingRepository.UpdateTrainingAsync(training, dto.TrainingDateTimeOld, dto.TrainingAnimalOld);
        }

        //For TrainerHistory
        public async Task<IEnumerable<TrainerHistoryDTO>> GetTrainerHistoryAsync(int personId, string animalType)
        {
            var history = await _trainingRepository.GetTrainerHistoryAsync(personId, animalType);
            return _mapper.Map<IEnumerable<TrainerHistoryDTO>>(history);
        }

        public async Task<string> AddTrainingAsync(TrainingDTO trainingDto)
        {           
            var training = _mapper.Map<Training>(trainingDto);
            var result = await _trainingRepository.AddTrainingAsync(training);
            // Fetch names from Persons table
            var trainee = await _trainingRepository.GetPersonByIdAsync(trainingDto.PersonId);
            var trainer = await _trainingRepository.GetPersonByIdAsync(trainingDto.TrainerId);

            string traineeName = trainee?.Person ?? trainingDto.PersonId.ToString();
            string trainerName = trainer?.Person ?? trainingDto.TrainerId.ToString();

            if (result == "EXISTS")
            {
                return $"{traineeName} has already trained for {trainingDto.TrainingType} brainstem removal: Cannot save record";
            }

            return $"{traineeName} has been trained in {trainingDto.TrainingType} brainstem removal on {trainingDto.TrainingDateTime:dd/MM/yyyy} by {trainerName}";
           
        }
        public async Task<string> DeleteTrainingAsync(int traineeId, string species, DateTime dateTrained)
        {
            return await _trainingRepository.DeleteTrainingAsync(traineeId, species, dateTrained);
        }
    }
}
