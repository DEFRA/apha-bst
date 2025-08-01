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
            // Arrange
            var siteDtos = new List<SiteDto>
            {
                new SiteDto { PlantNo = "Site1", Name = "Site 1" },
                new SiteDto { PlantNo = "Site2", Name = "Site 2" }
            };
            var siteViewModels = new List<SiteViewModel>
            {
                new SiteViewModel { PlantNo = "Site1", Name = "Site 1" },
                new SiteViewModel { PlantNo = "Site2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(siteDtos);
            _mapper.Map<IEnumerable<SiteViewModel>>(siteDtos).Returns(siteViewModels);

            // Act
            var result = await _controller.ViewSite("All");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.Equal(2, model.FilteredSites.Count());
            Assert.Equal("All", model.SelectedSite);
            await _siteService.Received(1).GetAllSitesAsync("All");
            _mapper.Received(1).Map<IEnumerable<SiteViewModel>>(siteDtos);
        }

        [Fact]
        public async Task ViewSite_WhenSelectedSiteIsSpecific_ReturnsFilteredSites()
        {
            // Arrange
            var siteDtos = new List<SiteDto>
            {
                new SiteDto { PlantNo = "Site1", Name = "Site 1" },
                new SiteDto { PlantNo = "Site2", Name = "Site 2" }
            };
            var siteViewModels = new List<SiteViewModel>
            {
                new SiteViewModel { PlantNo = "Site1", Name = "Site 1" },
                    new SiteViewModel { PlantNo = "Site2", Name = "Site 2" }
            };

            _siteService.GetAllSitesAsync("All").Returns(siteDtos);
            _mapper.Map<IEnumerable<SiteViewModel>>(siteDtos).Returns(siteViewModels);

            // Act
            var result = await _controller.ViewSite("Site1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SiteListViewModel>(viewResult.Model);
            Assert.Single(model.FilteredSites);
            Assert.Equal("Site1", model.FilteredSites.First().PlantNo);
            Assert.Equal("Site1", model.SelectedSite);
            await _siteService.Received(1).GetAllSitesAsync("All");
            _mapper.Received(1).Map<IEnumerable<SiteViewModel>>(siteDtos);
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
            Assert.Empty(model.FilteredSites);
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

        //[Fact]
        //public async Task SiteTrainee_WhenSelectedSiteIsAll_ReturnsAllSitesWithEmptyTrainees()
        //{
        //    // Arrange
        //    var allSitesDto = new List<SiteDTO> { new SiteDTO { PlantNo = "Site1" }, new SiteDTO { PlantNo = "Site2" } };
        //    var allSitesViewModel = new List<SiteViewModel> { new SiteViewModel { PlantNo = "Site1" }, new SiteViewModel { PlantNo = "Site2" } };

        //    _siteService.GetAllSitesAsync("All").Returns(allSitesDto);
        //    _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto).Returns(allSitesViewModel);

        //    // Act
        //    var result = await _controller.SiteTrainee("All");

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
        //    Assert.Equal("All", model.SelectedSite);
        //    Assert.Equal(allSitesViewModel, model.AllSites);
        //    Assert.Empty(model.FilteredTrainees);
        //}

        //[Fact]
        //public async Task SiteTrainee_WhenSelectedSiteIsSpecific_ReturnsFilteredTrainees()
        //{
        //    // Arrange
        //    var selectedSite = "Site1";
        //    var allSitesDto = new List<SiteDTO> { new SiteDTO { PlantNo = "Site1" }, new SiteDTO { PlantNo = "Site2" } };
        //    var allSitesViewModel = new List<SiteViewModel> { new SiteViewModel { PlantNo = "Site1" }, new SiteViewModel { PlantNo = "Site2" } };
        //    var traineeDto = new List<SiteTraineeDTO> { new SiteTraineeDTO { PersonId = 1 } };
        //    var traineeViewModel = new List<SiteTraineeViewModel> { new SiteTraineeViewModel { PersonId = 1 } };

        //    _siteService.GetAllSitesAsync("All").Returns(allSitesDto);
        //    _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto).Returns(allSitesViewModel);
        //    _siteService.GetSiteTraineesAsync(selectedSite).Returns(traineeDto);
        //    _mapper.Map<IEnumerable<SiteTraineeViewModel>>(traineeDto).Returns(traineeViewModel);

        //    // Act
        //    var result = await _controller.SiteTrainee(selectedSite);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
        //    Assert.Equal(selectedSite, model.SelectedSite);
        //    Assert.Equal(allSitesViewModel, model.AllSites);
        //    Assert.Equal(traineeViewModel, model.FilteredTrainees);
        //}

        //[Theory]
        ////[InlineData(null)]
        //[InlineData("")]
        //public async Task SiteTrainee_WhenSelectedSiteIsNullOrEmpty_TreatsAsAll(string selectedSite)
        //{
        //    // Arrange
        //    var allSitesDto = new List<SiteDTO> { new SiteDTO { PlantNo = "Site1" }, new SiteDTO { PlantNo = "Site2" } };
        //    var allSitesViewModel = new List<SiteViewModel> { new SiteViewModel { PlantNo = "Site1" }, new SiteViewModel { PlantNo = "Site2" } };

        //    _siteService.GetAllSitesAsync("All").Returns(allSitesDto);
        //    _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto).Returns(allSitesViewModel);

        //    // Act
        //    var result = await _controller.SiteTrainee(selectedSite);

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<SiteTraineeListViewModel>(viewResult.Model);
        //    Assert.Equal(selectedSite ?? "All", model.SelectedSite);
        //    Assert.Equal(allSitesViewModel, model.AllSites);
        //    Assert.Empty(model.FilteredTrainees);
        //}
       

        [Fact]
        public async Task SiteTrainee_VerifyServiceCalls()
        {
            // Arrange
            var selectedSite = "Site1";

            // Act
            await _controller.SiteTrainee(selectedSite);

            // Assert
            await _siteService.Received(1).GetAllSitesAsync("All");
            await _siteService.Received(1).GetSiteTraineesAsync(selectedSite);
        }
    }
}