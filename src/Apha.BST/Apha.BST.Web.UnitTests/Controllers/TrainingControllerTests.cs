using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Apha.BST.Core.Interfaces;
using Apha.BST.Core.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class TrainingControllerTests
    {
        private readonly TrainingController _controller;
        private readonly ITrainingService _trainingService;
        private readonly IMapper _mapper;
        private readonly IPersonsService _personService;
        private readonly IStaticDropdownService _staticDropdownService;

        public TrainingControllerTests()
        {
            // Mock all dependencies
            _trainingService = Substitute.For<ITrainingService>();
            _mapper = Substitute.For<IMapper>();
            _personService = Substitute.For<IPersonsService>();
            _staticDropdownService = Substitute.For<IStaticDropdownService>();

            // Initialize the controller with the mocked dependencies
            _controller = new TrainingController(_trainingService, _mapper, _personService, _staticDropdownService);

            // Ensure TempData is mocked
            _controller.TempData = Substitute.For<ITempDataDictionary>();
        }
        [Fact]
        public async Task AddTraining_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddTrainingViewModel
            {
                // Set valid properties here (e.g., valid PersonId, TrainingAnimal, etc.)
                PersonId = 1,
                TrainingAnimal = "Cattle",
                TrainingDateTime = DateTime.Now
            };

            var trainingDto = new TrainingDto(); // Map ViewModel to DTO
            _mapper.Map<TrainingDto>(viewModel).Returns(trainingDto);
            _trainingService.AddTrainingAsync(trainingDto).Returns("Training added successfully");

            // Act
            var result = await _controller.AddTraining(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(TrainingController.AddTraining), redirectResult.ActionName);
            Assert.Equal("Training added successfully", _controller.TempData["Message"]);
        }

        [Fact]
        public async Task AddTraining_ExceptionThrown_ReturnsRedirectToActionResultWithErrorMessage()
        {
            // Arrange
            var viewModel = new AddTrainingViewModel();
            var trainingDto = new TrainingDto();
            _mapper.Map<TrainingDto>(viewModel).Returns(trainingDto);
            _trainingService.AddTrainingAsync(Arg.Any<TrainingDto>()).Throws(new Exception("Test exception"));

            // Act
            var result = await _controller.AddTraining(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(TrainingController.AddTraining), redirectResult.ActionName);
            Assert.Equal("Save failed", _controller.TempData["Message"]);
        }
        [Fact]
        public async Task EditTraining_GET_ValidData_ReturnsViewWithModel()
        {
            // Arrange
            int traineeId = 1;
            int trainerId = 2;
            string species = "Cattle";
            DateTime dateTrained = DateTime.Now;
            string trainingType = "Basic";

            var dto = new TrainingDto
            {
                PersonId = traineeId,
                TrainerId = trainerId,
                TrainingAnimal = species,
                TrainingDateTime = dateTrained,
                TrainingType = trainingType
            };

            _trainingService.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType)
                .Returns(dto);

            var persons = new List<PersonsDto> { new PersonsDto { PersonId = traineeId, Person = "John" } };
            _trainingService.GetTraineesAsync().Returns(Task.FromResult(persons));

            _staticDropdownService.GetTrainingTypes().Returns(new List<SelectListItem> { new SelectListItem { Value = "Basic", Text = "Basic" } });
            _staticDropdownService.GetTrainingAnimal().Returns(new List<SelectListItem> { new SelectListItem { Value = "Dog", Text = "Dog" } });

            // Act
            var result = await _controller.EditTraining(traineeId, trainerId, species, dateTrained, trainingType);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditTrainingViewModel>(viewResult.Model);
            Assert.Equal(traineeId, model.TraineeId);
            Assert.Equal(trainerId, model.TrainerId);
            Assert.Equal(species, model.TrainingAnimal);
            Assert.Equal(dateTrained, model.TrainingDateTime);
            Assert.Equal(trainingType, model.TrainingType);
        }

        [Fact]
        public async Task EditTraining_GET_InvalidData_RedirectsToViewTraining()
        {
            // Arrange
            int traineeId = 1;
            int trainerId = 2;
            string species = "Cattle";
            DateTime dateTrained = DateTime.Now;
            string trainingType = "Basic";

            _trainingService.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType)
                .Returns((TrainingDto)null!);

            // Act
            var result = await _controller.EditTraining(traineeId, trainerId, species, dateTrained, trainingType);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewTraining", redirectResult.ActionName);
            Assert.Equal("Invalid data provided for deletion.", _controller.TempData["Message"]);
        }
        [Fact]
        public async Task EditTraining_POST_ValidModel_UpdatesTrainingAndRedirects()
        {
            // Arrange
            var viewModel = new EditTrainingViewModel
            {
                TraineeId = 1,
                TrainerId = 2,
                TrainingType = "Basic",
                TrainingAnimal = "Cattle",
                TrainingDateTime = DateTime.Now,
                TraineeIdOld = 1,
                TrainerIdOld = 2,
                TrainingAnimalOld = "Cattle",
                TrainingTypeOld = "Basic",
                TrainingDateTimeOld = DateTime.Now
            };

            var editTrainingDto = new EditTrainingDto
            {
                TraineeId = viewModel.TraineeId,
                TrainerId = viewModel.TrainerId,
                TrainingType = viewModel.TrainingType,
                TrainingAnimal = viewModel.TrainingAnimal,
                TrainingDateTime = viewModel.TrainingDateTime,
                TraineeIdOld = viewModel.TraineeIdOld,
                TrainerIdOld = viewModel.TrainerIdOld,
                TrainingAnimalOld = viewModel.TrainingAnimalOld,
                TrainingDateTimeOld = viewModel.TrainingDateTimeOld
            };

            _mapper.Map<EditTrainingDto>(viewModel).Returns(editTrainingDto);
            _trainingService.UpdateTrainingAsync(editTrainingDto).Returns("Training updated successfully");

            // Act
            var result = await _controller.EditTraining(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewTraining", redirectResult.ActionName);
            Assert.Equal("Training updated successfully", _controller.TempData["Message"]);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedTraineeId"));
            Assert.Equal(viewModel.TraineeIdOld, redirectResult.RouteValues["selectedTraineeId"]);
        }

        [Fact]
        public async Task EditTraining_POST_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var viewModel = new EditTrainingViewModel
            {
                TraineeId = 0, // Invalid, triggers model error
                TrainerId = 2,
                TrainingType = "Basic",
                TrainingAnimal = "Cattle",
                TrainingDateTime = DateTime.Now,
                TraineeIdOld = 1,
                TrainerIdOld = 2,
                TrainingAnimalOld = "Cattle",
                TrainingTypeOld = "Basic",
                TrainingDateTimeOld = DateTime.Now
            };

            _controller.ModelState.AddModelError("TraineeId", "Required");

            var persons = new List<PersonsDto> { new PersonsDto { PersonId = 1, Person = "John" } };
            _trainingService.GetTraineesAsync().Returns(Task.FromResult(persons));

            _staticDropdownService.GetTrainingTypes().Returns(new List<SelectListItem> {
        new SelectListItem { Value = "Basic", Text = "Basic" }
    });

            _staticDropdownService.GetTrainingAnimal().Returns(new List<SelectListItem> {
        new SelectListItem { Value = "Cattle", Text = "Cattle" }
    });

            // Act
            var result = await _controller.EditTraining(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditTrainingViewModel>(viewResult.Model);

            Assert.Equal(viewModel.TraineeIdOld, model.TraineeIdOld);
            Assert.Equal(viewModel.TrainerIdOld, model.TrainerIdOld);
            Assert.Equal(viewModel.TrainingTypeOld, model.TrainingTypeOld);
            Assert.Equal(viewModel.TrainingAnimalOld, model.TrainingAnimalOld);
            Assert.Equal(viewModel.TrainingDateTimeOld, model.TrainingDateTimeOld);
        }

        [Fact]
        public async Task EditTraining_GET_PopulatesSelectLists()
        {
            // Arrange
            int traineeId = 1;
            int trainerId = 2;
            string species = "Cattle";
            DateTime dateTrained = DateTime.Now;
            string trainingType = "Basic";

            var dto = new TrainingDto
            {
                PersonId = traineeId,
                TrainerId = trainerId,
                TrainingAnimal = species,
                TrainingDateTime = dateTrained,
                TrainingType = trainingType
            };

            _trainingService.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType)
                .Returns(dto);

            var persons = new List<PersonsDto>
    {
        new PersonsDto { PersonId = 1, Person = "John" },
        new PersonsDto { PersonId = 2, Person = "Jane" }
    };
            _trainingService.GetTraineesAsync().Returns(Task.FromResult(persons));

            var trainingTypes = new List<SelectListItem>
    {
        new SelectListItem { Value = "Basic", Text = "Basic" },
        new SelectListItem { Value = "Advanced", Text = "Advanced" }
    };
            _staticDropdownService.GetTrainingTypes().Returns(trainingTypes);

            var animals = new List<SelectListItem>
    {
        new SelectListItem { Value = "Cattle", Text = "Cattle" },
        new SelectListItem { Value = "Sheep&Goat", Text = "Sheep&Goat" }
    };
            _staticDropdownService.GetTrainingAnimal().Returns(animals);

            // Act
            var result = await _controller.EditTraining(traineeId, trainerId, species, dateTrained, trainingType);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditTrainingViewModel>(viewResult.Model);
            Assert.Equal(2, model.TraineeList.Count());
            Assert.Equal(2, model.TrainingTypesList.Count());
            Assert.Equal(2, model.TrainingAnimalList.Count());
        }


        [Fact]
        public async Task EditTraining_POST_CallsUpdateTrainingAsync()
        {
            // Arrange
            var viewModel = new EditTrainingViewModel
            {
                TraineeId = 1,
                TrainerId = 2,
                TrainingType = "Basic",
                TrainingAnimal = "Cattle",
                TrainingDateTime = DateTime.Now,
                TraineeIdOld = 1,
                TrainerIdOld = 2,
                TrainingAnimalOld = "Cattle",
                TrainingTypeOld = "Basic",
                TrainingDateTimeOld = DateTime.Now
            };

            var editTrainingDto = new EditTrainingDto
            {
                TraineeId = 1,
                TrainerId = 2,
                TrainingType = "Basic",
                TrainingAnimal = "Cattle",
                TrainingDateTime = viewModel.TrainingDateTime,
                TraineeIdOld = 1,
                TrainerIdOld = 2,
                TrainingAnimalOld = "Cattle",
                TrainingDateTimeOld = viewModel.TrainingDateTimeOld
            };

            _mapper.Map<EditTrainingDto>(viewModel).Returns(editTrainingDto);

            // Act
            await _controller.EditTraining(viewModel);

            // Assert
            await _trainingService.Received(1).UpdateTrainingAsync(editTrainingDto);
        }

        [Fact]
        public async Task EditTraining_GET_TrainingNotFound_ShowsTempDataAndRedirects()
        {
            // Arrange
            int traineeId = 1;
            int trainerId = 2;
            string species = "Cattle";
            DateTime dateTrained = DateTime.Now;
            string trainingType = "Basic";

            _trainingService.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType)
                .Returns((TrainingDto?)null);

            // Act
            var result = await _controller.EditTraining(traineeId, trainerId, species, dateTrained, trainingType);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewTraining", redirectResult.ActionName);
            Assert.Equal("Invalid data provided for deletion.", _controller.TempData["Message"]);
        }


        [Fact]
        public async Task EditTraining_ModelStateNotValid_ReturnsViewWithModel()
        {
            // Arrange
            var viewModel = new EditTrainingViewModel
            {
                TraineeId = 1,
                TrainerId = 2,
                TrainingType = "Basic",
                TrainingAnimal = "Cattle",
                TrainingDateTime = DateTime.Now,
                TraineeIdOld = 1,
                TrainerIdOld = 2,
                TrainingAnimalOld = "Cattle",
                TrainingTypeOld = "Basic",
                TrainingDateTimeOld = DateTime.Now
            };

            _controller.ModelState.AddModelError("error", "some error");

            var persons = new List<PersonsDto> { new PersonsDto { PersonId = 1, Person = "John" } };
            _trainingService.GetTraineesAsync().Returns(Task.FromResult(persons));

            _staticDropdownService.GetTrainingTypes().Returns(new List<SelectListItem> {
        new SelectListItem { Value = "Basic", Text = "Basic" }
    });

            _staticDropdownService.GetTrainingAnimal().Returns(new List<SelectListItem> {
        new SelectListItem { Value = "Cattle", Text = "Cattle" }
    });

            // Act
            var result = await _controller.EditTraining(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditTrainingViewModel>(viewResult.Model);
            Assert.Equal(viewModel.TraineeIdOld, model.TraineeIdOld);
        }

        [Fact]
        public async Task EditTraining_ModelStateValid_UpdateSuccessful_RedirectsToViewTraining()
        {
            // Arrange
            var viewModel = new EditTrainingViewModel
            {
                TraineeId = 1,
                TrainerId = 2,
                TrainingType = "Basic",
                TrainingAnimal = "Cattle",
                TrainingDateTime = DateTime.Now,
                TraineeIdOld = 1,
                TrainerIdOld = 2,
                TrainingAnimalOld = "Cattle",
                TrainingTypeOld = "Basic",
                TrainingDateTimeOld = DateTime.Now
            };

            var editTrainingDto = new EditTrainingDto
            {
                TraineeId = 1,
                TrainerId = 2,
                TrainingType = "Basic",
                TrainingAnimal = "Cattle",
                TrainingDateTime = viewModel.TrainingDateTime,
                TraineeIdOld = 1,
                TrainerIdOld = 2,
                TrainingAnimalOld = "Cattle",
                TrainingDateTimeOld = viewModel.TrainingDateTimeOld
            };


            _mapper.Map<EditTrainingDto>(viewModel).Returns(editTrainingDto);
            _trainingService.UpdateTrainingAsync(editTrainingDto).Returns("Update successful");

            // Act
            var result = await _controller.EditTraining(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewTraining", redirectResult.ActionName!); // null-forgiving operator
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues!.ContainsKey("selectedTraineeId"));
            Assert.Equal(viewModel.TraineeIdOld, redirectResult.RouteValues["selectedTraineeId"]);
        }
        [Fact]
        public async Task ViewTraining_WhenSelectedTraineeIdIsAll_ReturnsViewWithAllTrainings()
        {
            // Arrange
            var allPersons = new List<PersonsDto> { new PersonsDto { PersonId = 1, Person = "John" } };
            var allTrainee = new List<TraineeTrainerViewModel> { new TraineeTrainerViewModel { PersonID = 1, Person = "John Doe" } };
            var allTrainings = new List<TrainerTrainingDto> { new TrainerTrainingDto { PersonID = 1, TrainingType = "Type1" } };
            var mappedTrainings = new List<TrainingViewModel> { new TrainingViewModel { TraineeId = 1, TrainingType = "Type1" } };

            _personService.GetAllPersonAsync().Returns(allPersons);
            _mapper.Map<IEnumerable<TraineeTrainerViewModel>>(allPersons).Returns(allTrainee);
            _trainingService.GetAllTrainingsAsync().Returns(allTrainings);
            _mapper.Map<IEnumerable<TrainingViewModel>>(allTrainings).Returns(mappedTrainings);

            // Act
            var result = await _controller.ViewTraining("All") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainingListViewModel>(result.Model);
            // Ensure collections are not null before checking their length
            Assert.NotNull(model.FilteredTrainings);
            Assert.NotNull(model.AllTrainees);
            Assert.Single(model.FilteredTrainings);  // Check that there is only one training
            Assert.Single(model.AllTrainees);  // Check that there is only one trainee
            Assert.Equal("All", model.SelectedTraineeId);  // Ensure the "All" option is selected
        }
        [Fact]
        public async Task TrainerHistory_WithDefaultParameters_ReturnsCorrectViewModel()
        {
            // Arrange
            var allPersons = new List<PersonsDto> { new PersonsDto { PersonId = 1, Person = "John" } };
            var historyDto = new List<TrainerHistoryDto> { new TrainerHistoryDto { TrainerID = 1, TrainingAnimal = "Cattle" } };
            var historyModel = new List<TrainingHistoryModel> { new TrainingHistoryModel { TrainerID = 1, TrainingAnimal = "Cattle" } };
            var trainingAnimalList = new List<SelectListItem> { new SelectListItem { Value = "Cattle", Text = "Cattle" } };

            _personService.GetAllPersonAsync().Returns(allPersons);
            _trainingService.GetTrainerHistoryAsync(0, "Cattle").Returns(historyDto);
            _mapper.Map<List<TraineeTrainerViewModel>>(allPersons).Returns(new List<TraineeTrainerViewModel> { new TraineeTrainerViewModel { PersonID = 1, Person = "John Doe" } });
            _mapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);
            _staticDropdownService.GetTrainingAnimal().Returns(trainingAnimalList);

            // Act
            var result = await _controller.TrainerHistory() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainerHistoryViewModel>(result.Model);
            Assert.Equal(0, model.SelectedTrainerId);
            Assert.Equal("Cattle", model.SelectedSpecies);
            Assert.NotNull(model.AllTrainers);
            Assert.Single(model.AllTrainers);
            Assert.NotNull(model.HistoryDetails);
            Assert.Single(model.HistoryDetails);
            Assert.Equal(trainingAnimalList, model.TrainingAnimalList);
            Assert.Equal(2, model.AllTrainersList.Count); // Including "All people" option
        }

        [Fact]
        public async Task TrainerHistory_WithNonDefaultParameters_ReturnsCorrectViewModel()
        {
            // Arrange
            int selectedTrainerId = 1;
            string selectedSpecies = "Sheep";

            var allPersons = new List<PersonsDto> { new PersonsDto { PersonId = 1, Person = "John" } };
            var historyDto = new List<TrainerHistoryDto> { new TrainerHistoryDto { TrainerID = 1, TrainingAnimal = "Sheep" } };
            var historyModel = new List<TrainingHistoryModel> { new TrainingHistoryModel { TrainerID = 1, TrainingAnimal = "Sheep" } };
            var trainingAnimalList = new List<SelectListItem> { new SelectListItem { Value = "Sheep", Text = "Sheep" } };

            _personService.GetAllPersonAsync().Returns(allPersons);
            _trainingService.GetTrainerHistoryAsync(selectedTrainerId, selectedSpecies).Returns(historyDto);
            _mapper.Map<List<TraineeTrainerViewModel>>(allPersons).Returns(new List<TraineeTrainerViewModel> { new TraineeTrainerViewModel { PersonID = 1, Person = "John Doe" } });
            _mapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);
            _staticDropdownService.GetTrainingAnimal().Returns(trainingAnimalList);

            // Act
            var result = await _controller.TrainerHistory(selectedTrainerId, selectedSpecies) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainerHistoryViewModel>(result.Model);
            Assert.Equal(selectedTrainerId, model.SelectedTrainerId);
            Assert.Equal(selectedSpecies, model.SelectedSpecies);
            Assert.NotNull(model.AllTrainers);
            Assert.Single(model.AllTrainers);
            Assert.NotNull(model.HistoryDetails);
            Assert.Single(model.HistoryDetails);
            Assert.Equal(trainingAnimalList, model.TrainingAnimalList);
            Assert.Equal(2, model.AllTrainersList.Count); // Including "All people" option
        }

        [Fact]
        public async Task TrainerHistory_WithNoTrainers_ReturnsEmptyViewModel()
        {
            // Arrange
            var allPersons = new List<PersonsDto>();
            var historyDto = new List<TrainerHistoryDto>();
            var historyModel = new List<TrainingHistoryModel>();
            var trainingAnimalList = new List<SelectListItem>();

            _personService.GetAllPersonAsync().Returns(allPersons);
            _trainingService.GetTrainerHistoryAsync(0, "Cattle").Returns(historyDto);
            _mapper.Map<List<TraineeTrainerViewModel>>(allPersons).Returns(new List<TraineeTrainerViewModel>());
            _mapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);
            _staticDropdownService.GetTrainingAnimal().Returns(trainingAnimalList);

            // Act
            var result = await _controller.TrainerHistory() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainerHistoryViewModel>(result.Model);
            Assert.NotNull(model.AllTrainers);
            Assert.Empty(model.AllTrainers);
            Assert.NotNull(model.HistoryDetails);
            Assert.Empty(model.HistoryDetails);
            Assert.Empty(model.TrainingAnimalList);
            Assert.Single(model.AllTrainersList); // Only "All people" option
        }
        [Fact]
        public async Task TrainerTrained_WhenSelectedTrainerIdIsZero_ReturnsViewWithEmptyTraineeTrainingDetails()
        {
            // Arrange
            var allPersons = new List<PersonsDto> { new PersonsDto { PersonId = 1, Person = "John" } };
            var allTrainers = new List<TrainerTrainedModel> { new TrainerTrainedModel { PersonID = 1, Person = "John Doe" } };

            _personService.GetAllPersonAsync().Returns(allPersons);
            _mapper.Map<List<TrainerTrainedModel>>(allPersons).Returns(allTrainers);

            // Act
            var result = await _controller.TrainerTrained() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainerTrainedViewModel>(result.Model);
            Assert.Equal(0, model.SelectedTrainerId);
            Assert.NotNull(model.TraineeTrainingDetails);
            Assert.Empty(model.TraineeTrainingDetails);
            Assert.Equal(allTrainers, model.AllTrainers);
            Assert.Equal(2, model.TrainerSelectList.Count); // "Select trainer" + 1 trainer
        }

        [Fact]
        public async Task TrainerTrained_WhenSelectedTrainerIdIsValid_ReturnsViewWithTraineeTrainingDetails()
        {
            // Arrange
            int selectedTrainerId = 1;
            var allPersons = new List<PersonsDto> { new PersonsDto { PersonId = 1, Person = "John" } };
            var allTrainers = new List<TrainerTrainedModel> { new TrainerTrainedModel { PersonID = 1, Person = "John Doe" } };
            var trainedList = new List<TrainerTrainedDto> { new TrainerTrainedDto { TraineeNo = 2, Trainee = "Jane Doe" } };

            _personService.GetAllPersonAsync().Returns(allPersons);
            _mapper.Map<List<TrainerTrainedModel>>(allPersons).Returns(allTrainers);
            _trainingService.GetTrainerTrainedAsync(selectedTrainerId).Returns(trainedList);

            // Act
            var result = await _controller.TrainerTrained(selectedTrainerId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainerTrainedViewModel>(result.Model);
            Assert.Equal(selectedTrainerId, model.SelectedTrainerId);
            Assert.Equal(trainedList, model.TraineeTrainingDetails);
            Assert.Equal(allTrainers, model.AllTrainers);
            Assert.Equal(2, model.TrainerSelectList.Count); // "Select trainer" + 1 trainer
        }

        [Fact]
        public async Task TrainerTrained_WhenGetAllPersonAsyncThrowsException_ThrowsException()
        {
            // Arrange
            _personService.GetAllPersonAsync().Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.TrainerTrained());
        }

        [Fact]
        public async Task TrainerTrained_WhenGetTrainerTrainedAsyncThrowsException_ThrowsException()
        {
            // Arrange
            int selectedTrainerId = 1;
            var allPersons = new List<PersonsDto> { new PersonsDto { PersonId = 1, Person = "John" } };

            _personService.GetAllPersonAsync().Returns(allPersons);
            _trainingService.GetTrainerTrainedAsync(selectedTrainerId).Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.TrainerTrained(selectedTrainerId));
        }
        [Fact]
        public async Task DeleteTraining_ValidInput_ReturnsRedirectToActionResult()
        {
            // Arrange
            int traineeId = 1;
            string species = "Cattle";
            DateTime dateTrained = DateTime.Now;

            _trainingService.DeleteTrainingAsync(traineeId, species, dateTrained)
                .Returns("Training deleted successfully");

            // Act
            var result = await _controller.DeleteTraining(traineeId, species, dateTrained);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewTraining", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.Equal(traineeId, redirectResult.RouteValues["selectedTraineeId"]);
            await _trainingService.Received(1).DeleteTrainingAsync(traineeId, species, dateTrained);
            Assert.Equal("Training deleted successfully", _controller.TempData["Message"]);
        }

        [Fact]
        public async Task DeleteTraining_InvalidModelState_ReturnsRedirectToActionResult()
        {
            // Arrange
            int traineeId = 1;
            string species = "Cattle";
            DateTime dateTrained = DateTime.Now;

            _controller.ModelState.AddModelError("Error", "Invalid");

            // Act
            var result = await _controller.DeleteTraining(traineeId, species, dateTrained);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewTraining", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.Equal(traineeId, redirectResult.RouteValues["selectedTraineeId"]);
            Assert.Equal("Invalid data provided for deletion.", _controller.TempData["Message"]);
            await _trainingService.DidNotReceive().DeleteTrainingAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<DateTime>());
        }
    }

}
