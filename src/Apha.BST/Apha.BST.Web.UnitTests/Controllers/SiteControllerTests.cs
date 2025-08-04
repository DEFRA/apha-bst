using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.DataAccess.Data;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private readonly SiteController _controller;

        public SiteControllerTests()
        {
            _siteService = Substitute.For<ISiteService>();
            _mapper = Substitute.For<IMapper>();
            _controller = new SiteController(_siteService, _mapper);
            // Setup TempData for the controller
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            _controller.TempData = tempData;
        }

        [Fact]
        public async Task ViewSite_WhenSelectedSiteIsAll_ReturnsAllSites()
        {
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

            var result = await _controller.ViewSite("All");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.NotNull(model.FilteredSites);
            Assert.Equal(2, model.FilteredSites?.Count() ?? 0);
            Assert.Equal("All", model.SelectedSite);
        }

        [Fact]
        public async Task ViewSite_WhenSelectedSiteIsSpecific_ReturnsFilteredSites()
        {
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

            var result = await _controller.ViewSite("Site1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.NotNull(model.FilteredSites);
            Assert.Single(model.FilteredSites!);
            Assert.Equal("Site1", model.FilteredSites!.First().PlantNo);
            Assert.Equal("Site1", model.SelectedSite);
        }

        [Fact]
        public async Task ViewSite_WhenNoSitesAvailable_ReturnsEmptyList()
        {
            // Arrange
            var siteDtos = new List<SiteDto>();
            var siteViewModels = new List<SiteViewModel>();

            _siteService.GetAllSitesAsync("All").Returns(siteDtos);
            _mapper.Map<IEnumerable<SiteViewModel>>(siteDtos).Returns(siteViewModels);

            // Act
            var result = await _controller.ViewSite("All");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.NotNull(model.FilteredSites);
            Assert.Equal("All", model.SelectedSite);
            await _siteService.Received(1).GetAllSitesAsync("All");
            _mapper.Received(1).Map<IEnumerable<SiteViewModel>>(siteDtos);
        }
        [Fact]
        public async Task AddSite_ValidModel_ReturnsRedirectToActionResult()
        {
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            _siteService.AddSiteAsync(Arg.Any<SiteDto>()).Returns("Site added successfully.");

            var result = await _controller.AddSite(siteViewModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal("Site added successfully.", _controller.TempData["Message"]);
        }

        [Fact]
        public async Task AddSite_ExceptionThrown_SetsTempDataMessageAndRedirects()
        {
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            _siteService.AddSiteAsync(Arg.Any<SiteDto>()).Throws(new System.Exception("Test exception"));

            var result = await _controller.AddSite(siteViewModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal("Save failed", _controller.TempData["Message"]);
        }

        [Fact]
        public async Task AddSite_SiteAlreadyExists_SetsTempDataMessageAndRedirects()
        {
            var siteViewModel = new SiteViewModel { Name = "Existing Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Existing Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);
            _siteService.AddSiteAsync(Arg.Any<SiteDto>()).Returns("Site already exists.");

            var result = await _controller.AddSite(siteViewModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddSite", redirectResult.ActionName);
            Assert.Equal("Site already exists.", _controller.TempData["Message"]);
        }

        [Fact]
        public async Task AddSite_ValidModel_CallsServiceWithCorrectParameters()
        {
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDto>(siteViewModel).Returns(siteDto);

            await _controller.AddSite(siteViewModel);

            await _siteService.Received(1).AddSiteAsync(Arg.Is<SiteDto>(dto =>
                dto.Name == siteViewModel.Name && dto.PlantNo == siteViewModel.PlantNo));
        }

        [Fact]
        public async Task AddSite_ValidModel_MapsViewModelToDto()
        {
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };

            await _controller.AddSite(siteViewModel);

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
            var siteViewModel = new SiteViewModel { Name = name, PlantNo = plantNo };
            _controller.ModelState.AddModelError("", "Model state is invalid");

            var result = await _controller.AddSite(siteViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(siteViewModel, viewResult.Model);
        }

        [Fact]
        public async Task SiteTrainee_WithNullSelectedSite_ReturnsViewModelWithAllSites()
        {
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

            var result = await _controller.SiteTrainee();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.NotNull(model.AllSites);
            Assert.Equal(2, model.AllSites?.Count() ?? 0);
            Assert.Null(model.FilteredTrainees);
            Assert.Null(model.SelectedSite);
        }

        [Fact]
        public async Task SiteTrainee_WithSelectedSite_ReturnsViewModelWithFilteredTrainees()
        {
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

            var result = await _controller.SiteTrainee(selectedSite);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.NotNull(model.AllSites);
            Assert.Equal(2, model.AllSites?.Count() ?? 0);
            Assert.NotNull(model.FilteredTrainees);
            Assert.Equal(2, model.FilteredTrainees?.Count() ?? 0);
            Assert.Equal(selectedSite, model.SelectedSite);
        }

        [Fact]
        public async Task SiteTrainee_CorrectlyPopulatesAllSites()
        {
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

            var result = await _controller.SiteTrainee();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.NotNull(model.AllSites);
            Assert.Contains(model.AllSites!, s => s.Value == "1" && s.Text == "Site 1");
            Assert.Contains(model.AllSites!, s => s.Value == "2" && s.Text == "Site 2");
        }

        [Fact]
        public async Task SiteTrainee_WithSelectedSite_SetsSelectedPropertyInSelectList()
        {
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

            var result = await _controller.SiteTrainee(selectedSite);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
            Assert.NotNull(model.AllSites);
            var selectedItem = model.AllSites!.SingleOrDefault(s => s.Selected);
            Assert.NotNull(selectedItem);
            Assert.Equal(selectedSite, selectedItem.Value);
        }
        [Fact]
        public async Task DeleteTrainee_ValidModelState_SuccessfulDeletion_ReturnsRedirectToActionResult()
        {
            int personId = 1;
            string selectedSite = "Site1";
            _siteService.DeleteTraineeAsync(personId).Returns("Trainee deleted successfully");

            var result = await _controller.DeleteTrainee(personId, selectedSite);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedSite"));
            Assert.Equal(selectedSite, redirectResult.RouteValues["selectedSite"]);
            Assert.Equal("Trainee deleted successfully", _controller.TempData["message"]);
        }

        [Fact]
        public async Task DeleteTrainee_InvalidModelState_ReturnsRedirectToActionResult()
        {
            int personId = 1;
            string selectedSite = "Site1";
            _controller.ModelState.AddModelError("error", "Some error");

            var result = await _controller.DeleteTrainee(personId, selectedSite);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedSite"));
            Assert.Equal(selectedSite, redirectResult.RouteValues["selectedSite"]);
            Assert.Equal("Invalid request", _controller.TempData["message"]);
        }

        [Fact]
        public async Task DeleteTrainee_ExceptionThrown_ReturnsRedirectToActionResult()
        {
            // Arrange
            int personId = 1;
            string selectedSite = "Site1";
            _siteService.DeleteTraineeAsync(personId).Returns(Task.FromException<string>(new Exception("Some error")));

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedSite"));
            Assert.Equal(selectedSite, redirectResult.RouteValues["selectedSite"]);
            Assert.Equal("Save failed", _controller.TempData["message"]);
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

            // Act
            var result = await _controller.DeleteTrainee(personId, selectedSite);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SiteTrainee", redirectResult.ActionName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.True(redirectResult.RouteValues.ContainsKey("selectedSite"));
            Assert.Equal(selectedSite, redirectResult.RouteValues["selectedSite"]);
            Assert.Equal(serviceMessage, _controller.TempData["message"]);
        }
    }
}