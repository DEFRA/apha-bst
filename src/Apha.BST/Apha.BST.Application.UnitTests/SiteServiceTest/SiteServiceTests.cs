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
            public async Task GetAllSitesAsync_WhenSitesExist_ShouldReturnAllSites()
            {
             //Arrange
              MockforGetSites();
            // Act
            var result = await _siteService.GetAllSitesAsync("PLANT001");

                // Assert
                result.Count().Should().Be(1);
            }

            [Fact]
            public async Task GetAllSitesAsync_WhenNoSitesExist_ShouldReturnEmptyList()
            {
            //Arrange
            MockforGetSites();
            // Act
            var result = await _siteService.GetAllSitesAsync("PLANT_NO_DATA");

                // Assert
                result.Should().BeEmpty();
            }

            [Fact]
            public async Task GetAllSitesAsync_ShouldReturnExpectedMappedData()
            {
            //Arrange
            MockforGetSites();
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
            var siteDto = new SiteDTO { Name = "Test Site", PlantNo = "1234" };
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
            var siteDto = new SiteDTO { Name = "Existing Site", PlantNo = "5678" };
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
            var siteDto = new SiteDTO { Name = "Test Site", PlantNo = "1234" };
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
        public async Task GetSiteTraineesAsync_WhenTraineesExist_ShouldReturnTraineesList()
        {
            // Arrange
            MockForGetSiteTrainees("PLANT001");

            // Act
            var result = await _siteService.GetSiteTraineesAsync("PLANT001");

            // Assert
            result.Count().Should().Be(2);
            result.First().PersonId.Should().Be(1);
            result.First().Person.Should().Be("John Doe");
            result.First().Cattle.Should().BeTrue();
            result.First().Sheep.Should().BeFalse();
            result.First().Goats.Should().BeTrue();
        }

        [Fact]
        public async Task GetSiteTraineesAsync_WhenNoTraineesExist_ShouldReturnEmptyList()
        {
            // Arrange
            MockForGetSiteTrainees("PLANT_NO_DATA");

            // Act
            var result = await _siteService.GetSiteTraineesAsync("PLANT_NO_DATA");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSiteTraineesAsync_InvalidPlantNo_ShouldReturnEmptyList()
        {
            // Arrange
            MockForGetSiteTrainees("INVALID_PLANT");

            // Act
            var result = await _siteService.GetSiteTraineesAsync("INVALID_PLANT");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSiteTraineesAsync_ShouldReturnExpectedMappedData()
        {
            // Arrange
            MockForGetSiteTrainees("PLANT001");

            // Act
            var result = await _siteService.GetSiteTraineesAsync("PLANT001");

            // Assert
            result.Should().ContainSingle(t => t.Person == "Jane Smith" && t.Sheep);
            result.First(t => t.Person == "John Doe").Cattle.Should().BeTrue();
        }
    }
}