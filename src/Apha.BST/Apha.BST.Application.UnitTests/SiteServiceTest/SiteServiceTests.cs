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
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Application.UnitTests.Services
{
    public class SiteServiceTests : AbstractSiteServiceTest
    {
        [Fact]
        public async Task GetAllSitesAsync_WithValidPlantNo_ReturnsListOfSiteDtos()
        {
            // Arrange
            MockForGetSites("PLANT001");

            // Act
            var result = await _siteService.GetAllSitesAsync("PLANT001");

            // Assert
            result.Count().Should().Be(1);
            result.First().Name.Should().Be("Site 1");
            result.First().PlantNo.Should().Be("PLANT001");
        }

        [Fact]
        public async Task GetAllSitesAsync_WithAnotherValidPlantNo_ReturnsListOfSiteDtos()
        {
            // Arrange
            MockForGetSites("PLANT002");

            // Act
            var result = await _siteService.GetAllSitesAsync("PLANT002");

            // Assert
            result.Count().Should().Be(1);
            result.First().Name.Should().Be("Site 1");
            result.First().PlantNo.Should().Be("PLANT002");
        }

        [Fact]
        public async Task GetAllSitesAsync_ShouldReturnExpectedMappedData()
        {
            // Arrange
            MockForGetSites("PLANT001");

            // Act
            var result = await _siteService.GetAllSitesAsync("PLANT001");

            // Assert
            result.Should().ContainSingle(s => s.Name == "Site 1");
            result.First().PlantNo.Should().Be("PLANT001");
        }
        [Fact]
        public async Task AddSiteAsync_NewSite_ReturnsSuccessMessage()
        {
            // Arrange
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "1234" };
            MockForAddSiteAsync("CREATED", siteDto);

            // Act
            var result = await _siteService.AddSiteAsync(siteDto);

            // Assert
            result.Should().Be("'Test Site' saved as site");
        }

        [Fact]
        public async Task AddSiteAsync_ExistingSite_ReturnsErrorMessage()
        {
            // Arrange
            var siteDto = new SiteDto { Name = "Existing Site", PlantNo = "5678" };
            MockForAddSiteAsync("EXISTS", siteDto);

            // Act
            var result = await _siteService.AddSiteAsync(siteDto);

            // Assert
            result.Should().Be("Site already exists. Please choose another Site / Plant No.");
        }

        [Fact]
        public async Task AddSiteAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var siteDto = new SiteDto { Name = "Test Site", PlantNo = "1234" };
            var mockRepo = Substitute.For<ISiteRepository>();
            var mockMapper = Substitute.For<IMapper>();
            var site = new Site { Name = "Test Site", PlantNo = "1234" };
            mockMapper.Map<Site>(siteDto).Returns(site);
            mockRepo.AddSiteAsync(site).ThrowsAsync(new Exception("Repository error"));

            _siteService = new SiteService(mockRepo, mockMapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _siteService.AddSiteAsync(siteDto));
        }

        [Fact]
        public async Task GetSiteTraineesAsync_ValidPlantNo_ReturnsNonEmptyList()
        {
            // Arrange
            MockForGetSiteTraineesAsync("PLANT001");  // Mock data for valid plant number

            // Act
            var result = await _siteService.GetSiteTraineesAsync("PLANT001");

            // Assert
            result.Count.Should().Be(2);  // Expecting 2 trainees
            result[0].Person.Should().Be("John Doe");
            result[1].Person.Should().Be("Jane Smith");
            result[0].Cattle.Should().BeTrue();
            result[1].SheepAndGoat.Should().BeTrue();
        }

        [Fact]
        public async Task GetSiteTraineesAsync_InvalidPlantNo_ReturnsEmptyList()
        {
            // Arrange
            MockForGetSiteTraineesAsync("INVALID001");  // Mock empty list for invalid plant number

            // Act
            var result = await _siteService.GetSiteTraineesAsync("INVALID001");

            // Assert
            result.Should().BeEmpty();  // Expecting an empty list for invalid plant number
        }

        [Fact]
        public async Task DeleteTraineeAsync_TraineeWithExistingRecords_ReturnsWarningMessage()
        {
            // Arrange
            int personId = 2;
            string personName = "Jane Smith";
            MockforDeleteTraineeAsync(personId, personName, false);

            // Act
            var result = await _siteService.DeleteTraineeAsync(personId);

            // Assert
            result.Should().Be($"Trainee '{personName}' has training records. Delete them first if you wish to remove the person.");
        }
    }
}