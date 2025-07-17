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
            //// Setup TempData
            //var httpContext = new DefaultHttpContext();
            //_controller.TempData = new TempDataDictionary(httpContext, Substitute.For<ITempDataProvider>());
        }

        [Fact]
        public async Task ViewSite_WhenSelectedSiteIsAll_ReturnsAllSites()
        {
            // Arrange
            var siteDtos = new List<SiteDTO>
            {
                new SiteDTO { PlantNo = "Site1", Name = "Site 1" },
                new SiteDTO { PlantNo = "Site2", Name = "Site 2" }
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
            var siteDtos = new List<SiteDTO>
            {
                new SiteDTO { PlantNo = "Site1", Name = "Site 1" },
                new SiteDTO { PlantNo = "Site2", Name = "Site 2" }
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
            var siteDtos = new List<SiteDTO>();
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
        public async Task AddSite_ValidInput_SiteAddedSuccessfully()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Test Site", PlantNo = "123" };
            var siteDto = new SiteDTO { Name = "Test Site", PlantNo = "123" };
            _mapper.Map<SiteDTO>(siteViewModel).Returns(siteDto);
            _siteService.CreateSiteAsync(Arg.Any<SiteDTO>()).Returns("Site added successfully.");

            // Act
            var result = await _controller.AddSite(siteViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(SiteController.AddSite), result.ActionName);
            Assert.Equal("'Test Site' saved as site", _controller.TempData["Message"]);
            await _siteService.Received(1).CreateSiteAsync(Arg.Is<SiteDTO>(dto => dto.Name == "Test Site" && dto.PlantNo == "123"));
            _mapper.Received(1).Map<SiteDTO>(Arg.Is<SiteViewModel>(vm => vm.Name == "Test Site" && vm.PlantNo == "123"));
        }

        [Fact]
        public async Task AddSite_ValidInput_SiteAlreadyExists()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "Existing Site", PlantNo = "456" };
            var siteDto = new SiteDTO { Name = "Existing Site", PlantNo = "456" };
            _mapper.Map<SiteDTO>(siteViewModel).Returns(siteDto);
            _siteService.CreateSiteAsync(Arg.Any<SiteDTO>()).Returns("Site already exists.");

            // Act
            var result = await _controller.AddSite(siteViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(SiteController.AddSite), result.ActionName);
            Assert.Equal("Site already exists.", _controller.TempData["Message"]);
            await _siteService.Received(1).CreateSiteAsync(Arg.Is<SiteDTO>(dto => dto.Name == "Existing Site" && dto.PlantNo == "456"));
            _mapper.Received(1).Map<SiteDTO>(Arg.Is<SiteViewModel>(vm => vm.Name == "Existing Site" && vm.PlantNo == "456"));
        }

        [Fact]
        public async Task AddSite_InvalidInput_ReturnsViewWithModel()
        {
            // Arrange
            var siteViewModel = new SiteViewModel { Name = "", PlantNo = "" };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.AddSite(siteViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SiteViewModel>(result.Model);
            Assert.False(_controller.ModelState.IsValid);
            await _siteService.DidNotReceive().CreateSiteAsync(Arg.Any<SiteDTO>());
            _mapper.DidNotReceive().Map<SiteDTO>(Arg.Any<SiteViewModel>());
        }
    }
}