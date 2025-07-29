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

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class TrainingControllerTests
    {
        private readonly ITrainingService _mockTrainingService;
        private readonly IPersonsRepository _mockPersonRepository;
        private readonly IMapper _mockMapper;
        private readonly TrainingController _controller;
        private readonly ILogger<TrainingController> _logger;

        public TrainingControllerTests()
        {
            _mockTrainingService = Substitute.For<ITrainingService>();
            _mockPersonRepository = Substitute.For<IPersonsRepository>();
            _mockMapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<TrainingController>>();
            //_controller = new TrainingController(_mockTrainingService, _mockMapper);
            _controller = new TrainingController(_mockTrainingService, _mockMapper,_mockPersonRepository,_logger);
            // Setup TempData for the controller
            _controller.TempData = Substitute.For<ITempDataDictionary>();
        }

        [Fact]
        public async Task AddTraining_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var viewModel = new AddTrainingViewModel();
            _controller.ModelState.AddModelError("Error", "Model error");

            _mockTrainingService.GetTraineesAsync().Returns(new List<PersonsDTO>
        {
            new PersonsDTO { PersonId = 1, Person = "John Doe" }
        });

            // Act
            var result = await _controller.AddTraining(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedModel = Assert.IsType<AddTrainingViewModel>(viewResult.Model);
            Assert.Equal(viewModel, returnedModel);
            Assert.Single(returnedModel.Persons);
        }

        [Fact]
        public async Task AddTraining_ValidModelState_RedirectsToAddTraining()
        {
            // Arrange
            var viewModel = new AddTrainingViewModel { PersonId = 1, TrainingAnimal = "Dog" };
            var dto = new TrainingDTO();
            _mockMapper.Map<TrainingDTO>(viewModel).Returns(dto);
            _mockTrainingService.AddTrainingAsync(dto).Returns("Training added successfully");

            // Act
            var result = await _controller.AddTraining(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(TrainingController.AddTraining), redirectResult.ActionName);
            Assert.Equal("Training added successfully", _controller.TempData["Message"]);
            await _mockTrainingService.Received(1).AddTrainingAsync(dto);
        }

        [Fact]
        public async Task AddTraining_ValidModelState_SuccessfulAddition()
        {
            // Arrange
            var viewModel = new AddTrainingViewModel
            {
                TrainerId = 1,
                TrainingAnimal = "Dog",
                TrainingDateTime = DateTime.Now
            };

            var dto = new TrainingDTO();
            _mockMapper.Map<TrainingDTO>(viewModel).Returns(dto);
            _mockTrainingService.AddTrainingAsync(dto).Returns("Training added successfully");

            // Act
            var result = await _controller.AddTraining(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(TrainingController.AddTraining), redirectResult.ActionName);
            Assert.Equal("Training added successfully", _controller.TempData["Message"]);
        }

        [Fact]
        public async Task AddTraining_InvalidModelState_ReturnsViewWithErrors()
        {
            // Arrange
            var viewModel = new AddTrainingViewModel();
            _controller.ModelState.AddModelError("Species", "Species is required");

            var trainees = new List<TraineeDTO>
        {
            new TraineeDTO { PersonId = 1, Person = "John Doe" }
        };
            _mockTrainingService.GetTraineesAsync().Returns(Task.FromResult(new List<PersonsDTO>
{
    new PersonsDTO { PersonId = 1, Person = "John Doe" }
}));


            // Act
            var result = await _controller.AddTraining(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
            Assert.NotNull((viewResult.Model as AddTrainingViewModel).Persons);
            Assert.Single((viewResult.Model as AddTrainingViewModel).Persons);
        }

        [Fact]
        public async Task AddTraining_ExceptionThrown_ReturnsSaveFailedMessage()
        {
            // Arrange
            var viewModel = new AddTrainingViewModel
            {
                TrainerId = 1,
                TrainingAnimal = "Cat",
                TrainingDateTime = DateTime.Now
            };

            var dto = new TrainingDTO();
            _mockMapper.Map<TrainingDTO>(viewModel).Returns(dto);
            _mockTrainingService.AddTrainingAsync(dto).Throws(new Exception("Database error"));

            // Act
            var result = await _controller.AddTraining(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(TrainingController.AddTraining), redirectResult.ActionName);
            Assert.Equal("Save failed", _controller.TempData["Message"]);
        }


        //[Fact]
        //public async Task AddTraining_ExceptionThrown_ReturnsViewWithError()
        //{
        //    // Arrange
        //    var viewModel = new AddTrainingViewModel { PersonId = 1, TrainingAnimal = "Dog" };
        //    _mockMapper.Map<TrainingDTO>(viewModel).Returns(new TrainingDTO());
        //    _mockTrainingService.AddTrainingAsync(Arg.Any<TrainingDTO>()).Throws(new System.Exception("Test exception"));

        //    // Act
        //    var result = await _controller.AddTraining(viewModel);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal(viewModel, viewResult.Model);
        //    Assert.Equal("An error occurred while adding the training.", _controller.TempData["Message"]);
        //}

        //[Fact]
        //public async Task AddTraining_NullViewModel_ReturnsBadRequest()
        //{
        //    // Arrange
        //    AddTrainingViewModel viewModel = null;

        //    // Act
        //    var result = await _controller.AddTraining(viewModel);

        //    // Assert
        //    Assert.IsType<BadRequestResult>(result);
        //}
        [Fact]
        public async Task ViewTrainer_WhenSelectedTraineeIdIsAll_ReturnsAllTrainings()
        {
            // Arrange
            var personList = new List<Persons> { new Persons { PersonId = 1, Person = "John Doe" } };
            var traineeVMs = new List<TraineeViewModel> { new TraineeViewModel {PersonID = 1, Person = "John Doe" } };
            var trainingDTOs = new List<TrainerTrainingDTO> { new TrainerTrainingDTO() };
            var trainingVMs = new List<TrainingViewModel> { new TrainingViewModel() };

            _mockPersonRepository.GetAllAsync().Returns(Task.FromResult<IEnumerable<Persons>>(personList));
            _mockMapper.Map<IEnumerable<TraineeViewModel>>(personList).Returns(traineeVMs);
            _mockTrainingService.GetAllTrainingsAsync().Returns(Task.FromResult<IEnumerable<TrainerTrainingDTO>>(trainingDTOs));
            _mockMapper.Map<IEnumerable<TrainingViewModel>>(trainingDTOs).Returns(trainingVMs);

            // Act
            var result = await _controller.ViewTrainer("All") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainingListViewModel>(result.Model);
            Assert.Equal("All", model.SelectedTraineeId);
            Assert.Equal(traineeVMs, model.AllTrainees);
            Assert.Equal(trainingVMs, model.FilteredTrainings);
        }

        [Fact]
        public async Task ViewTrainer_WhenSelectedTraineeIdIsSpecific_ReturnsFilteredTrainings()
        {
            // Arrange
            var traineeId = "1";
            var personList = new List<Persons> { new Persons { PersonId = 1, Person = "John Doe" } };
            var traineeVMs = new List<TraineeViewModel> { new TraineeViewModel { PersonID = 1, Person = "John Doe" } };
            var trainingDTOs = new List<TrainerTrainingDTO> { new TrainerTrainingDTO() };
            var trainingVMs = new List<TrainingViewModel> { new TrainingViewModel() };

            _mockPersonRepository.GetAllAsync().Returns(Task.FromResult<IEnumerable<Persons>>(personList));
            _mockMapper.Map<IEnumerable<TraineeViewModel>>(personList).Returns(traineeVMs);
            _mockTrainingService.GetTrainingByTraineeAsync(traineeId).Returns(Task.FromResult<IEnumerable<TrainerTrainingDTO>>(trainingDTOs));
            _mockMapper.Map<IEnumerable<TrainingViewModel>>(trainingDTOs).Returns(trainingVMs);

            // Act
            var result = await _controller.ViewTrainer(traineeId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainingListViewModel>(result.Model);
            Assert.Equal(traineeId, model.SelectedTraineeId);
            Assert.Equal(traineeVMs, model.AllTrainees);
            Assert.Equal(trainingVMs, model.FilteredTrainings);
        }

        [Fact]
        public async Task ViewTrainer_WhenNoTrainingsFound_ReturnsEmptyFilteredTrainings()
        {
            // Arrange
            var traineeId = "1";
            var personList = new List<Persons> { new Persons { PersonId = 1, Person = "John Doe" } };
            var traineeVMs = new List<TraineeViewModel> { new TraineeViewModel { PersonID = 1, Person = "John Doe" } };

            _mockPersonRepository.GetAllAsync().Returns(Task.FromResult<IEnumerable<Persons>>(personList));
            _mockMapper.Map<IEnumerable<TraineeViewModel>>(personList).Returns(traineeVMs);
            _mockTrainingService.GetTrainingByTraineeAsync(traineeId).Returns(Task.FromResult<IEnumerable<TrainerTrainingDTO>>(new List<TrainerTrainingDTO>()));
            _mockMapper.Map<IEnumerable<TrainingViewModel>>(Arg.Any<IEnumerable<TrainerTrainingDTO>>()).Returns(new List<TrainingViewModel>());

            // Act
            var result = await _controller.ViewTrainer(traineeId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainingListViewModel>(result.Model);
            Assert.Equal(traineeId, model.SelectedTraineeId);
            Assert.Equal(traineeVMs, model.AllTrainees);
            Assert.Empty(model.FilteredTrainings);
        }

        [Fact]
        public async Task ViewTrainer_WhenNoTraineesFound_ReturnsEmptyAllTrainees()
        {
            // Arrange
            var emptyPersonList = new List<Persons>();
            var emptyTrainees = new List<TraineeViewModel>();
            var emptyTrainings = new List<TrainerTrainingDTO>();
            var emptyTrainingVMs = new List<TrainingViewModel>();

            _mockPersonRepository.GetAllAsync().Returns(Task.FromResult<IEnumerable<Persons>>(emptyPersonList));
            _mockMapper.Map<IEnumerable<TraineeViewModel>>(emptyPersonList).Returns(emptyTrainees);
            _mockTrainingService.GetAllTrainingsAsync().Returns(Task.FromResult<IEnumerable<TrainerTrainingDTO>>(emptyTrainings));
            _mockMapper.Map<IEnumerable<TrainingViewModel>>(emptyTrainings).Returns(emptyTrainingVMs);

            // Act
            var result = await _controller.ViewTrainer("All") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<TrainingListViewModel>(result.Model);
            Assert.Equal("All", model.SelectedTraineeId);
            Assert.Empty(model.AllTrainees);
            Assert.Empty(model.FilteredTrainings);
        }

        [Fact]
        public async Task TrainerHistory_WithDefaultParameters_ReturnsCorrectViewModel()
        {
            var persons = new List<Persons> { new Persons { PersonId = 1, Person = "John Doe" } };
            var historyDto = new List<TrainerHistoryDTO> { new TrainerHistoryDTO() };
            var historyModel = new List<TrainingHistoryModel> { new TrainingHistoryModel() };

            _mockPersonRepository.GetAllAsync().Returns(persons);
            _mockTrainingService.GetTrainerHistoryAsync(0, "Cattle").Returns(historyDto);
            _mockMapper.Map<List<TraineeViewModel>>(persons).Returns(new List<TraineeViewModel> { new TraineeViewModel { PersonID = 1, Person = "John Doe" } });
            _mockMapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);

            var result = await _controller.TrainerHistory();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TrainerHistoryViewModel>(viewResult.Model);
            Assert.Equal(0, model.SelectedTrainerId);
            Assert.Equal("Cattle", model.SelectedSpecies);
            Assert.Single(model.AllTrainers);
            Assert.Single(model.HistoryDetails);
        }

        [Fact]
        public async Task TrainerHistory_WithCustomParameters_ReturnsCorrectViewModel()
        {
            var persons = new List<Persons> { new Persons { PersonId = 2, Person = "Jane Smith" } };
            var historyDto = new List<TrainerHistoryDTO> { new TrainerHistoryDTO() };
            var historyModel = new List<TrainingHistoryModel> { new TrainingHistoryModel() };

            _mockPersonRepository.GetAllAsync().Returns(persons);
            _mockTrainingService.GetTrainerHistoryAsync(2, "Sheep").Returns(historyDto);
            _mockMapper.Map<List<TraineeViewModel>>(persons).Returns(new List<TraineeViewModel> { new TraineeViewModel { PersonID = 2, Person = "Jane Smith" } });
            _mockMapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);

            var result = await _controller.TrainerHistory(2, "Sheep");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TrainerHistoryViewModel>(viewResult.Model);
            Assert.Equal(2, model.SelectedTrainerId);
            Assert.Equal("Sheep", model.SelectedSpecies);
            Assert.Single(model.AllTrainers);
            Assert.Single(model.HistoryDetails);
        }

        [Fact]
        public async Task TrainerHistory_WithNoTrainers_ReturnsEmptyTrainerList()
        {
            var persons = new List<Persons>();
            var historyDto = new List<TrainerHistoryDTO>();
            var historyModel = new List<TrainingHistoryModel>();

            _mockPersonRepository.GetAllAsync().Returns(persons);
            _mockTrainingService.GetTrainerHistoryAsync(0, "Cattle").Returns(historyDto);
            _mockMapper.Map<List<TraineeViewModel>>(persons).Returns(new List<TraineeViewModel>());
            _mockMapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);

            var result = await _controller.TrainerHistory();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TrainerHistoryViewModel>(viewResult.Model);
            Assert.Empty(model.AllTrainers);
        }

        [Fact]
        public async Task TrainerHistory_WithNoHistory_ReturnsEmptyHistoryList()
        {
            var persons = new List<Persons> { new Persons { PersonId = 1, Person = "John Doe" } };
            var historyDto = new List<TrainerHistoryDTO>();
            var historyModel = new List<TrainingHistoryModel>();

            _mockPersonRepository.GetAllAsync().Returns(persons);
            _mockTrainingService.GetTrainerHistoryAsync(1, "Cattle").Returns(historyDto);
            _mockMapper.Map<List<TraineeViewModel>>(persons).Returns(new List<TraineeViewModel> { new TraineeViewModel { PersonID = 1, Person = "John Doe" } });
            _mockMapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);

            var result = await _controller.TrainerHistory(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TrainerHistoryViewModel>(viewResult.Model);
            Assert.Empty(model.HistoryDetails);
        }

        [Fact]
        public async Task TrainerHistory_CallsRepositoryAndServiceMethods()
        {
            var persons = new List<Persons> { new Persons { PersonId = 1, Person = "John Doe" } };
            var historyDto = new List<TrainerHistoryDTO> { new TrainerHistoryDTO() };
            var historyModel = new List<TrainingHistoryModel> { new TrainingHistoryModel() };

            _mockPersonRepository.GetAllAsync().Returns(persons);
            _mockTrainingService.GetTrainerHistoryAsync(1, "Cattle").Returns(historyDto);
            _mockMapper.Map<List<TraineeViewModel>>(persons).Returns(new List<TraineeViewModel> { new TraineeViewModel { PersonID = 1, Person = "John Doe" } });
            _mockMapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);

            await _controller.TrainerHistory(1);

            await _mockPersonRepository.Received(1).GetAllAsync();
            await _mockTrainingService.Received(1).GetTrainerHistoryAsync(1, "Cattle");
            _mockMapper.Received(1).Map<List<TraineeViewModel>>(persons);
            _mockMapper.Received(1).Map<List<TrainingHistoryModel>>(historyDto);
        }

        [Fact]
        public async Task TrainerHistory_ReturnsViewResult()
        {
            var persons = new List<Persons> { new Persons { PersonId = 1, Person = "John Doe" } };
            var historyDto = new List<TrainerHistoryDTO> { new TrainerHistoryDTO() };
            var historyModel = new List<TrainingHistoryModel> { new TrainingHistoryModel() };

            _mockPersonRepository.GetAllAsync().Returns(persons);
            _mockTrainingService.GetTrainerHistoryAsync(1, "Cattle").Returns(historyDto);
            _mockMapper.Map<List<TraineeViewModel>>(persons).Returns(new List<TraineeViewModel> { new TraineeViewModel { PersonID = 1, Person = "John Doe" } });
            _mockMapper.Map<List<TrainingHistoryModel>>(historyDto).Returns(historyModel);

            var result = await _controller.TrainerHistory(1);

            Assert.IsType<ViewResult>(result);
        }



    }

}
