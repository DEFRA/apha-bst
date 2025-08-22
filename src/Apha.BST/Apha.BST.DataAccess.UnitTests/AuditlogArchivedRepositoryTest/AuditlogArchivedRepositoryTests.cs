using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Pagination;
using Apha.BST.DataAccess.Data;
using Moq;

namespace Apha.BST.DataAccess.UnitTests.AuditlogArchivedRepositoryTest
{
    public class AuditlogArchivedRepositoryTests
    {
        [Fact]
        public async Task GetArchiveAuditLogsAsync_ReturnsPagedData()
        {
            var archivedLogs = new List<AuditlogArchived>
            {
                new AuditlogArchived { Procedure = "sp_Test1", User = "user1", TransactionType = "Write", Date = DateTime.UtcNow },
                new AuditlogArchived { Procedure = "sp_Test2", User = "user2", TransactionType = "Read", Date = DateTime.UtcNow }
            }.AsQueryable();

            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditlogArchivedRepositoryTest(mockContext.Object, archivedLogs);

            var filter = new PaginationParameters(page: 1, pageSize: 10);

            var result = await repo.GetArchiveAuditLogsAsync(filter, "sp_Test1");

            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetArchiveAuditLogsAsync_FiltersBySearch()
        {
            var archivedLogs = new List<AuditlogArchived>
            {
                new AuditlogArchived { Procedure = "sp_Test1", User = "user1" },
                new AuditlogArchived { Procedure = "sp_Test2", User = "user2" }
            }.AsQueryable();

            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditlogArchivedRepositoryTest(mockContext.Object, archivedLogs);

            var filter = new PaginationParameters(search: "sp_Test2", page: 1, pageSize: 10);

            var result = await repo.GetArchiveAuditLogsAsync(filter, "sp_Test2");

            Assert.Single(result.Items);
            Assert.Equal("sp_Test2", result.Items.First().Procedure);
        }

        [Fact]
        public async Task GetStoredProcedureNamesAsync_ReturnsNames()
        {
            var spList = new List<StoredProcedureList>
            {
                new StoredProcedureList { Name = "sp_Test1" },
                new StoredProcedureList { Name = "sp_Test2" }
            }.AsQueryable();

            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditlogArchivedRepositoryTest(mockContext.Object, null, spList);

            var result = await repo.GetStoredProcedureNamesAsync();

            Assert.NotNull(result);
            Assert.Contains("sp_Test1", result);
            Assert.Contains("sp_Test2", result);
        }

        [Fact]
        public async Task GetArchiveAuditLogsAsync_SortsDescending()
        {
            var now = DateTime.UtcNow;
            var archivedLogs = new List<AuditlogArchived>
            {
                new AuditlogArchived { Procedure = "sp_Test1", User = "user1", Date = now.AddDays(-1) },
                new AuditlogArchived { Procedure = "sp_Test2", User = "user2", Date = now }
            }.AsQueryable();

            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditlogArchivedRepositoryTest(mockContext.Object, archivedLogs);

            var filter = new PaginationParameters(sortBy: "date", descending: true, page: 1, pageSize: 10);

            var result = await repo.GetArchiveAuditLogsAsync(filter, "");

            Assert.Equal("sp_Test2", result.Items.First().Procedure);
        }
        [Fact]
        public async Task GetArchiveAuditLogsAsync_ReturnsEmpty_WhenNoLogs()
        {
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditlogArchivedRepositoryTest(mockContext.Object, Enumerable.Empty<AuditlogArchived>().AsQueryable());
            var filter = new PaginationParameters(page: 1, pageSize: 10);
            var result = await repo.GetArchiveAuditLogsAsync(filter, "");
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetStoredProcedureNamesAsync_ReturnsEmpty_WhenNoProcedures()
        {
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditlogArchivedRepositoryTest(mockContext.Object, null, Enumerable.Empty<StoredProcedureList>().AsQueryable());
            var result = await repo.GetStoredProcedureNamesAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetArchiveAuditLogsAsync_DoesNotFilter_WhenSearchIsNullOrPercent()
        {
            var archivedLogs = new List<AuditlogArchived>
    {
        new AuditlogArchived { Procedure = "sp_Test1", User = "user1" },
        new AuditlogArchived { Procedure = "sp_Test2", User = "user2" }
    }.AsQueryable();

            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditlogArchivedRepositoryTest(mockContext.Object, archivedLogs);

            var filterNull = new PaginationParameters(search: null, page: 1, pageSize: 10);
            var resultNull = await repo.GetArchiveAuditLogsAsync(filterNull, "");
            Assert.Equal(2, resultNull.Items.Count);

            var filterPercent = new PaginationParameters(search: "%", page: 1, pageSize: 10);
            var resultPercent = await repo.GetArchiveAuditLogsAsync(filterPercent, "");
            Assert.Equal(2, resultPercent.Items.Count);
        }

        [Fact]
        public async Task GetArchiveAuditLogsAsync_UnknownSortBy_DoesNotThrow()
        {
            var archivedLogs = new List<AuditlogArchived>
    {
        new AuditlogArchived { Procedure = "sp_Test1", User = "user1" },
        new AuditlogArchived { Procedure = "sp_Test2", User = "user2" }
    }.AsQueryable();

            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditlogArchivedRepositoryTest(mockContext.Object, archivedLogs);

            var filter = new PaginationParameters(sortBy: "unknown", page: 1, pageSize: 10);
            var result = await repo.GetArchiveAuditLogsAsync(filter, "");
            Assert.Equal(2, result.Items.Count);
        }

    }
}
