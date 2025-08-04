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
using FluentAssertions.Execution;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Application.UnitTests.TrainingServiceTest
{
    public class TrainingServiceTests : AbstractTrainingServiceTest
    {
        [Fact]
        public async Task AddTrainingAsync_WithValidInput_ShouldReturnSuccessMessage()
        {
            var trainingDto = new TrainingDto
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingType = "Test",
                TrainingDateTime = DateTime.Now
            };

            var trainingEntity = new Training();
            var trainee = new Persons { Person = "John Doe" };
            var trainer = new Persons { Person = "Jane Smith" };

            MockForAddTrainingAsync(trainingDto, trainingEntity, "SUCCESS", trainee, trainer);

            var result = await _trainingService!.AddTrainingAsync(trainingDto);

            Assert.Contains("John Doe has been trained in Test brainstem removal", result);
            Assert.Contains("by Jane Smith", result);
        }

        [Fact]
        public async Task AddTrainingAsync_WithDuplicateTraining_ShouldReturnErrorMessage()
        {
            var trainingDto = new TrainingDto
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingType = "Test",
                TrainingDateTime = DateTime.Now
            };

            var trainingEntity = new Training();
            var trainee = new Persons { Person = "John Doe" };

            MockForAddTrainingAsync(trainingDto, trainingEntity, "EXISTS", trainee, null);

            var result = await _trainingService!.AddTrainingAsync(trainingDto);

            Assert.Contains("John Doe has already trained for Test brainstem removal", result);
            Assert.Contains("Cannot save record", result);
        }

        [Fact]
        public async Task AddTrainingAsync_WithNonExistentTrainee_ShouldUseIdAsName()
        {
            var trainingDto = new TrainingDto
            {
                PersonId = 999,
                TrainerId = 2,
                TrainingType = "Test",
                TrainingDateTime = DateTime.Now
            };

            var trainingEntity = new Training();
            Persons? trainee = null;
            var trainer = new Persons { Person = "Jane Smith" };

            MockForAddTrainingAsync(trainingDto, trainingEntity, "SUCCESS", trainee, trainer);

            var result = await _trainingService!.AddTrainingAsync(trainingDto);

            Assert.Contains("999 has been trained in Test brainstem removal", result);
            Assert.Contains("by Jane Smith", result);
        }

        [Fact]
        public async Task GetAllTrainingsAsync_ShouldReturnMappedTrainings_WhenTrainingsExist()
        {
            var trainings = new List<TrainerTraining>
        {
            new TrainerTraining { PersonID = 1, Name = "Training 1" },
            new TrainerTraining { PersonID = 2, Name = "Training 2" }
        };
            var mappedTrainings = new List<TrainerTrainingDto>
        {
            new TrainerTrainingDto { PersonID = 1, Name = "Training 1" },
            new TrainerTrainingDto{ PersonID = 2, Name = "Training 2" }
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
            var emptyMapped = new List<TrainerTrainingDto>();

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
            var mappedDtos = new List<TrainerTrainingDto> { new TrainerTrainingDto() };

            MockForGetTrainingByTrainee(traineeId, trainings, mappedDtos);

            var result = await _trainingService!.GetTrainingByTraineeAsync(traineeId);

            Assert.Equal(mappedDtos, result);
        }

        [Theory]
        [InlineData("invalid")]
        public async Task GetTrainingByTraineeAsync_ShouldReturnEmpty_WhenTraineeIdIsInvalid(string traineeId)
        {
            var trainings = new List<TrainerTraining>();
            var mappedDtos = new List<TrainerTrainingDto>();

            MockForGetTrainingByTrainee(traineeId, trainings, mappedDtos);

            var result = await _trainingService!.GetTrainingByTraineeAsync(traineeId);

            Assert.Equal(mappedDtos, result); // Empty result, so assert that it's equal to an empty list
        }


        [Fact]
        public async Task GetTrainingByTraineeAsync_EmptyResult_ReturnsEmptyList()
        {
            var trainings = new List<TrainerTraining>();
            var mappedDtos = new List<TrainerTrainingDto>();

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
            string animalType = "Sheep&Goat";
            var mockHistory = new List<TrainerHistory> { new TrainerHistory(), new TrainerHistory() };
            var expectedDtos = new List<TrainerHistoryDto> { new TrainerHistoryDto(), new TrainerHistoryDto() };

            MockForGetTrainerHistory(personId, animalType, mockHistory, expectedDtos);

            var result = await _trainingService!.GetTrainerHistoryAsync(personId, animalType);

            Assert.Equal(expectedDtos, result);
        }

        [Fact]
        public async Task GetTrainerHistoryAsync_EmptyResult_ReturnsEmptyList()
        {
            int personId = 1;
            string animalType = "Cattle";
            var mockHistory = new List<TrainerHistory>();
            var expectedDtos = new List<TrainerHistoryDto>();

            MockForGetTrainerHistory(personId, animalType, mockHistory, expectedDtos);

            var result = await _trainingService!.GetTrainerHistoryAsync(personId, animalType);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTrainerHistoryAsync_InvalidPersonId_ReturnsEmptyList()
        {
            int invalidPersonId = -1;
            string animalType = "Sheep&Goat";
            var mockHistory = new List<TrainerHistory>();
            var expectedDtos = new List<TrainerHistoryDto>();

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
            var expectedDtos = new List<TrainerHistoryDto>();

            MockForGetTrainerHistory(personId, invalidAnimalType, mockHistory, expectedDtos);

            var result = await _trainingService!.GetTrainerHistoryAsync(personId, invalidAnimalType);

            Assert.Empty(result);
        }
        [Fact]
        public async Task GetTraineesAsync_ShouldReturnMappedTrainees_WhenTraineesExist()
        {
            var trainees = new List<TraineeTrainer>
    {
        new TraineeTrainer { PersonId = 1, Person = "John Doe" },
        new TraineeTrainer { PersonId = 2, Person = "Jane Smith" }
    };
            var mappedDtos = new List<PersonsDto>
    {
        new PersonsDto { PersonId = 1, Person = "John Doe" },
        new PersonsDto { PersonId = 2, Person = "Jane Smith" }
    };

            MockForGetTraineesAsync(trainees, mappedDtos);

            var result = await _trainingService!.GetTraineesAsync();

            Assert.NotNull(result);
            Assert.Equal(mappedDtos.Count, result.Count);
            Assert.Equal(mappedDtos, result);
        }

        [Fact]
        public async Task GetTraineesAsync_ShouldReturnEmptyList_WhenNoTraineesExist()
        {
            var trainees = new List<TraineeTrainer>();
            var mappedDtos = new List<PersonsDto>();

            MockForGetTraineesAsync(trainees, mappedDtos);

            var result = await _trainingService!.GetTraineesAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public async Task GetTrainingByKeysAsync_ShouldReturnMappedDto_WhenTrainingExists()
        {
            int traineeId = 1;
            int trainerId = 2;
            string species = "Sheep&Goat";
            DateTime dateTrained = DateTime.Now;
            string trainingType = "Cascade Trained";

            var training = new Training { PersonId = traineeId };
            var mappedDto = new TrainingDto { PersonId = traineeId };

            MockForGetTrainingByKeys(training, mappedDto, traineeId, trainerId, species, dateTrained, trainingType);

            var result = await _trainingService!.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType);

            Assert.NotNull(result);
            Assert.Equal(mappedDto, result);
        }
        [Fact]
        public async Task GetTrainingByKeysAsync_ShouldReturnNull_WhenNoTrainingFound()
        {
            int traineeId = 1;
            int trainerId = 2;
            string species = "Cattle";
            DateTime dateTrained = DateTime.Now;
            string trainingType = "Trained";

            MockForGetTrainingByKeys(null, null, traineeId, trainerId, species, dateTrained, trainingType);

            var result = await _trainingService!.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType);

            Assert.Null(result);
        }
        [Fact]
        public async Task GetTrainingByKeysAsync_ShouldCallRepoCorrectly_WithEdgeInputs()
        {
            int traineeId = 0;
            int trainerId = int.MaxValue;
            string species = "";
            DateTime dateTrained = DateTime.MinValue;
            string trainingType = string.Empty;

            var mockRepo = Substitute.For<ITrainingRepository>();
            var mockMapper = Substitute.For<IMapper>();
            mockRepo.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType).Returns((Training)null!);

            _trainingService = new TrainingService(mockRepo, mockMapper);

            await _trainingService.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType);

            await mockRepo.Received(1).GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType);
        }
        [Fact]
        public async Task GetTrainingByKeysAsync_ShouldThrowException_WhenRepoFails()
        {
            int traineeId = 1;
            int trainerId = 2;
            string species = "Cattle";
            DateTime dateTrained = DateTime.Now;
            string trainingType = "Training Confirmed";

            var mockRepo = Substitute.For<ITrainingRepository>();
            var mockMapper = Substitute.For<IMapper>();
            mockRepo.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType)
                    .ThrowsAsync(new Exception("Database error"));

            _trainingService = new TrainingService(mockRepo, mockMapper);

            await Assert.ThrowsAsync<Exception>(() =>
                _trainingService.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType));
        }

        [Fact]
        public async Task UpdateTrainingAsync_TraineeAndTrainerAreSamePerson_ReturnsErrorMessage()
        {
            var dto = new EditTrainingDto
            {
                TraineeId = 1,
                TrainerId = 1,
                TrainingType = "Cascade training",
                TrainingAnimal = "Sheep&Goat",
                TrainingAnimalOld = "Cattle",
                TrainingDateTime = new DateTime(2025, 8, 2, 11, 0, 0, DateTimeKind.Utc)
            };

            var editTraining = new EditTraining
            {
                TrainingType = dto.TrainingType,
                TrainingAnimal = dto.TrainingAnimal,
                TrainingAnimalOld = dto.TrainingAnimalOld
            };

            MockForUpdateTrainingAsync(dto, editTraining, "", null, null);

            var result = await _trainingService!.UpdateTrainingAsync(dto);

            Assert.Equal("Trainee and Trainer cannot be the same person.", result);
        }

        [Fact]
        public async Task UpdateTrainingAsync_ValidData_ReturnsSuccessMessage()
        {
            var dto = new EditTrainingDto
            {
                TraineeId = 1,
                TrainerId = 2,
                TrainingAnimal = "Cattle",
                TrainingAnimalOld = "Cattle",
                TrainingType = "Brainstem",
                TrainingDateTime = new DateTime(2023, 5, 1, 10, 0, 0, DateTimeKind.Utc)
            };

            var editTraining = new EditTraining
            {
                TrainingType = dto.TrainingType,
                TrainingAnimal = dto.TrainingAnimal,
                TrainingAnimalOld = dto.TrainingAnimalOld
            };

            var trainee = new Persons { Person = "John" };
            var trainer = new Persons { Person = "Jane" };

            MockForUpdateTrainingAsync(dto, editTraining, "SUCCESS", trainee, trainer);

            var result = await _trainingService!.UpdateTrainingAsync(dto);

            Assert.Equal("John has been trained in Cattle brainstem removal on 01/05/2023 by Jane", result);
        }

        [Fact]
        public async Task UpdateTrainingAsync_UpdateFails_ReturnsFailureMessage()
        {
            var dto = new EditTrainingDto
            {
                TraineeId = 1,
                TrainerId = 2,
                TrainingAnimal = "Cattle",
                TrainingAnimalOld = "Cattle",
                TrainingType = "Brainstem",
                TrainingDateTime = new DateTime(2025, 8, 1, 10, 0, 0, DateTimeKind.Utc)
            };

            var editTraining = new EditTraining
            {
                TrainingType = dto.TrainingType,
                TrainingAnimal = dto.TrainingAnimal,
                TrainingAnimalOld = dto.TrainingAnimalOld
            };

            var trainee = new Persons { Person = "Alice" };
            var trainer = new Persons { Person = "Bob" };

            MockForUpdateTrainingAsync(dto, editTraining, "FAIL: Some error occurred", trainee, trainer);

            var result = await _trainingService!.UpdateTrainingAsync(dto);

            Assert.Equal("Save failed.", result);
        }
        [Fact]
        public async Task GetTrainerTrainedAsync_ShouldReturnMappedDtos_WhenTrainerHasMultipleRecords()
        {
            var trainerId = 1;
            var trainerTrained = new List<TrainerTrained>
    {
        new TrainerTrained { TraineeNo = trainerId, Site = "John Doe", SpeciesTrained = "Sheep&Goat" },
        new TrainerTrained { TraineeNo = trainerId, Site = "Jane Smith", SpeciesTrained = "Cattle" }
    };

            var mappedDtos = new List<TrainerTrainedDto>
    {
        new TrainerTrainedDto { TraineeNo = trainerId, Site = "John Doe", SpeciesTrained = "Sheep&Goat" },
        new TrainerTrainedDto { TraineeNo = trainerId, Site = "Jane Smith", SpeciesTrained = "Cattle" }
    };

            MockForGetTrainerTrained(trainerId, trainerTrained, mappedDtos);

            var result = await _trainingService!.GetTrainerTrainedAsync(trainerId);

            Assert.NotNull(result);
            Assert.Equal(mappedDtos, result);
        }

        [Fact]
        public async Task GetTrainerTrainedAsync_ShouldReturnEmptyList_WhenTrainerHasNoRecords()
        {
            var trainerId = 2;
            var trainerTrained = new List<TrainerTrained>();
            var mappedDtos = new List<TrainerTrainedDto>();

            MockForGetTrainerTrained(trainerId, trainerTrained, mappedDtos);

            var result = await _trainingService!.GetTrainerTrainedAsync(trainerId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTrainerTrainedAsync_ShouldReturnEmptyList_WhenTrainerIdIsInvalid()
        {
            var trainerId = -1;
            var trainerTrained = new List<TrainerTrained>();
            var mappedDtos = new List<TrainerTrainedDto>();

            MockForGetTrainerTrained(trainerId, trainerTrained, mappedDtos);

            var result = await _trainingService!.GetTrainerTrainedAsync(trainerId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Fact]
        public async Task DeleteTrainingAsync_SuccessfulDeletion_ReturnsSuccessMessage()
        {
            // Arrange
            int traineeId = 1;
            string species = "Sheep&Goat";
            DateTime dateTrained = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var person = new Persons { PersonId = traineeId, Person = "John Doe" }; // Use Persons here

            MockForDeleteTrainingAsync(traineeId, species, dateTrained, "SUCCESS", person);

            // Act
            var result = await _trainingService!.DeleteTrainingAsync(traineeId, species, dateTrained);

            // Assert
            Assert.Contains("John Doe trained in Sheep&Goat brainstem removal on 01/01/2025 has been deleted from the database", result);
        }
        [Fact]
        public async Task DeleteTrainingAsync_FailedDeletion_ReturnsFailureMessage()
        {
            // Arrange
            int traineeId = 1;
            string species = "Cattle";
            DateTime dateTrained = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            MockForDeleteTrainingAsync(traineeId, species, dateTrained, "FAIL: Record not found", null);  // Pass null for the person

            // Act
            var result = await _trainingService!.DeleteTrainingAsync(traineeId, species, dateTrained);

            // Assert
            Assert.Equal("Delete failed.", result);
        }

        [Fact]
        public async Task DeleteTrainingAsync_PersonNotFound_UsesIdAsName()
        {
            // Arrange
            int traineeId = 1;
            string species = "Sheep&Goat";
            DateTime dateTrained = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            MockForDeleteTrainingAsync(traineeId, species, dateTrained, "SUCCESS", null);  // Pass null for the person

            // Act
            var result = await _trainingService!.DeleteTrainingAsync(traineeId, species, dateTrained);

            // Assert
            Assert.Contains("1 trained in Sheep&Goat brainstem removal on 01/01/2024 has been deleted from the database", result);
        }

    }
}
