using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Pagination;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class AuditLogControllerTests
    {
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;
        private readonly AuditLogController _controller;

        public AuditLogControllerTests()
        {
            _auditLogService = Substitute.For<IAuditLogService>();
            _mapper = Substitute.For<IMapper>();
            _userDataService = Substitute.For<IUserDataService>();
            _logService = Substitute.For<ILogService>();
            _controller = new AuditLogController(_auditLogService, _mapper, _userDataService, _logService);
        }
        
        [Fact]
        public async Task Index_ValidInput_ReturnsViewWithCorrectModel()
        {
            // Arrange
            int pageNo = 1;
            string column = "Date";
            bool sortOrder = false;
            string storedProcedure = "%";

            var auditLogDtos = new List<AuditLogDto>
            {
                new AuditLogDto { Procedure = "Proc1", Parameters = "Param1", User = "User1", Date = DateTime.Now, TransactionType = "Type1" },
                new AuditLogDto { Procedure = "Proc2", Parameters = "Param2", User = "User2", Date = DateTime.Now, TransactionType = "Type2" }
            };

            var paginatedResult = new PaginatedResult<AuditLogDto>
            {
                data = auditLogDtos,
                TotalCount = 2
            };

            _auditLogService.GetAuditLogsAsync(Arg.Any<QueryParameters>(), Arg.Any<string>()).Returns(paginatedResult);
            _auditLogService.GetStoredProcedureNamesAsync().Returns(new List<string> { "Proc1", "Proc2" });

            var auditLogViewModels = new List<AuditLogViewModel>
            {
            new AuditLogViewModel { Procedure = "Proc1", Parameters = "Param1", User = "User1", Date = DateTime.Now, TransactionType = "Type1" },
            new AuditLogViewModel { Procedure = "Proc2", Parameters = "Param2", User = "User2", Date = DateTime.Now, TransactionType = "Type2" }
            };

            _mapper.Map<IEnumerable<AuditLogViewModel>>(Arg.Any<IEnumerable<AuditLogDto>>()).Returns(auditLogViewModels);

            // Act
            var result = await _controller.Index(pageNo, column, sortOrder, storedProcedure);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AuditLogListViewModel>(viewResult.Model);
            Assert.Equal(2, model.AuditLogsResults?.Count());
            Assert.Equal(2, model.StoredProcedures.Count());
            Assert.Equal(storedProcedure, model.StoredProcedure);
            Assert.Equal(pageNo, model.Pagination?.PageNumber);
            Assert.Equal(30, model.Pagination?.PageSize);
            Assert.Equal(column, model.Pagination?.SortColumn);
            Assert.Equal(sortOrder, model.Pagination?.SortDirection);
        }
        [Fact]
        public async Task Index_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task Index_EmptyColumn_UsesDefaultSorting()
        {
            // Arrange
            _auditLogService.GetAuditLogsAsync(Arg.Any<QueryParameters>(), Arg.Any<string>())
            .Returns(new PaginatedResult<AuditLogDto> { data = new List<AuditLogDto>(), TotalCount = 0 });
            _auditLogService.GetStoredProcedureNamesAsync().Returns(new List<string>());

            // Act
            var result = await _controller.Index(column: "");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AuditLogListViewModel>(viewResult.Model);
            Assert.Equal("", model.Pagination?.SortColumn);
        }
        [Fact]
        public async Task Index_StoredProcedureFilter_AppliesFilter()
        {
            // Arrange
            string storedProcedure = "TestProc";
            _auditLogService.GetAuditLogsAsync(Arg.Any<QueryParameters>(), Arg.Is<string>(s => s == storedProcedure))
            .Returns(new PaginatedResult<AuditLogDto> { data = new List<AuditLogDto>(), TotalCount = 0 });
            _auditLogService.GetStoredProcedureNamesAsync().Returns(new List<string> { storedProcedure });

            // Act
            var result = await _controller.Index(storedProcedure: storedProcedure);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AuditLogListViewModel>(viewResult.Model);
            Assert.Equal(storedProcedure, model.StoredProcedure);
        }        
        
    }
}
