using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Apha.BST.Web.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;

        public ReportController(IReportService reportService,IUserDataService userDataService,ILogService logService)
        {
            _reportService = reportService;
            _userDataService = userDataService;
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> Report()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);

            var viewModel = new ReportViewModel
            {
                ReportTitle = "Reports",
                CanEdit = canEdit
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateExcel()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);            

            try
            {
                if (canEdit)
                {                   
                    var (fileContent, fileName) = await _reportService.GenerateExcelReportAsync();
                    return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
                
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData["ReportMessage"] = "Report generation failed due to database error.";
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData["ReportMessage"] = "Report generation failed.";
            }

            return RedirectToAction(nameof(Report));
        }
    }
}
