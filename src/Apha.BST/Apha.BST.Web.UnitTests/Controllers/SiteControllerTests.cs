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
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class SiteControllerTests
    {
        private readonly ISiteService _siteService;
        private readonly IMapper _mapper;
        private readonly IUserDataService _userDataService;
        private const string status = "Save failed";
        private readonly ILogService _logService;
        private readonly SiteController _controller;

        private static SqlException CreateSqlExceptionForInner()
        {
            // This creates an uninitialized SqlException instance for testing purposes.
            return (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));
        }

        public SiteControllerTests()
        {
            _siteService = Substitute.For<ISiteService>();
            _mapper = Substitute.For<IMapper>();
            _userDataService = Substitute.For<IUserDataService>();
            _logService = Substitute.For<ILogService>();
            _controller = new SiteController(_siteService, _mapper, _userDataService, _logService);

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
            Assert.Equal(status+ ": Test exception", _controller.TempData["SiteMessage"]);
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
            _siteService.AddSiteAsync(Arg.Any<SiteDto>(), userName).Throws(sqlException);

            // Act
            var result = await _controller.AddSite(siteViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal(status+ ": Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.", _controller.TempData["SiteMessage"]);
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
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
            Assert.Equal(status+": General error", _controller.TempData["SiteMessage"]);
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
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
            Assert.Equal(status+ ": Some error", _controller.TempData["SiteMessage"]);
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

            _siteService.DeleteTraineeAsync(personId).Throws(sqlException);

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.Equal(status+ ": Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.", _controller.TempData["SiteMessage"]);
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);

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
            Assert.Equal(status+ ": General error", _controller.TempData["SiteMessage"]);
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);

        }
        [Fact]
        public async Task EditSite_ValidInput_SuccessfulUpdate()
        {
            // Arrange
            var editSiteViewModel = new EditSiteViewModel { PlantNo = "123", Name = "Test Site", AddressLine1 = "a", AddressCounty = "a", IsAhvla = true };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<SiteInputDto>(Arg.Any<EditSiteViewModel>()).Returns(new SiteInputDto
            {
                PlantNo = "123",
                Name = "Test Site",
                IsAhvla = true,
            });
            _siteService.UpdateSiteAsync(Arg.Any<SiteInputDto>()).Returns("Site updated successfully");

            // Act
            var result = await _controller.EditSite(editSiteViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(editSiteViewModel, result.Model);
            Assert.Equal("Site updated successfully", _controller.TempData["SiteMessage"]);
        }

        [Fact]
        public async Task EditSite_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var editSiteViewModel = new EditSiteViewModel
            {
                PlantNo = "123",
                Name = "Test Site",
                AddressLine1 = "a",
                AddressCounty = "a",
                IsAhvla = true
            };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.EditSite(editSiteViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsType<EditSiteViewModel>(result.Model);

            var model = result.Model as EditSiteViewModel;
            Assert.NotNull(model);
            Assert.Equal(editSiteViewModel.PlantNo, model.PlantNo);
            Assert.Equal(editSiteViewModel.Name, model.Name);
            Assert.Equal(editSiteViewModel.AddressLine1, model.AddressLine1);
            Assert.Equal(editSiteViewModel.AddressCounty, model.AddressCounty);
            Assert.Equal(editSiteViewModel.IsAhvla, model.IsAhvla);
            Assert.True(model.CanEdit);
        }

        [Fact]
        public async Task EditSite_SqlException_LogsErrorAndReturnsView()
        {
            // Arrange
            var editSiteViewModel = new EditSiteViewModel
            {
                PlantNo = "123",
                Name = "Test Site",
                AddressLine1 = "a",
                AddressCounty = "a",
                IsAhvla = true
            };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<SiteInputDto>(Arg.Any<EditSiteViewModel>()).Returns(new SiteInputDto
            {
                PlantNo = "123",
                Name = "Test Site",
                IsAhvla = true,
            });
            var sqlException = CreateSqlExceptionForInner();
            _siteService.UpdateSiteAsync(Arg.Any<SiteInputDto>()).Throws(sqlException);

            // Act
            var result = await _controller.EditSite(editSiteViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(editSiteViewModel, result.Model);
            Assert.Equal(status+ ": Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.", _controller.TempData["SiteMessage"]);
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }
        [Fact]
        public async Task EditSite_GeneralException_LogsErrorAndReturnsView()
        {
            // Arrange
            var editSiteViewModel = new EditSiteViewModel
            {
                PlantNo = "123",
                Name = "Test Site",
                AddressLine1 = "a",
                AddressCounty = "a",
                IsAhvla = true
            };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<SiteInputDto>(Arg.Any<EditSiteViewModel>()).Returns(new SiteInputDto
            {
                PlantNo = "123",
                Name = "Test Site",
                IsAhvla = true,
            });
            var exception = new Exception("General error");
            _siteService.UpdateSiteAsync(Arg.Any<SiteInputDto>()).Throws(exception);

            // Act
            var result = await _controller.EditSite(editSiteViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(editSiteViewModel, result.Model);
            Assert.Equal(status+ ": General error", _controller.TempData["SiteMessage"]);

            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);

        }
        [Fact]
        public async Task EditSite_ValidPlantNo_ReturnsViewWithCorrectModel()
        {
            // Arrange
            string plantNo = "123";
            bool canEdit = true;
            var siteDto = new SiteDto { PlantNo = plantNo, Name = "Test Site" };
            var siteDtos = new List<SiteDto> { siteDto };

            _userDataService.CanEditPage(Arg.Any<string>()).Returns(canEdit);
            _siteService.GetAllSitesAsync(plantNo).Returns(siteDtos);

            // Act
            var result = await _controller.EditSite(plantNo);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditSiteViewModel>(viewResult.Model);
            Assert.Equal(plantNo, model.PlantNo);
            Assert.Equal("Test Site", model.Name);
            Assert.Equal(canEdit, model.CanEdit);
        }

        [Fact]
        public async Task EditSite_NullOrEmptyPlantNo_RedirectsToViewSite()
        {
            // Act
            var plantNo = "123";
            var result = await _controller.EditSite(plantNo);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewSite", redirectResult.ActionName);
        }

        [Fact]
        public async Task EditSite_InvalidPlantNo_RedirectsToViewSiteWithMessage()
        {
            // Arrange
            string plantNo = "invalid";
            _siteService.GetAllSitesAsync(plantNo).Returns(new List<SiteDto>());

            // Act
            var result = await _controller.EditSite(plantNo);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewSite", redirectResult.ActionName);
            Assert.Equal("Invalid data provided for editing.", _controller.TempData["SiteMessage"]);
        }

        [Fact]
        public async Task EditSite_ValidPlantNo_SetsIsAhvlaCorrectly()
        {
            // Arrange
            string plantNo = "123";
            var siteDto = new SiteDto { PlantNo = plantNo, Name = "name", Ahvla = "AHVLA" };
            var siteDtos = new List<SiteDto> { siteDto };

            _siteService.GetAllSitesAsync(plantNo).Returns(siteDtos);

            // Act
            var result = await _controller.EditSite(plantNo);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditSiteViewModel>(viewResult.Model);
            Assert.True(model.IsAhvla);
        }

        [Fact]
        public async Task EditSite_ValidPlantNo_SetsIsAhvlaFalseForNonAhvla()
        {
            // Arrange
            string plantNo = "123";

            var siteDto = new SiteDto
            {
                PlantNo = plantNo,
                Name = "Test Site",
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressTown = "Town",
                AddressCounty = "County",
                AddressPostCode = "12345",
                Telephone = "123-456-7890",
                Fax = "098-765-4321",
                Ahvla = "Non-AHVLA"
            };
            var siteDtos = new List<SiteDto> { siteDto };

            _siteService.GetAllSitesAsync(plantNo).Returns(siteDtos);

            // Act
            var result = await _controller.EditSite(plantNo);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditSiteViewModel>(viewResult.Model);
            Assert.False(model.IsAhvla);
        }

        [Fact]
        public async Task EditSite_ValidPlantNo_CallsDependenciesWithCorrectParameters()
        {
            // Arrange
            string plantNo = "123";

            // Act
            await _controller.EditSite(plantNo);

            // Assert
            await _userDataService.Received(1).CanEditPage(Arg.Any<string>());
            await _siteService.Received(1).GetAllSitesAsync(plantNo);
        }

        [Fact]
        public async Task EditSite_ValidPlantNo_SetsAllPropertiesCorrectly()
        {
            // Arrange
            string plantNo = "123";
            var siteDto = new SiteDto
            {
                PlantNo = plantNo,
                Name = "Test Site",
                AddressLine1 = "Address 1",
                AddressLine2 = "Address 2",
                AddressTown = "Town",
                AddressCounty = "County",
                AddressPostCode = "12345",
                Telephone = "123-456-7890",
                Fax = "098-765-4321",
                Ahvla = "AHVLA"
            };
            var siteDtos = new List<SiteDto> { siteDto };

            _siteService.GetAllSitesAsync(plantNo).Returns(siteDtos);

            // Act
            var result = await _controller.EditSite(plantNo);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditSiteViewModel>(viewResult.Model);
            Assert.Equal(plantNo, model.PlantNo);
            Assert.Equal("Test Site", model.Name);
            Assert.Equal("Address 1", model.AddressLine1);
            Assert.Equal("Address 2", model.AddressLine2);
            Assert.Equal("Town", model.AddressTown);
            Assert.Equal("County", model.AddressCounty);
            Assert.Equal("12345", model.AddressPostCode);
            Assert.Equal("123-456-7890", model.Telephone);
            Assert.Equal("098-765-4321", model.Fax);
            Assert.True(model.IsAhvla);
        }
        //new

        [Fact]
        public async Task AddSite_GET_ReturnsViewWithEmptyModel()
        {
            // Arrange
            _controller.ControllerContext.ActionDescriptor.ActionName = "AddSite";
            _userDataService.CanEditPage("AddSite").Returns(true);

            // Act
            var result = await _controller.AddSite();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteViewModel>(viewResult.Model);

            Assert.Equal(string.Empty, model.PlantNo);
            Assert.Equal(string.Empty, model.Name);
            Assert.True(model.CanEdit);

            await _userDataService.Received(1).CanEditPage("AddSite");
        }

        [Fact]
        public async Task AddSite_GET_UserCannotEdit_ReturnsViewWithCanEditFalse()
        {
            // Arrange
            _controller.ControllerContext.ActionDescriptor.ActionName = "AddSite";
            _userDataService.CanEditPage("AddSite").Returns(false);

            // Act
            var result = await _controller.AddSite();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteViewModel>(viewResult.Model);

            Assert.Equal(string.Empty, model.PlantNo);
            Assert.Equal(string.Empty, model.Name);
            Assert.False(model.CanEdit);

            await _userDataService.Received(1).CanEditPage("AddSite");
        }
    }
}