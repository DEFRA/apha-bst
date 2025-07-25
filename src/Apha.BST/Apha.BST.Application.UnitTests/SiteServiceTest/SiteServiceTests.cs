using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using FluentAssertions;

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
        public async Task CreateSiteAsync_WhenSiteAlreadyExists_ShouldReturnExistsMessage()
        {
            // Arrange
            MockForCreateSite(returnCode: 1);
            var siteDto = new SiteDTO { PlantNo = "PLANT002", Name = "Existing Site" };

            // Act
            var result = await _siteService.CreateSiteAsync(siteDto);

            // Assert
            result.Should().Be("Site already exists. Please choose another Site / Plant No.");
        }

        [Fact]
        public async Task CreateSiteAsync_WhenSiteIsNew_ShouldReturnSuccessMessage()
        {
            // Arrange
            MockForCreateSite(returnCode: 0);
            var siteDto = new SiteDTO { PlantNo = "PLANT003", Name = "Fresh Site" };

            // Act
            var result = await _siteService.CreateSiteAsync(siteDto);

            // Assert
            result.Should().Be("Site added successfully.");
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