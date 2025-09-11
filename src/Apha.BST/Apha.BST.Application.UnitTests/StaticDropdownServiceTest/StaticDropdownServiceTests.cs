using Apha.BST.Application.Services;
using Apha.BST.Application.UnitTests.StaticDropdownServiceTest;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Xunit;

namespace Apha.BST.Application.Tests.Services
{
    public class StaticDropdownServiceTests : AbstractStaticDropdownServiceTest
    {
        [Fact]
        public void GetTrainingTypes_ReturnsExpectedList()
        {
            // Arrange
            var service = new StaticDropdownService();
            var expected = new List<(string Value, string Text)>
    {
        ("Cascade trained", "Cascade trained"),
        ("Trained", "Trained"),
        ("Training confirmed", "Training confirmed")
    };

            // Act
            var result = service.GetTrainingTypes();

            // Assert
            Assert.Equal(expected.Count, result.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Value, result[i].Value);
                Assert.Equal(expected[i].Text, result[i].Text);
            }
        }

        [Fact]
        public void GetTrainingAnimal_ReturnsExpectedList()
        {
            // Arrange
            var service = new StaticDropdownService();
            var expected = new List<(string Value, string Text)>
    {
        ("Cattle", "Cattle"),
        ("Sheep and Goat", "Sheep and Goat")
    };

            // Act
            var result = service.GetTrainingAnimal();

            // Assert
            Assert.Equal(expected.Count, result.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Value, result[i].Value);
                Assert.Equal(expected[i].Text, result[i].Text);
            }
        }
    }
}
