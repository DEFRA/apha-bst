using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
}