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
        private readonly ILogService _logService;

        public ReportController(IReportService reportService, ILogService logService)
        {
            _reportService = reportService;
          
            _logService = logService;
        }

        [HttpGet]
        public IActionResult Report()  
        {          

            var viewModel = new ReportViewModel
            {
                ReportTitle = "Reports"
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateExcel()
        {

            try
            {

                var (fileContent, fileName) = await _reportService.GenerateExcelReportAsync();
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

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
