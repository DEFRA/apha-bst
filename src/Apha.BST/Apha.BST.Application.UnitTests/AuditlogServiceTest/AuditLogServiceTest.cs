using Apha.BST.Application.DTOs;
using Apha.BST.Application.Pagination;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Pagination;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Apha.BST.Application.UnitTests.AuditlogServiceTest
{
    public class AuditLogServiceTests : AbstractAuditLogServiceTest
    {
        public AuditLogServiceTests() : base() { }

        [Fact]
        public async Task GetAuditLogsAsync_SuccessfulRetrieval_ReturnsPaginatedResult()
        {
            // Arrange
            var filter = new QueryParameters { Page = 1, PageSize = 10 };
            var storedProcedure = "GetAuditLogs";
            var paginationParameters = new PaginationParameters();
            var repositoryResult = new PagedData<AuditLog>(new List<AuditLog> { new AuditLog() }, 1);
            var mappedResult = new PaginatedResult<AuditLogDto> { data = new[] { new AuditLogDto() }, TotalCount = 1 };

            SetupMapperForPaginationParameters(filter, paginationParameters);
            SetupRepositoryForGetAuditLogs(paginationParameters, storedProcedure, repositoryResult);
            SetupMapperForAuditLogResult(repositoryResult, mappedResult);

            // Act
            var result = await AuditLogService.GetAuditLogsAsync(filter, storedProcedure);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PaginatedResult<AuditLogDto>>(result);
            Assert.Single(result.data);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task GetAuditLogsAsync_EmptyResult_ReturnsEmptyPaginatedResult()
        {
            // Arrange
            var filter = new QueryParameters { Page = 1, PageSize = 10 };
            var storedProcedure = "GetAuditLogs";
            var paginationParameters = new PaginationParameters();
            var emptyRepositoryResult = new PagedData<AuditLog>(new List<AuditLog>(), 0);
            var emptyMappedResult = new PaginatedResult<AuditLogDto> { data = Array.Empty<AuditLogDto>(), TotalCount = 0 };

            SetupMapperForPaginationParameters(filter, paginationParameters);
            SetupRepositoryForGetAuditLogs(paginationParameters, storedProcedure, emptyRepositoryResult);
            SetupMapperForAuditLogResult(emptyRepositoryResult, emptyMappedResult);

            // Act
            var result = await AuditLogService.GetAuditLogsAsync(filter, storedProcedure);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.data);
            Assert.Equal(0, result.TotalCount);
        }



        [Fact]
        public async Task GetAuditLogsAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var filter = new QueryParameters { Page = 1, PageSize = 10 };
            var storedProcedure = "GetAuditLogs";
            var paginationParameters = new PaginationParameters();
            var exception = new Exception("Repository error");

            SetupMapperForPaginationParameters(filter, paginationParameters);
            SetupRepositoryToThrowException(paginationParameters, storedProcedure, exception);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => AuditLogService.GetAuditLogsAsync(filter, storedProcedure));
        }

        [Fact]
        public async Task GetAuditLogsAsync_VerifyMethodCalls()
        {
            // Arrange
            var filter = new QueryParameters { Page = 1, PageSize = 10 };
            var storedProcedure = "GetAuditLogs";
            var paginationParameters = new PaginationParameters();
            var repositoryResult = new PagedData<AuditLog>(new List<AuditLog>(), 0);
            var mappedResult = new PaginatedResult<AuditLogDto>();

            SetupMapperForPaginationParameters(filter, paginationParameters);
            SetupRepositoryForGetAuditLogs(paginationParameters, storedProcedure, repositoryResult);
            SetupMapperForAuditLogResult(repositoryResult, mappedResult);

            // Act
            await AuditLogService.GetAuditLogsAsync(filter, storedProcedure);

            // Assert
            Mapper.Received(1).Map<PaginationParameters>(filter);
            await AuditLogRepository.Received(1).GetAuditLogsAsync(paginationParameters, storedProcedure);
            Mapper.Received(1).Map<PaginatedResult<AuditLogDto>>(Arg.Is<PagedData<AuditLog>>(p => p == repositoryResult));
        }
        [Fact]
        public async Task ArchiveAuditLogAsync_CallsRepositoryWithCorrectUsername()
        {
            // Arrange
            string username = "testuser";
            SetupRepositoryForArchiveAuditLog(username);

            // Act
            await AuditLogService.ArchiveAuditLogAsync(username);

            // Assert
            await AuditLogRepository.Received(1).ArchiveAuditLogAsync(username);
        }



        [Fact]
        public async Task ArchiveAuditLogAsync_HandlesRepositoryExceptions()
        {
            // Arrange
            string username = "testuser";
            var exception = new Exception("Repository error");
            SetupRepositoryForArchiveAuditLogToThrowException(username, exception);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => AuditLogService.ArchiveAuditLogAsync(username));
        }
        [Fact]
        public async Task GetStoredProcedureNamesAsync_ReturnsExpectedList()
        {
            // Arrange
            var expectedProcedures = new List<string> { "proc1", "proc2", "proc3" };
            SetupRepositoryForGetStoredProcedureNames(expectedProcedures);

            // Act
            var result = await AuditLogService.GetStoredProcedureNamesAsync();

            // Assert
            Assert.Equal(expectedProcedures, result);
            await AuditLogRepository.Received(1).GetStoredProcedureNamesAsync();
        }
     

        [Fact]
        public async Task GetStoredProcedureNamesAsync_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            var expectedException = new Exception("Repository error");
            AuditLogRepository.GetStoredProcedureNamesAsync().Throws(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => AuditLogService.GetStoredProcedureNamesAsync());
            Assert.Equal(expectedException.Message, exception.Message);
            await AuditLogRepository.Received(1).GetStoredProcedureNamesAsync();
        }
    }
}

    
