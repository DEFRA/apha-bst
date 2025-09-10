using Apha.BST.Web.PresentationService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Apha.BST.Web.UnitTests.PresentationService
{
    public class LogServiceTests
    {
        private readonly ILogService _logService;
        private readonly ILogger<LogService> _logger;
        private readonly IConfiguration _configuration;

        public LogServiceTests()
        {
            _logger = Substitute.For<ILogger<LogService>>();
            _configuration = Substitute.For<IConfiguration>();
            _logService = new LogService(_logger, _configuration);
        }

        [Fact]
        public void LogGeneralException_WithConfigurationValue_LogsWithConfiguredErrorType()
        {
            // Arrange
            var exception = new Exception("Test general exception");
            var context = "TestController";
            var configuredErrorType = "BST.CUSTOM_GENERAL_EXCEPTION";

            _configuration["ExceptionTypes:General"].Returns(configuredErrorType);

            // Act
            _logService.LogGeneralException(exception, context);

            // Assert
            _logger.Received(1).Log(
                LogLevel.Error,
                0,
                Arg.Is<object>(o => o.ToString()!.Contains(configuredErrorType)),
                exception,
                Arg.Any<Func<object, Exception?, string>>()
            );

            var _ = _configuration.Received(1)["ExceptionTypes:General"];
        }

        [Fact]
        public void LogGeneralException_WithoutConfigurationValue_LogsWithDefaultErrorType()
        {
            // Arrange
            var exception = new Exception("Test general exception");
            var context = "TestController";
            var defaultErrorType = "BST.GENERAL_EXCEPTION";

            _configuration["ExceptionTypes:General"].Returns((string?)null);

            // Act
            _logService.LogGeneralException(exception, context);

            // Assert
            _logger.Received(1).Log(
                 LogLevel.Error,
                 0,
                 Arg.Is<object>(o => o.ToString()!.Contains(defaultErrorType)),
                 exception,
                 Arg.Any<Func<object, Exception?, string>>()
             );

            var _ = _configuration.Received(1)["ExceptionTypes:General"];
        }

        [Fact]
        public void LogSqlException_WithConfigurationValue_LogsWithConfiguredErrorType()
        {
            // Arrange
            var exception = new Exception("Test SQL exception");
            var context = "DataRepository";
            var configuredErrorType = "BST.CUSTOM_SQL_EXCEPTION";

            _configuration["ExceptionTypes:Sql"].Returns(configuredErrorType);

            // Act
            _logService.LogSqlException(exception, context);



            // Assert
            _logger.Received(1).Log(
        LogLevel.Error,
        0,
        Arg.Is<object>(o =>
            o.ToString()!.Contains(configuredErrorType) &&
            o.ToString()!.Contains(context) &&
            o.ToString()!.Contains(exception.Message)
        ),
        exception,
        Arg.Any<Func<object, Exception?, string>>()
    );
            var _ = _configuration.Received(1)["ExceptionTypes:Sql"];
        }

        [Fact]
        public void LogSqlException_WithoutConfigurationValue_LogsWithDefaultErrorType()
        {
            // Arrange
            var exception = new Exception("Test SQL exception");
            var context = "DataRepository";
            var defaultErrorType = "BST.GENERAL_EXCEPTION";

            _configuration["ExceptionTypes:Sql"].Returns((string?)null);

            // Act
            _logService.LogSqlException(exception, context);

            // Assert
            // Assert
            _logger.Received(1).Log(
        LogLevel.Error,
        0,
        Arg.Is<object>(o =>
            o.ToString()!.Contains(defaultErrorType) &&
            o.ToString()!.Contains(context) &&
            o.ToString()!.Contains(exception.Message)
        ),
        exception,
        Arg.Any<Func<object, Exception?, string>>()
    );

            var _ = _configuration.Received(1)["ExceptionTypes:Sql"];
        }

      

     
    }
}