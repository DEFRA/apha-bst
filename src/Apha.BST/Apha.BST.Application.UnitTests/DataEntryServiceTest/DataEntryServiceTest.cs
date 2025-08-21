using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.UnitTests.DataEntryServiceTest
{
    public class DataEntryServiceTests : AbstractDataEntryServiceTest
    {
        [Fact]
        public async Task CanEditPage_WhenRepositoryReturnsTrue_ShouldReturnTrue()
        {
            // Arrange
            const string action = "TestAction";
            MockCanEditPage(action, true);

            // Act
            var result = await Service!.CanEditPage(action);

            // Assert
            Assert.True(result);
            await AssertCanEditPageCalled(action);
        }

        [Fact]
        public async Task CanEditPage_WhenRepositoryReturnsFalse_ShouldReturnFalse()
        {
            // Arrange
            const string action = "TestAction";
            MockCanEditPage(action, false);

            // Act
            var result = await Service!.CanEditPage(action);

            // Assert
            Assert.False(result);
            await AssertCanEditPageCalled(action);
        }

        [Fact]
        public async Task CanEditPage_WhenRepositoryThrowsException_ShouldThrowException()
        {
            // Arrange
            const string action = "TestAction";
            var expectedException = new Exception("Test exception");
            SetupMockCanEditPageThrowsException(action, expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => Service!.CanEditPage(action));
            Assert.Equal(expectedException.Message, exception.Message);
            await AssertCanEditPageCalled(action);
        }
    }
    }
