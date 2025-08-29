using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess.Repositories;
using Apha.BST.DataAccess.UnitTests.Helpers;
using Moq.Protected;
using Moq;
using Microsoft.Data.SqlClient;

namespace Apha.BST.DataAccess.UnitTests.TrainingRepositoryTest
{
    public class TrainingRepositoryTests
    {
        [Fact]
        public async Task GetAllTraineesAsync_ReturnsTraineeTrainers()
        {
            var trainees = new TestAsyncEnumerable<TraineeTrainer>(new[]
            {
                new TraineeTrainer { PersonId = 1, Person = "John" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractTrainingRepositoryTest(mockContext.Object, traineeTrainers: trainees);

            var result = await repo.GetAllTraineesAsync();

            Assert.Single(result);
            Assert.Equal("John", result[0].Person);
        }

        [Fact]
        public async Task GetTrainingByTraineeAsync_ReturnsTrainerTrainings()
        {
            var trainings = new TestAsyncEnumerable<TrainerTraining>(new[]
            {
                new TrainerTraining { PersonID = 1, Person = "John", TrainingAnimal = "Dog", TrainingType = "TypeA" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractTrainingRepositoryTest(mockContext.Object, trainerTrainings: trainings);

            var result = await repo.GetTrainingByTraineeAsync("1");

            Assert.Single(result);
            Assert.Equal("John", result.First().Person);
        }

        [Fact]
        public async Task GetAllTrainingsAsync_ReturnsTrainerTrainings()
        {
            var trainings = new TestAsyncEnumerable<TrainerTraining>(new[]
            {
                new TrainerTraining { PersonID = 2, Person = "Jane", TrainingAnimal = "Cat", TrainingType = "TypeB" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractTrainingRepositoryTest(mockContext.Object, trainerTrainings: trainings);

            var result = await repo.GetAllTrainingsAsync();

            Assert.Single(result);
            Assert.Equal("Jane", result.First().Person);
        }

        [Fact]
        public async Task GetTrainerHistoryAsync_ReturnsTrainerHistories()
        {
            var histories = new TestAsyncEnumerable<TrainerHistory>(new[]
            {
                new TrainerHistory { PersonID = 1, Person = "John", TrainingAnimal = "Dog" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractTrainingRepositoryTest(mockContext.Object, trainerHistories: histories);

            var result = await repo.GetTrainerHistoryAsync(1, "Dog");

            Assert.Single(result);
            Assert.Equal("John", result.First().Person);
        }

        [Fact]
        public async Task GetTrainerTrainedAsync_ReturnsTrainerTraineds()
        {
            var traineds = new TestAsyncEnumerable<TrainerTrained>(new[]
            {
                new TrainerTrained { Site = "Site1" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractTrainingRepositoryTest(mockContext.Object, trainerTraineds: traineds);

            var result = await repo.GetTrainerTrainedAsync(1);

            Assert.Single(result);
            Assert.Equal("Site1", result.First().Site);
        }

        [Fact]
        public async Task GetTrainingByKeysAsync_ReturnsTraining()
        {
            var trainings = new TestAsyncEnumerable<Training>(new[]
            {
               new Training
               {
                   PersonId = 1,
                   TrainerId = 2,
                   TrainingAnimal = "Dog",
                   TrainingDateTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), // Specify DateTimeKind  
                   TrainingType = "TypeA"
               }
           });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractTrainingRepositoryTest(mockContext.Object, trainings: trainings);

            var result = await repo.GetTrainingByKeysAsync(1, 2, "Dog", new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), "TypeA"); // Specify DateTimeKind  

            Assert.NotNull(result);
            Assert.Equal(2, result.TrainerId);
        }

        [Fact]
        public async Task GetPersonByIdAsync_ReturnsPerson()
        {
            var persons = new TestAsyncEnumerable<Persons>(new[]
            {
                new Persons { PersonId = 1, Person = "John" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractTrainingRepositoryTest(mockContext.Object, persons: persons);

            var result = await repo.GetPersonByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("John", result.Person);
        }

        [Fact]
        public async Task AddTrainingAsync_ReturnsSuccess_WhenNotExists()
        {
            var training = new Training
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingAnimal = "Dog",
                TrainingType = "TypeA",
                TrainingDateTime = DateTime.Now
            };
            var mockContext = new Mock<BstContext>();
            var repo = new Mock<TrainingRepository>(mockContext.Object) { CallBase = true };

            repo.Protected()
                .Setup<Task<int>>("ExecuteSqlAsync",
                    ItExpr.Is<string>(s => s.Contains("sp_Training_Add")),
                    ItExpr.IsAny<object[]>())
                .Callback<string, object[]>((sql, parameters) =>
                {
                    ((SqlParameter)parameters[5]).Value = (byte)0;
                })
                .ReturnsAsync(1);

            var result = await repo.Object.AddTrainingAsync(training);

            Assert.Equal(TrainingRepository.Success, result);
        }

        [Fact]
        public async Task AddTrainingAsync_ReturnsExists_WhenAlreadyExists()
        {
            var training = new Training
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingAnimal = "Dog",
                TrainingType = "TypeA",
                TrainingDateTime = DateTime.Now
            };
            var mockContext = new Mock<BstContext>();
            var repo = new Mock<TrainingRepository>(mockContext.Object) { CallBase = true };

            repo.Protected()
                .Setup<Task<int>>("ExecuteSqlAsync",
                    ItExpr.Is<string>(s => s.Contains("sp_Training_Add")),
                    ItExpr.IsAny<object[]>())
                .Callback<string, object[]>((sql, parameters) =>
                {
                    ((SqlParameter)parameters[5]).Value = (byte)1;
                })
                .ReturnsAsync(1);

            var result = await repo.Object.AddTrainingAsync(training);

            Assert.Equal(TrainingRepository.Exists, result);
        }

        [Fact]
        public async Task UpdateTrainingAsync_ReturnsSuccess_OnSuccess()
        {
            var editTraining = new EditTraining
            {
                TraineeIdOld = 1,
                TrainingDateTime = DateTime.Now,
                TrainingDateTimeOld = DateTime.Now.AddDays(-1),
                TrainingAnimal = "Dog",
                TrainingAnimalOld = "Dog",
                TrainerId = 2,
                TrainerIdOld = 2,
                TrainingType = "TypeA"
            };
            var mockContext = new Mock<BstContext>();
            var repo = new Mock<TrainingRepository>(mockContext.Object) { CallBase = true };

            repo.Protected()
                .Setup<Task<int>>("ExecuteSqlAsync",
                    ItExpr.Is<string>(s => s.Contains("sp_Training_Update")),
                    ItExpr.IsAny<object[]>())
                .ReturnsAsync(1);

            var result = await repo.Object.UpdateTrainingAsync(editTraining);

            Assert.Equal(TrainingRepository.Success, result);
        }



        [Fact]
        public async Task DeleteTrainingAsync_ReturnsSuccess_OnSuccess()
        {
            var mockContext = new Mock<BstContext>();
            var repo = new Mock<TrainingRepository>(mockContext.Object) { CallBase = true };

            repo.Protected()
                .Setup<Task<int>>("ExecuteSqlAsync",
                    ItExpr.Is<string>(s => s.Contains("sp_Training_Delete")),
                    ItExpr.IsAny<object[]>())
                .ReturnsAsync(1);

            var result = await repo.Object.DeleteTrainingAsync(1, "Dog", DateTime.Now);

            Assert.Equal(TrainingRepository.Success, result);
        }

    }
}
