using Apha.BST.Core.Entities;
using Apha.BST.Core.Pagination;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.DataAccess.UnitTests.AuditLogTest
{
        public class AuditLogRepositoryTests
        {
        private static readonly IFormatProvider DateFormat = CultureInfo.InvariantCulture;
        private List<AuditLog> GetSampleAuditLogs()
        {
            return new List<AuditLog>
        {
            new AuditLog { User = "User1", TransactionType = "Type1", Procedure = "Proc1", Parameters = "Param1", Date = DateTime.Parse("2023-01-01",DateFormat) },
            new AuditLog { User = "User2", TransactionType = "Type2", Procedure = "Proc2", Parameters = "Param2", Date = DateTime.Parse("2023-01-02",DateFormat) },
            new AuditLog { User = "User3", TransactionType = "Type3", Procedure = "Proc3", Parameters = "Param3",Date = DateTime.Parse("2023-01-03", DateFormat) }
        };
        }
        [Theory]
        [InlineData("user", false, "User1", "User2", "User3")]
        [InlineData("user", true, "User3", "User2", "User1")]
        [InlineData("transactiontype", false, "Type1", "Type2", "Type3")]
        [InlineData("transactiontype", true, "Type3", "Type2", "Type1")]
        [InlineData("procedure", false, "Proc1", "Proc2", "Proc3")]
        [InlineData("procedure", true, "Proc3", "Proc2", "Proc1")]
        [InlineData("parameters", false, "Param1", "Param2", "Param3")]
        [InlineData("parameters", true, "Param3", "Param2", "Param1")]
        [InlineData("date", false, "2023-01-01", "2023-01-02", "2023-01-03")]
        [InlineData("date", true, "2023-01-03", "2023-01-02", "2023-01-01")]
        public void ApplySortingByProperty_SortsCorrectly(string property, bool descending, params string[] expectedOrder)
        {
            // Arrange
            var sampleData = GetSampleAuditLogs().AsQueryable();

            // Act
            var result = ApplySortingByProperty(sampleData, property, descending);

            // Assert
            var resultList = result.Cast<AuditLog>().ToList();
            Assert.Equal(expectedOrder.Length, resultList.Count);

            for (int i = 0; i < expectedOrder.Length; i++)
            {
                string? actualValue = property switch
                {
                    "user" => resultList[i].User,
                    "transactiontype" => resultList[i].TransactionType,
                    "procedure" => resultList[i].Procedure,
                    "parameters" => resultList[i].Parameters,
                    "date" => resultList[i].Date?.ToString("yyyy-MM-dd"),
                    _ => throw new ArgumentException($"Unexpected property: {property}")
                };

                Assert.Equal(expectedOrder[i], actualValue);
            }
        }

        [Fact]
        public void ApplySortingByProperty_UnknownProperty_ReturnsOriginalQuery()
        {
            // Arrange
            var sampleData = GetSampleAuditLogs().AsQueryable();

            // Act
            var result = ApplySortingByProperty(sampleData, "unknownproperty", false);

            // Assert
            Assert.Same(sampleData, result);
        }

        // Fixes for CS0161, CA1822, IDE0060

        private static IQueryable ApplySortingByProperty(IQueryable<AuditLog> query, string property, bool descending)
        {
            if (string.IsNullOrWhiteSpace(property))
                return query;

            property = property.ToLowerInvariant();

            switch (property)
            {
                case "user":
                    return descending
                        ? query.OrderByDescending(x => x.User)
                        : query.OrderBy(x => x.User);
                case "transactiontype":
                    return descending
                        ? query.OrderByDescending(x => x.TransactionType)
                        : query.OrderBy(x => x.TransactionType);
                case "procedure":
                    return descending
                        ? query.OrderByDescending(x => x.Procedure)
                        : query.OrderBy(x => x.Procedure);
                case "parameters":
                    return descending
                        ? query.OrderByDescending(x => x.Parameters)
                        : query.OrderBy(x => x.Parameters);
                case "date":
                    return descending
                        ? query.OrderByDescending(x => x.Date)
                        : query.OrderBy(x => x.Date);
                default:
                    return query;
            }
        }

    
        [Fact]
        public async Task AddAuditLogAsync_FormatsParametersAndCallsExecute()
        {
            // Arrange
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditLogRepositoryTest(mockContext.Object);
            var parameters = new[]
            {
            new SqlParameter("@Id", SqlDbType.Int) { Value = 42 },
            new SqlParameter("@Name", SqlDbType.VarChar, 50) { Value = "Test" },
            new SqlParameter("@NullParam", SqlDbType.VarChar, 50) { Value = DBNull.Value },
            new SqlParameter("@NullParam2", SqlDbType.VarChar, 50) { Value = null }
        };

            // Act
            await repo.AddAuditLogAsync("sp_Test", parameters, "Write", "tester", "Test error");

            // Assert
            Assert.True(repo.ExecuteCalled);
            Assert.NotNull(repo.LastSql);
            Assert.Contains("sp_Audit_Log", repo.LastSql);
            Assert.NotNull(repo.LastParameters);
            var paramList = repo.LastParameters!;
            Assert.Equal("sp_Test", paramList[0].Value);
            Assert.Contains("Error occured in SP: Test error", paramList[1].Value.ToString());
            Assert.Contains("@Id:42;", paramList[1].Value.ToString());
            Assert.Contains("@Name:Test;", paramList[1].Value.ToString());
            Assert.Contains("@NullParam:;", paramList[1].Value.ToString());
            Assert.Contains("@NullParam2:;", paramList[1].Value.ToString());
            Assert.Equal("tester", paramList[2].Value);
            Assert.Equal("Write", paramList[3].Value);
        }

        [Fact]
            public async Task AddAuditLogAsync_IncludesErrorInParameters()
            {
                // Arrange
                var mockContext = new Mock<BstContext>();
                var repo = new AbstractAuditLogRepositoryTest(mockContext.Object);

                var parameters = new[]
                {
                new SqlParameter("@Id", SqlDbType.Int) { Value = 1 }
            };
                var errorMsg = "Some error";

                // Act
                await repo.AddAuditLogAsync("sp_Test", parameters, "Read", "user", errorMsg);

                // Assert
                Assert.True(repo.ExecuteCalled);

                var paramList = repo.LastParameters!;
                Assert.Contains("Error occured in SP: Some error", paramList[1].Value.ToString());
            }

        [Fact]
        public async Task AddAuditLogAsync_HandlesNullParameters()
        {
            // Arrange
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditLogRepositoryTest(mockContext.Object);

            var parameters = new SqlParameter[]
            {
                null!,
            };

            // Act
            await repo.AddAuditLogAsync("sp_Test", parameters, "Write", "tester");

            // Assert
            Assert.True(repo.ExecuteCalled);
            Assert.NotNull(repo.LastSql);
            Assert.Contains("sp_Audit_Log", repo.LastSql);
            Assert.NotNull(repo.LastParameters);
            var paramList = repo.LastParameters!;
            Assert.Equal("sp_Test", paramList[0].Value);
            Assert.Equal(string.Empty, paramList[1].Value);
            Assert.Equal("tester", paramList[2].Value);
            Assert.Equal("Write", paramList[3].Value);
        }

        [Fact]
            public async Task GetAuditLogsAsync_ReturnsPagedData()
            {
                // Arrange
                var auditLogs = new List<AuditLog>
            {
                new AuditLog { Procedure = "sp_Test1", User = "user1", TransactionType = "Write", Date = DateTime.UtcNow },
                new AuditLog { Procedure = "sp_Test2", User = "user2", TransactionType = "Read", Date = DateTime.UtcNow }
            }.AsQueryable();

                var mockContext = new Mock<BstContext>();
                var repo = new AbstractAuditLogRepositoryTest(mockContext.Object, auditLogs);

                var filter = new PaginationParameters(page: 1, pageSize: 10);

                // Act
                var result = await repo.GetAuditLogsAsync(filter, "sp_Test1");

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.TotalCount);
                Assert.Equal(2, result.Items.Count);
            }

            [Fact]
            public async Task GetStoredProcedureNamesAsync_ReturnsNames()
            {
                // Arrange
                var spList = new List<StoredProcedureList>
            {
                new StoredProcedureList { Name = "sp_Test1" },
                new StoredProcedureList { Name = "sp_Test2" }
            }.AsQueryable();

                var mockContext = new Mock<BstContext>();
                var repo = new AbstractAuditLogRepositoryTest(mockContext.Object, null, spList);

                // Act
                var result = await repo.GetStoredProcedureNamesAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Contains("sp_Test1", result);
                Assert.Contains("sp_Test2", result);
            }
        [Fact]
        public async Task GetAuditLogsAsync_DoesNotFilter_WhenSearchIsNullOrPercent()
        {
            var auditLogs = new List<AuditLog>
    {
        new AuditLog { Procedure = "sp_Test1", User = "user1" },
        new AuditLog { Procedure = "sp_Test2", User = "user2" }
    }.AsQueryable();

            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditLogRepositoryTest(mockContext.Object, auditLogs);

            var filterNull = new PaginationParameters(search: null, page: 1, pageSize: 10);
            var resultNull = await repo.GetAuditLogsAsync(filterNull, "");
            Assert.Equal(2, resultNull.Items.Count);

            var filterPercent = new PaginationParameters(search: "%", page: 1, pageSize: 10);
            var resultPercent = await repo.GetAuditLogsAsync(filterPercent, "");
            Assert.Equal(2, resultPercent.Items.Count);
        }

        [Fact]
        public async Task GetAuditLogsAsync_UnknownSortBy_DoesNotThrow()
        {
            var auditLogs = new List<AuditLog>
    {
        new AuditLog { Procedure = "sp_Test1", User = "user1" },
        new AuditLog { Procedure = "sp_Test2", User = "user2" }
    }.AsQueryable();

            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditLogRepositoryTest(mockContext.Object, auditLogs);

            var filter = new PaginationParameters(sortBy: "unknown", page: 1, pageSize: 10);
            var result = await repo.GetAuditLogsAsync(filter, "");
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetAuditLogsAsync_ReturnsEmpty_WhenNoLogs()
        {
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditLogRepositoryTest(mockContext.Object, Enumerable.Empty<AuditLog>().AsQueryable());
            var filter = new PaginationParameters(page: 1, pageSize: 10);
            var result = await repo.GetAuditLogsAsync(filter, "");
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetStoredProcedureNamesAsync_ReturnsEmpty_WhenNoProcedures()
        {
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditLogRepositoryTest(mockContext.Object, null, Enumerable.Empty<StoredProcedureList>().AsQueryable());
            var result = await repo.GetStoredProcedureNamesAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAuditLogAsync_HandlesEmptyParameters()
        {
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractAuditLogRepositoryTest(mockContext.Object);

            var parameters = Array.Empty<SqlParameter>();
            await repo.AddAuditLogAsync("sp_Test", parameters, "Write", "tester");
            Assert.True(repo.ExecuteCalled);
            Assert.NotNull(repo.LastParameters);
        }

        [Fact]
        public async Task ArchiveAuditLogAsync_DoesNotLogForDeleteOrUsageInsert()
        {
            var mockContext = new Mock<BstContext>();
            var repo = new Mock<AuditLogRepository>(mockContext.Object) { CallBase = true };

            repo.Protected()
                .Setup<Task<int>>("ExecuteSqlAsync", ItExpr.IsAny<string>(), ItExpr.IsAny<object[]>())
                .ReturnsAsync(1);

            // Should NOT call AddAuditLogAsync for DELETE
            await repo.Object.ArchiveAuditLogAsync("user");
            repo.Verify(x => x.AddAuditLogAsync(
                "sp_Audit_log_DELETE",
                It.IsAny<SqlParameter[]>(),
                "Write",
                "user",
                It.IsAny<string?>()), Times.Never);

            // Should NOT call AddAuditLogAsync for USAGE_INSERT
            await repo.Object.ArchiveAuditLogAsync("user");
            repo.Verify(x => x.AddAuditLogAsync(
                "sp_Usage_Insert",
                It.IsAny<SqlParameter[]>(),
                "Write",
                "user",
                It.IsAny<string?>()), Times.Never);
        }


        [Fact]
            public async Task ArchiveAuditLogAsync_CallsAddAuditLogAsync()
            {
                // Arrange
                var mockContext = new Mock<BstContext>();
                var repo = new Mock<AuditLogRepository>(mockContext.Object) { CallBase = true };

                repo.Protected()
                    .Setup<Task<int>>("ExecuteSqlAsync", ItExpr.IsAny<string>(), ItExpr.IsAny<object[]>())
                    .ReturnsAsync(1);

                repo.Setup(x => x.AddAuditLogAsync(
                        "sp_Audit_log_Archive",
                        It.IsAny<SqlParameter[]>(),
                        "Write",
                        "user",
                        It.IsAny<string?>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                // Act
                await repo.Object.ArchiveAuditLogAsync("user");

                // Assert
                repo.Verify(x => x.AddAuditLogAsync(
                    "sp_Audit_log_Archive",
                    It.IsAny<SqlParameter[]>(),
                    "Write",
                    "user",
                    It.IsAny<string?>()), Times.Once);
            }

            [Fact]
            public async Task ArchiveAuditLogAsync_ThrowsException_AndLogsAudit()
            {
                // Arrange
                var mockContext = new Mock<BstContext>();
                var repo = new AbstractAuditLogRepositoryTest(mockContext.Object);
                repo.SimulateException = new Exception("DB error");

                // Act & Assert
                var ex = await Assert.ThrowsAsync<Exception>(() => repo.ArchiveAuditLogAsync("admin"));
                Assert.Equal("DB error", ex.Message);
                Assert.True(repo.ExecuteCalled);
            }

        }
}
