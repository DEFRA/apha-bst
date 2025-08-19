using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class ReportControllerTests
    {
        private readonly IReportService _reportService;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            _reportService = Substitute.For<IReportService>();
            _userDataService = Substitute.For<IUserDataService>();
            _logService = Substitute.For<ILogService>();
            _controller = new ReportController(_reportService, _userDataService, _logService);

            // Setup TempData for the controller
            var tempData = Substitute.For<ITempDataDictionary>();
            _controller.TempData = tempData;

            // Setup ControllerContext with ActionDescriptor
            var controllerActionDescriptor = new ControllerActionDescriptor { ActionName = "Report" };
            _controller.ControllerContext = new ControllerContext
            {
                ActionDescriptor = controllerActionDescriptor
            };
        }

        [Fact]
        public async Task Report_SuccessfulExecution_ReturnsViewResult()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.Report();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Report_VerifyViewModelProperties_ReturnsCorrectProperties()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.Report();
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ReportViewModel>(viewResult.Model);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal("Reports", viewModel.ReportTitle);
            Assert.True(viewModel.CanEdit);
        }

        [Fact]
        public async Task Report_CanEditPageReturnsTrue_CanEditIsTrue()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.Report();
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ReportViewModel>(viewResult.Model);

            // Assert
            Assert.True(viewModel.CanEdit);
        }

        [Fact]
        public async Task Report_CanEditPageReturnsFalse_CanEditIsFalse()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.Report();
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ReportViewModel>(viewResult.Model);
            Assert.False(viewModel.CanEdit);
        }

        [Fact]
        public async Task Report_VerifyUserDataServiceCalled_CalledWithCorrectActionName()
        {
            // Arrange
            string expectedActionName = "Report";
            _controller.ControllerContext = new ControllerContext
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ActionName = expectedActionName
                }
            };

            // Act
            await _controller.Report();

            // Assert
            await _userDataService.Received(1).CanEditPage(expectedActionName);
        }

        [Fact]
        public async Task GenerateExcel_UserHasEditPermissions_ReturnsFileResult()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            var fileContent = new byte[] { 1, 2, 3 };
            var fileName = "report.xlsx";
            _reportService.GenerateExcelReportAsync().Returns((fileContent, fileName));

            // Act
            var result = await _controller.GenerateExcel();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal(fileContent, fileResult.FileContents);
            Assert.Equal(fileName, fileResult.FileDownloadName);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
        }

        [Fact]
        public async Task GenerateExcel_UserDoesNotHaveEditPermissions_RedirectsToReport()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.GenerateExcel();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Report", redirectResult.ActionName);
        }

        [Fact]
        public async Task GenerateExcel_SqlExceptionOccurs_LogsErrorAndRedirectsToReport()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _reportService.GenerateExcelReportAsync().Throws(CreateSqlException());

            // Act
            var result = await _controller.GenerateExcel();

            // Assert
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), Arg.Any<string>());
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Report", redirectResult.ActionName);
            Assert.Equal("Report generation failed due to database error.", _controller.TempData["ReportMessage"]);
        }

        [Fact]
        public async Task GenerateExcel_GeneralExceptionOccurs_LogsErrorAndRedirectsToReport()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _reportService.GenerateExcelReportAsync().Throws(new Exception());

            // Act
            var result = await _controller.GenerateExcel();

            // Assert
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), Arg.Any<string>());
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Report", redirectResult.ActionName);
            Assert.Equal("Report generation failed.", _controller.TempData["ReportMessage"]);
        }

        // Helper to create an uninitialized SqlException for testing
        private static SqlException CreateSqlException()
        {
            return (SqlException)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(SqlException));
        }
    }
}
