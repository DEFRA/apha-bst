using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Pagination;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;

namespace Apha.BST.DataAccess.UnitTests.AuditLogTest
{
        public class AuditLogRepositoryTests
        {
            [Fact]
            public async Task AddAuditLogAsync_FormatsParametersAndCallsExecute()
            {
                // Arrange
                var mockContext = new Mock<BstContext>();
                var repo = new AbstractAuditLogRepositoryTest(mockContext.Object);

                var parameters = new[]
                {
                new SqlParameter("@Id", SqlDbType.Int) { Value = 42 },
                new SqlParameter("@Name", SqlDbType.VarChar, 50) { Value = "Test" }
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
            Assert.Contains("@Id:42;", paramList[1].Value.ToString());
            Assert.Contains("@Name:Test;", paramList[1].Value.ToString());
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
                new SqlParameter("@Value", SqlDbType.VarChar, 10) { Value = DBNull.Value }
                };

                // Act
                await repo.AddAuditLogAsync("sp_Test", parameters, "Read", "user");

                // Assert
                Assert.True(repo.ExecuteCalled);

                var paramList = repo.LastParameters!;
                Assert.Contains("@Value:;", paramList[1].Value.ToString());
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
