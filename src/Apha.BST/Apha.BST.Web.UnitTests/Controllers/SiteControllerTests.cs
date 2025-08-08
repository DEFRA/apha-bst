using System.Reflection;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.DataAccess.Data;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class SiteControllerTests
    {
        private readonly ISiteService _siteService;
        private readonly IMapper _mapper;
        private readonly IUserDataService _userDataService;
        private readonly ILogger<SiteController> _logger;
        private readonly SiteController _controller;
        private static SqlException CreateSqlExceptionForInner()
        {
            // Try to find any non-public constructor and pass dummy values
            var ctors = typeof(SqlException).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
               
                object?[] args = new object?[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    args[i] = parameters[i].ParameterType.IsValueType
                        ? Activator.CreateInstance(parameters[i].ParameterType)
                        : null;
                }
                try
                {
                    return (SqlException)ctor.Invoke(args);
                }
                catch
                {
                    // Ignore and try next
                }
            }
            throw new InvalidOperationException("Could not create SqlException for testing.");
        }

        public SiteControllerTests()
        {
            _siteService = Substitute.For<ISiteService>();
            _mapper = Substitute.For<IMapper>();
            _userDataService = Substitute.For<IUserDataService>();
            _logger = Substitute.For<ILogger<SiteController>>();
            _controller = new SiteController(_logger, _siteService, _mapper, _userDataService);

            // Setup TempData for the controller
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            _controller.TempData = tempData;

            // Setup ControllerContext with ActionDescriptor
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(
                httpContext,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor());
            _controller.ControllerContext = new ControllerContext(actionContext);

            // Setup default permissions
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testUser");
        }

        [Fact]
        public async Task ViewSite_WhenSelectedSiteIsAll_ReturnsAllSites()
        {
            // Arrange
            var siteDtos = new List<SiteDto> {
                new SiteDto { PlantNo = "Site1", Name = "Site 1" },
                new SiteDto { PlantNo = "Site2", Name = "Site 2" }
            };
            var siteViewModels = new List<SiteViewModel> {
                new SiteViewModel { PlantNo = "Site1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "Site2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(siteDtos);
            _mapper.Map<IEnumerable<SiteViewModel>>(siteDtos).Returns(siteViewModels);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.ViewSite("All");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.NotNull(model.FilteredSites);
            Assert.Equal(2, model.FilteredSites?.Count() ?? 0);
            Assert.Equal("All", model.SelectedSite);
            Assert.True(model.CanEdit);
        }

        [Fact]
        public async Task ViewSite_WhenSelectedSiteIsSpecific_ReturnsFilteredSites()
        {
            // Arrange
            var siteDtos = new List<SiteDto> {
                new SiteDto { PlantNo = "Site1", Name = "Site 1" },
                new SiteDto { PlantNo = "Site2", Name = "Site 2" }
            };
            var siteViewModels = new List<SiteViewModel> {
                new SiteViewModel { PlantNo = "Site1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "Site2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(siteDtos);
            _mapper.Map<IEnumerable<SiteViewModel>>(siteDtos).Returns(siteViewModels);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.ViewSite("Site1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.NotNull(model.FilteredSites);
            Assert.Single(model.FilteredSites!);
            Assert.Equal("Site1", model.FilteredSites!.First().PlantNo);
            Assert.Equal("Site1", model.SelectedSite);
            Assert.True(model.CanEdit);
        }

        [Fact]
        public async Task ViewSite_WhenNoSitesAvailable_ReturnsEmptyList()
        {
            // Arrange
            var siteDtos = new List<SiteDto>();
            var siteViewModels = new List<SiteViewModel>();

            _siteService.GetAllSitesAsync("All").Returns(siteDtos);
            _mapper.Map<IEnumerable<SiteViewModel>>(siteDtos).Returns(siteViewModels);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.ViewSite("All");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.NotNull(model.FilteredSites);
            Assert.Equal("All", model.SelectedSite);
            Assert.True(model.CanEdit);
            await _siteService.Received(1).GetAllSitesAsync("All");
            _mapper.Received(1).Map<IEnumerable<SiteViewModel>>(siteDtos);
        }

        [Fact]
        public async Task ViewSite_WithCanEditFalse_ReturnsViewModelWithCanEditFalse()
        {
            // Arrange
            var siteDtos = new List<SiteDto> {
                new SiteDto { PlantNo = "Site1", Name = "Site 1" },
                new SiteDto { PlantNo = "Site2", Name = "Site 2" }
            };
            var siteViewModels = new List<SiteViewModel> {
                new SiteViewModel { PlantNo = "Site1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "Site2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(siteDtos);
            _mapper.Map<IEnumerable<SiteViewModel>>(siteDtos).Returns(siteViewModels);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.ViewSite("All");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.False(model.CanEdit);
        }

        [Fact]
        public async Task AddSite_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            var userName = "testUser";
            _userDataService.GetUsername().Returns(userName);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _siteService.AddSiteAsync(Arg.Any<SiteDto>(), userName).Returns("Site added successfully.");

            // Act
            var result = await _controller.AddSite(siteViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal("Site added successfully.", _controller.TempData["SiteMessage"]);
        }

        [Fact]
        public async Task AddSite_ExceptionThrown_SetsTempDataMessageAndRedirects()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            var userName = "testUser";
            _userDataService.GetUsername().Returns(userName);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);            
            _siteService.AddSiteAsync(Arg.Any<SiteDto>(), userName).Throws(new Exception("Test exception"));

            // Act
            var result = await _controller.AddSite(siteViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal("Save failed", _controller.TempData["SiteMessage"]);
        }

        [Fact]
        public async Task AddSite_SiteAlreadyExists_SetsTempDataMessageAndRedirects()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Existing Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Existing Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            var userName = "testUser";
            _userDataService.GetUsername().Returns(userName);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _siteService.AddSiteAsync(Arg.Any<SiteDto>(), userName).Returns("Site already exists.");

            // Act
            var result = await _controller.AddSite(siteViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal("Site already exists.", _controller.TempData["SiteMessage"]);
        }

        [Fact]
        public async Task AddSite_ValidModel_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            var userName = "testUser";
            _userDataService.GetUsername().Returns(userName);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            await _controller.AddSite(siteViewModel);

            // Assert
            await _siteService.Received(1).AddSiteAsync(
                Arg.Is<SiteDto>(dto => dto.Name == siteViewModel.Name && dto.PlantNo == siteViewModel.PlantNo),
                Arg.Is<string>(user => user == userName));
        }

        [Fact]
        public async Task AddSite_WithCanEditFalse_DoesNotCallAddSiteAsync()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            var userName = "testUser";
            _userDataService.GetUsername().Returns(userName);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            await _controller.AddSite(siteViewModel);

            // Assert
            await _siteService.DidNotReceive().AddSiteAsync(Arg.Any<SiteDto>(), Arg.Any<string>());
        }

        [Fact]
        public async Task AddSite_ValidModel_MapsViewModelToDto()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            await _controller.AddSite(siteViewModel);

            // Assert
            _mapper.Received(1).Map<SiteDto>(Arg.Is<SiteViewModel>(vm =>
                vm.Name == siteViewModel.Name && vm.PlantNo == siteViewModel.PlantNo));
        }

        [Theory]
        [InlineData(null, "123")]
        [InlineData("", "123")]
        [InlineData("Test Site", null)]
        [InlineData("Test Site", "")]
        public async Task AddSite_InvalidModelState_ReturnsViewWithModel(string name, string plantNo)
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = name, PlantNo = plantNo };
            _controller.ModelState.AddModelError("", "Model state is invalid");
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.AddSite(siteViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(siteViewModel, viewResult.Model);
        }
        [Fact]
        public async Task AddSite_WhenSqlExceptionThrown_LogsSqlErrorAndSetsTempData()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            var userName = "testUser";
            _userDataService.GetUsername().Returns(userName);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Create a dummy SqlException for the inner exception
            var sqlException = CreateSqlExceptionForInner();
            var wrapperException = new Exception("Wrapper", sqlException);
            _siteService.AddSiteAsync(Arg.Any<SiteDto>(), userName).Throws(wrapperException);

            // Act
            var result = await _controller.AddSite(siteViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal("Save failed", _controller.TempData["SiteMessage"]);
            _logger.ReceivedWithAnyArgs().LogError(default!, default!, default!, default!);
        }
        [Fact]
        public async Task AddSite_WhenGeneralExceptionThrown_LogsGeneralErrorAndSetsTempData()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            var userName = "testUser";
            _userDataService.GetUsername().Returns(userName);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Create a general exception
            var exception = new Exception("General error");
            _siteService.AddSiteAsync(Arg.Any<SiteDto>(), userName).Throws(exception);

            // Act
            var result = await _controller.AddSite(siteViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal("Save failed", _controller.TempData["SiteMessage"]);
            _logger.ReceivedWithAnyArgs().LogError(default!, default!, default!, default!);
        }
        [Fact]
        public async Task SiteTrainee_WithNullSelectedSite_ReturnsViewModelWithAllSites()
        {
            // Arrange
            var allSitesDto = new List<SiteDto> {
                new SiteDto { PlantNo = "1", Name = "Site 1" },
                new SiteDto { PlantNo = "2", Name = "Site 2" }
            };
            var allSites = new List<SiteViewModel> {
                new SiteViewModel { PlantNo = "1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(allSitesDto);
            _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto).Returns(allSites);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.SiteTrainee();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.NotNull(model.AllSites);
            Assert.Equal(2, model.AllSites?.Count() ?? 0);
            Assert.Null(model.FilteredTrainees);
            Assert.Null(model.SelectedSite);
            Assert.True(model.CanEdit);
        }

        [Fact]
        public async Task SiteTrainee_WithSelectedSite_ReturnsViewModelWithFilteredTrainees()
        {
            // Arrange
            var selectedSite = "1";
            var allSitesDto = new List<SiteDto> {
                new SiteDto { PlantNo = "1", Name = "Site 1" },
                new SiteDto { PlantNo = "2", Name = "Site 2" }
            };
            var allSites = new List<SiteViewModel> {
                new SiteViewModel { PlantNo = "1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "2", Name = "Site 2" }
            };
            var traineeDtos = new List<SiteTraineeDto> {
                new SiteTraineeDto { PersonId = 1, Person = "Trainee 1" },
                new SiteTraineeDto { PersonId = 2, Person = "Trainee 2" }
            };
            var traineeViewModels = new List<SiteTraineeViewModel> {
                new SiteTraineeViewModel { PersonId = 1, Person = "Trainee 1" },
                new SiteTraineeViewModel { PersonId = 2, Person = "Trainee 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(allSitesDto);
            _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto).Returns(allSites);
            _siteService.GetSiteTraineesAsync(selectedSite).Returns(traineeDtos);
            _mapper.Map<IEnumerable<SiteTraineeViewModel>>(traineeDtos).Returns(traineeViewModels);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.SiteTrainee(selectedSite);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.NotNull(model.AllSites);
            Assert.Equal(2, model.AllSites?.Count() ?? 0);
            Assert.NotNull(model.FilteredTrainees);
            Assert.Equal(2, model.FilteredTrainees?.Count() ?? 0);
            Assert.Equal(selectedSite, model.SelectedSite);
            Assert.True(model.CanEdit);
        }

        [Fact]
        public async Task SiteTrainee_WithCanEditFalse_ReturnsViewModelWithCanEditFalse()
        {
            // Arrange
            var allSitesDto = new List<SiteDto> {
                new SiteDto { PlantNo = "1", Name = "Site 1" },
                new SiteDto { PlantNo = "2", Name = "Site 2" }
            };
            var allSites = new List<SiteViewModel> {
                new SiteViewModel { PlantNo = "1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(allSitesDto);
            _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto).Returns(allSites);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.SiteTrainee();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.False(model.CanEdit);
        }

        [Fact]
        public async Task SiteTrainee_CorrectlyPopulatesAllSites()
        {
            // Arrange
            var allSitesDto = new List<SiteDto> {
                new SiteDto { PlantNo = "1", Name = "Site 1" },
                new SiteDto { PlantNo = "2", Name = "Site 2" }
            };
            var allSites = new List<SiteViewModel> {
                new SiteViewModel { PlantNo = "1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(allSitesDto);
            _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto).Returns(allSites);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.SiteTrainee();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.NotNull(model.AllSites);
            Assert.Contains(model.AllSites!, s => s.Value == "1" && s.Text == "Site 1");
            Assert.Contains(model.AllSites!, s => s.Value == "2" && s.Text == "Site 2");
            Assert.True(model.CanEdit);
        }

        [Fact]
        public async Task SiteTrainee_WithSelectedSite_SetsSelectedPropertyInSelectList()
        {
            // Arrange
            var selectedSite = "1";
            var allSitesDto = new List<SiteDto> {
                new SiteDto { PlantNo = "1", Name = "Site 1" },
                new SiteDto { PlantNo = "2", Name = "Site 2" }
            };
            var allSites = new List<SiteViewModel> {
                new SiteViewModel { PlantNo = "1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(allSitesDto);
            _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto).Returns(allSites);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.SiteTrainee(selectedSite);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.NotNull(model.AllSites);
            var selectedItem = model.AllSites!.SingleOrDefault(s => s.Selected);
            Assert.NotNull(selectedItem);
            Assert.Equal(selectedSite, selectedItem.Value);
            Assert.True(model.CanEdit);
        }

        [Fact]
        public async Task DeleteTrainee_ValidModelState_SuccessfulDeletion_ReturnsRedirectToActionResult()
        {
            // Arrange
            int personId = 1;
            string selectedSite = "Site1";
            _siteService.DeleteTraineeAsync(personId).Returns("Trainee deleted successfully");
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedSite"));
            Assert.Equal(selectedSite, redirectResult.RouteValues["selectedSite"]);
            Assert.Equal("Trainee deleted successfully", _controller.TempData["SiteMessage"]);
        }

        [Fact]
        public async Task DeleteTrainee_WithCanEditFalse_DoesNotCallDeleteTraineeAsync()
        {
            // Arrange
            int personId = 1;
            string selectedSite = "Site1";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectResult.RouteValues);
            await _siteService.DidNotReceive().DeleteTraineeAsync(personId);
        }

        [Fact]
        public async Task DeleteTrainee_InvalidModelState_ReturnsRedirectToActionResult()
        {
            // Arrange
            int personId = 1;
            string selectedSite = "Site1";
            _controller.ModelState.AddModelError("error", "Some error");
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedSite"));
            Assert.Equal(selectedSite, redirectResult.RouteValues["selectedSite"]);
            Assert.Equal("Invalid request", _controller.TempData["SiteMessage"]);
        }

        [Fact]
        public async Task DeleteTrainee_ExceptionThrown_ReturnsRedirectToActionResult()
        {
            // Arrange
            int personId = 1;
            string selectedSite = "Site1";
            _siteService.DeleteTraineeAsync(personId).Throws(new Exception("Some error"));
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedSite"));
            Assert.Equal(selectedSite, redirectResult.RouteValues["selectedSite"]);
            Assert.Equal("Save failed", _controller.TempData["SiteMessage"]);
        }


        [Theory]
        [InlineData("Trainee deleted successfully")]
        [InlineData("Trainee not found")]
        public async Task DeleteTrainee_DifferentServiceResults_ReturnsRedirectToActionResult(string serviceMessage)
        {
            // Arrange
            int personId = 1;
            string selectedSite = "Site1";
            _siteService.DeleteTraineeAsync(personId).Returns(serviceMessage);
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedSite"));
            Assert.Equal(selectedSite, redirectResult.RouteValues["selectedSite"]);
            Assert.Equal(serviceMessage, _controller.TempData["SiteMessage"]);
        }
        [Fact]
        public async Task DeleteTrainee_WhenSqlExceptionThrown_LogsSqlErrorAndSetsTempData()
        {
            // Arrange
            int personId = 1;
            string selectedSite = "Site1";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            var sqlException = CreateSqlExceptionForInner();
            var wrapperException = new Exception("Wrapper", sqlException);
            _siteService.DeleteTraineeAsync(personId).Throws(wrapperException);

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.Equal("Save failed", _controller.TempData["SiteMessage"]);
            _logger.ReceivedWithAnyArgs().LogError(default!, default!, default!, default!);

        }
        [Fact]
        public async Task DeleteTrainee_WhenGeneralExceptionThrown_LogsGeneralErrorAndSetsTempData()
        {
            // Arrange
            int personId = 1;
            string selectedSite = "Site1";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Create a general exception
            var exception = new Exception("General error");
            _siteService.DeleteTraineeAsync(personId).Throws(exception);

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.Equal("Save failed", _controller.TempData["SiteMessage"]);
            _logger.ReceivedWithAnyArgs().LogError(default!, default!, default!, default!);

        }

    }
}