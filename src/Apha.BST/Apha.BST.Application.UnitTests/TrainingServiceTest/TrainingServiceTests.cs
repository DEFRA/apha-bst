using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Services;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Application.UnitTests.TrainingServiceTest
{
    public class TrainingServiceTests : AbstractTrainingServiceTest
    {
       
        [Fact]
        public async Task GetAllTrainingsAsync_ShouldReturnMappedTrainings_WhenTrainingsExist()
        {
            var trainings = new List<TrainerTraining>
        {
            new TrainerTraining { PersonID = 1, Name = "Training 1" },
            new TrainerTraining { PersonID = 2, Name = "Training 2" }
        };
            var mappedTrainings = new List<TrainerTrainingDTO>
        {
            new TrainerTrainingDTO { PersonID = 1, Name = "Training 1" },
            new TrainerTrainingDTO { PersonID = 2, Name = "Training 2" }
        };

            MockForGetAllTrainings(trainings, mappedTrainings);

            var result = await _trainingService!.GetAllTrainingsAsync();

            Assert.NotNull(result);
            Assert.Equal(mappedTrainings, result);
        }

        [Fact]
        public async Task GetAllTrainingsAsync_ShouldReturnEmptyList_WhenNoTrainingsExist()
        {
            var emptyTrainings = new List<TrainerTraining>();
            var emptyMapped = new List<TrainerTrainingDTO>();

            MockForGetAllTrainings(emptyTrainings, emptyMapped);

            var result = await _trainingService!.GetAllTrainingsAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("1")]
        public async Task GetTrainingByTraineeAsync_ShouldReturnTrainings(string traineeId)
        {
            var trainings = new List<TrainerTraining> { new TrainerTraining() };
            var mappedDtos = new List<TrainerTrainingDTO> { new TrainerTrainingDTO() };

            MockForGetTrainingByTrainee(traineeId, trainings, mappedDtos);

            var result = await _trainingService!.GetTrainingByTraineeAsync(traineeId);

            Assert.Equal(mappedDtos, result);
        }

        [Theory]
        [InlineData("invalid")]
        public async Task GetTrainingByTraineeAsync_ShouldReturnEmpty_WhenTraineeIdIsInvalid(string traineeId)
        {
            var trainings = new List<TrainerTraining>();
            var mappedDtos = new List<TrainerTrainingDTO>();

            MockForGetTrainingByTrainee(traineeId, trainings, mappedDtos);

            var result = await _trainingService!.GetTrainingByTraineeAsync(traineeId);

            Assert.Equal(mappedDtos, result); // Empty result, so assert that it's equal to an empty list
        }


        [Fact]
        public async Task GetTrainingByTraineeAsync_EmptyResult_ReturnsEmptyList()
        {
            var trainings = new List<TrainerTraining>();
            var mappedDtos = new List<TrainerTrainingDTO>();

            MockForGetTrainingByTrainee("1", trainings, mappedDtos);

            var result = await _trainingService!.GetTrainingByTraineeAsync("1");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTrainingByTraineeAsync_RepositoryThrowsException_PropagatesException()
        {
            MockForTrainingByTrainee_Exception("1", repoThrows: true, mapperThrows: false);

            await Assert.ThrowsAsync<Exception>(() => _trainingService!.GetTrainingByTraineeAsync("1"));
        }

        [Fact]
        public async Task GetTrainingByTraineeAsync_MapperThrowsException_PropagatesException()
        {
            MockForTrainingByTrainee_Exception("1", repoThrows: false, mapperThrows: true);

            await Assert.ThrowsAsync<AutoMapperMappingException>(() => _trainingService!.GetTrainingByTraineeAsync("1"));
        }
        [Fact]
        public async Task GetTrainerHistoryAsync_SuccessfulRetrieval_ReturnsTrainerHistory()
        {
            int personId = 1;
            string animalType = "Dog";
            var mockHistory = new List<TrainerHistory> { new TrainerHistory(), new TrainerHistory() };
            var expectedDtos = new List<TrainerHistoryDTO> { new TrainerHistoryDTO(), new TrainerHistoryDTO() };

            MockForGetTrainerHistory(personId, animalType, mockHistory, expectedDtos);

            var result = await _trainingService!.GetTrainerHistoryAsync(personId, animalType);

            Assert.Equal(expectedDtos, result);
        }

        [Fact]
        public async Task GetTrainerHistoryAsync_EmptyResult_ReturnsEmptyList()
        {
            int personId = 1;
            string animalType = "Cat";
            var mockHistory = new List<TrainerHistory>();
            var expectedDtos = new List<TrainerHistoryDTO>();

            MockForGetTrainerHistory(personId, animalType, mockHistory, expectedDtos);

            var result = await _trainingService!.GetTrainerHistoryAsync(personId, animalType);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTrainerHistoryAsync_InvalidPersonId_ReturnsEmptyList()
        {
            int invalidPersonId = -1;
            string animalType = "Dog";
            var mockHistory = new List<TrainerHistory>();
            var expectedDtos = new List<TrainerHistoryDTO>();

            MockForGetTrainerHistory(invalidPersonId, animalType, mockHistory, expectedDtos);

            var result = await _trainingService!.GetTrainerHistoryAsync(invalidPersonId, animalType);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTrainerHistoryAsync_InvalidAnimalType_ReturnsEmptyList()
        {
            int personId = 1;
            string invalidAnimalType = "InvalidType";
            var mockHistory = new List<TrainerHistory>();
            var expectedDtos = new List<TrainerHistoryDTO>();

            MockForGetTrainerHistory(personId, invalidAnimalType, mockHistory, expectedDtos);

            var result = await _trainingService!.GetTrainerHistoryAsync(personId, invalidAnimalType);

            Assert.Empty(result);
        }
    }
}
