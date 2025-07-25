using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Core.Entities;
using AutoMapper;
using FluentAssertions;

namespace Apha.BST.Application.UnitTests.TrainingServiceTest
{
    public class TrainingServiceTests : AbstractTrainingServiceTest
    {
        [Fact]
        public async Task AddTrainingAsync_WhenSuccessful_ShouldReturnSuccessMessage()
        {
            // Arrange
            MockForAddTraining(returnCode: 0);

            var dto = new TrainingDTO
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingType = "Test Type",
                TrainingDateTime = DateTime.Now
            };

            // Act
            var result = await _trainingService!.AddTrainingAsync(dto);

            // Assert
            result.Should().Contain("John Doe has been trained in Test Type brainstem removal on");
            result.Should().Contain("by Jane Smith");
        }

        [Fact]
        public async Task AddTrainingAsync_WhenDuplicate_ShouldReturnErrorMessage()
        {
            // Arrange
            MockForAddTraining(returnCode: 1);

            var dto = new TrainingDTO
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingType = "Test Type",
                TrainingDateTime = DateTime.Now
            };

            // Act
            var result = await _trainingService!.AddTrainingAsync(dto);

            // Assert
            result.Should().Be("John Doe has already trained for Test Type brainstem removal: Cannot save record");
        }

        [Theory]
        [InlineData("0001-01-01")]
        [InlineData("9999-12-31")]
        public async Task AddTrainingAsync_EdgeCaseDates_ShouldReturnSuccessMessage(string date)
        {
            var parsedDate = DateTime.Parse(date);
            MockForAddTraining(returnCode: 0, date: parsedDate);

            var dto = new TrainingDTO
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingType = "Test Type",
                TrainingDateTime = parsedDate
            };

            var result = await _trainingService!.AddTrainingAsync(dto);

            result.Should().Contain("John Doe has been trained in Test Type brainstem removal on");
            result.Should().Contain("by Jane Smith");
        }

        [Theory]
        [InlineData(null, 2)]
        [InlineData(1, null)]
        public async Task AddTrainingAsync_NullPersonOrTrainer_ShouldReturnMessageWithId(int? personId, int? trainerId)
        {
            // Arrange
            MockForAddTrainingWithNullNames(personId, trainerId);

            var dto = new TrainingDTO
            {
                PersonId = personId ?? 0,
                TrainerId = trainerId ?? 0,
                TrainingType = "Test Type",
                TrainingDateTime = DateTime.Now
            };

            // Act
            var result = await _trainingService!.AddTrainingAsync(dto);

            // Assert
            result.Should().Contain($"{dto.PersonId} has been trained in Test Type brainstem removal on");
            result.Should().Contain($"by {dto.TrainerId}");
        }
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
    }
}
