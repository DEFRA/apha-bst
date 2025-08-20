using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Pagination;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Controllers
{
    public class AuditLogArchivedController : Controller
    {
        private readonly IAuditlogArchivedService _auditLogArchiveService;

        private readonly IMapper _mapper;
        public AuditLogArchivedController(IAuditlogArchivedService auditLogArchiveService, IMapper mapper)
        {
            _auditLogArchiveService = auditLogArchiveService ?? throw new ArgumentNullException(nameof(auditLogArchiveService));
            _mapper = mapper;

        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNo = 1, string column = "", bool sortOrder = false, string storedProcedure = "%")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid parameters");
            }
            QueryParameters queryParameters = new QueryParameters
            {
                Search = storedProcedure,
                Page = pageNo,
                PageSize = 30,
                SortBy = column,
                Descending = sortOrder

            };
            var auditLogsDto = await _auditLogArchiveService.GetArchiveAuditLogsAsync(queryParameters, storedProcedure);
            var auditLogs = _mapper.Map<IEnumerable<AuditLogsArchivedViewModel>>(auditLogsDto.data);


            List<string> storedProcedureNames = await _auditLogArchiveService.GetStoredProcedureNamesAsync();
            var dropDownList = storedProcedureNames
                .Select(name => new SelectListItem
                {
                    Value = name,
                    Text = name
                })
                .ToList();


            var model = new AuditLogArchivedListViewModel
            {
                AuditLogsResults = auditLogs.ToList(),
                StoredProcedures = dropDownList,
                StoredProcedure = storedProcedure,
                Pagination = new PaginationModel
                {
                    TotalCount = auditLogsDto.TotalCount,
                    PageNumber = queryParameters.Page,
                    PageSize = queryParameters.PageSize,
                    SortColumn = queryParameters.SortBy,
                    SortDirection = queryParameters.Descending
                }
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult CurrentAuditLog()
        {
            return RedirectToAction(nameof(Index), "AuditLog");
        }
    }
}
