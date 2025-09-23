using System.Globalization;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace Apha.BST.Web.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;
        private const string newsMessage = "NewsMessage";
        private const string newsLoadingMessage = "Error loading news";
       
        

        public NewsController(INewsService newsService, IUserService userService, IMapper mapper, IUserDataService userDataService, ILogService logService)
        {
            _newsService = newsService;
            _userService = userService;
            _mapper = mapper;
            _userDataService = userDataService;
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> AddNews()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            var viewModel = new AddNewsViewModel
            {
                
                Users = (await _userService.GetUsersAsync("All users"))
                    .Select(u => new SelectListItem { Value = u.UserId, Text = u.UserName })
                    .ToList(),
                CanEdit = canEdit
            };

            // Add a default selection prompt
            var usersList = viewModel.Users.ToList();
            usersList.Insert(0, new SelectListItem { Value = "", Text = "Please select user", Selected = true });
            viewModel.Users = usersList;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNews(AddNewsViewModel viewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            // Validate DatePublished string
            DateTime parsedDate;
           
            if (string.IsNullOrWhiteSpace(viewModel.DatePublished))
            {
                ModelState.AddModelError(nameof(viewModel.DatePublished), "Complete date and time");
            }
            else
            {
                bool success = TryParseCustomDate(viewModel.DatePublished, out parsedDate);
                if(!success)
                {
                    ModelState.AddModelError(nameof(viewModel.DatePublished), "Please enter valid date");
                }
                else
                {
                    viewModel.DatePublished = parsedDate.ToString();
                }
            }

            if (!ModelState.IsValid)
            {
                viewModel.Users = (await _userService.GetUsersAsync("All users"))
                    .Select(u => new SelectListItem { Value = u.UserId, Text = u.UserName })
                    .ToList();

                // Add a default selection prompt
                var usersList = viewModel.Users.ToList();
                usersList.Insert(0, new SelectListItem { Value = "", Text = "Please select user", Selected = true });
                viewModel.Users = usersList;
                viewModel.CanEdit = canEdit;

                return View(viewModel);
            }

            try
            {
                if (canEdit)
                {
                    // Map string DatePublished to DateTime for persistence
                    var dto = _mapper.Map<NewsDto>(viewModel);
                    TempData[newsMessage] = await _newsService.AddNewsAsync(dto);
                }               
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[newsMessage] = "Save failed: " + sqlEx.Message.ToString();
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[newsMessage] = "Save failed: " + ex.Message.ToString();
            }
            return RedirectToAction(nameof(AddNews));
        }
        [HttpGet]
        public async Task<IActionResult> ViewNews()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            var viewModel = new ViewNewsViewModel
            {
                CanEdit = canEdit
            };

            try
            {
                viewModel.NewsList = await _newsService.GetNewsAsync();

                if (TempData[newsMessage] != null)
                {
                    viewModel.Message = TempData[newsMessage]?.ToString();
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                viewModel.Message = newsLoadingMessage;
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                viewModel.Message = newsLoadingMessage;
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNews(string title)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            try
            {
                if (canEdit)
                {
                    var message = await _newsService.DeleteNewsAsync(title);
                    TempData[newsMessage] = message;
                }
                else
                {
                    TempData[newsMessage] = "You do not have permission to perform this action.";
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[newsMessage] = "Delete failed";
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[newsMessage] = "Delete failed";
            }
            return RedirectToAction(nameof(ViewNews));
        }

        [HttpGet]
        public async Task<IActionResult> OldNews()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);

            var viewModel = new OldNewsViewModel
            {
                CanEdit = canEdit
            };

            try
            {
                viewModel.NewsList = await _newsService.GetNewsAsync();

                if (TempData[newsMessage] != null)
                {
                    viewModel.Message = TempData[newsMessage]?.ToString();
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, "OldNews");
                viewModel.Message = newsLoadingMessage;
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, "OldNews");
                viewModel.Message = newsLoadingMessage;
            }
            return View(viewModel);
        }

        private static bool TryParseCustomDate(string input, out DateTime result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] acceptedFormats = new[]
            {
            "yyyy/MM/dd",
            "yyyy-MM-dd",            
            "dd/MM/yyyy",
            "dd-MM-yyyy",            
             // Add these for time + seconds + AM/PM           
            "yyyy-MM-dd h:mm:ss tt",
            "yyyy-MM-dd hh:mm:ss tt",
            "d/M/yyyy h:mm:ss tt",
            "d/M/yyyy hh:mm:ss tt",
            "dd/MM/yyyy h:mm:ss tt",
            "dd/MM/yyyy hh:mm:ss tt",
            // 24-hour time formats            
            "dd/MM/yyyy HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss"
        };

            return DateTime.TryParseExact(
                input,
                acceptedFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result
            );
        }
    }
}
