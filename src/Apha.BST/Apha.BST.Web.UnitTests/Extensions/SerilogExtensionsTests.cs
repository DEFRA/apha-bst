using Apha.BST.Web.Extensions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Web.UnitTests.Extensions
{
    public class SerilogExtensionsTests
    {
        [Fact]
        public void UseAwsCloudWatch_WithConfiguration_ReturnsLoggerConfiguration()
        {
            // Arrange
            var mockConfiguration = Substitute.For<IConfiguration>();
            mockConfiguration["AwsLogging:LogGroupName"].Returns("test-log-group");

            var loggerConfiguration = new LoggerConfiguration();

            // Act
            var result = loggerConfiguration.UseAwsCloudWatch(mockConfiguration);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LoggerConfiguration>(result);

            // Verify that the configuration was accessed
            var _ = mockConfiguration.Received(1)["AwsLogging:LogGroupName"];
        }

      

        [Fact]
        public void UseAwsCloudWatch_WithNullLogGroupName_ThrowsArgumentException()
        {
            // Arrange
            var mockConfiguration = Substitute.For<IConfiguration>();
            mockConfiguration["AwsLogging:LogGroupName"].Returns((string?)null);

            var loggerConfiguration = new LoggerConfiguration();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                loggerConfiguration.UseAwsCloudWatch(mockConfiguration));

            Assert.Contains("LogGroupName must be specified", exception.Message);

            // Verify that the configuration was accessed
            var _ = mockConfiguration.Received(1)["AwsLogging:LogGroupName"];
        }

        [Fact]
        public void UseAwsCloudWatch_WithEmptyLogGroupName_ThrowsArgumentException()
        {
            // Arrange
            var mockConfiguration = Substitute.For<IConfiguration>();
            mockConfiguration["AwsLogging:LogGroupName"].Returns("");

            var loggerConfiguration = new LoggerConfiguration();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                loggerConfiguration.UseAwsCloudWatch(mockConfiguration));

            Assert.Contains("LogGroupName must be specified", exception.Message);

            // Verify that the configuration was accessed
            var _ = mockConfiguration.Received(1)["AwsLogging:LogGroupName"];
        }

      
    }
    }
