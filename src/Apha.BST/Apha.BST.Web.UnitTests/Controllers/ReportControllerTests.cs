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
       
        private readonly ILogService _logService;
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            _reportService = Substitute.For<IReportService>();
           
            _logService = Substitute.For<ILogService>();
            _controller = new ReportController(_reportService,  _logService);

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
        public void Report_SuccessfulExecution_ReturnsViewResult()
        {
            // Arrange           

            // Act
            var result =  _controller.Report();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Report_VerifyViewModelProperties_ReturnsCorrectProperties()
        {
            // Act
            var result =  _controller.Report();
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ReportViewModel>(viewResult.Model);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal("Reports", viewModel.ReportTitle);
           
        }           
             

        [Fact]
        public async Task GenerateExcel_UserHasEditPermissions_ReturnsFileResult()
        {
            // Arrange           
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
        public async Task GenerateExcel_SqlExceptionOccurs_LogsErrorAndRedirectsToReport()
        {
            // Arrange            
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
