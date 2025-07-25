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

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class TrainingControllerTests
    {
        private readonly ITrainingService _mockTrainingService;
        private readonly IPersonsRepository _mockPersonRepository;
        private readonly IMapper _mockMapper;
        private readonly TrainingController _controller;

        public TrainingControllerTests()
        {
            _mockTrainingService = Substitute.For<ITrainingService>();
            _mockPersonRepository = Substitute.For<IPersonsRepository>();
            _mockMapper = Substitute.For<IMapper>();
            //_controller = new TrainingController(_mockTrainingService, _mockMapper);
            _controller = new TrainingController(_mockTrainingService, _mockMapper,_mockPersonRepository);
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

    }

}
