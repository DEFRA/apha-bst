using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
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
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleMappingService _roleMappingService;
        private readonly IMapper _mapper;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;
        private const string userMessage = "UserMessage";

        public UserController(IUserService userService, IRoleMappingService roleMappingService, IMapper mapper, IUserDataService userDataService, ILogService logService)
        {
            _userService = userService;
            _roleMappingService = roleMappingService;
            _mapper = mapper;
            _userDataService = userDataService;
            _logService = logService;

        }

        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);

            var viewModel = new AddUserViewModel
            {
                Locations = (await _userService.GetLocationsAsync())
                    .Select(l => new SelectListItem { Value = l.LocId, Text = l.VlaLocation })
                    .ToList(),
                UserLevels = _roleMappingService.GetUserLevels(),
                UserLevel = 0, // Default to "0" for User Level
                CanEdit = canEdit
            };
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(AddUserViewModel viewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                viewModel.Locations = (await _userService.GetLocationsAsync())
                    .Select(l => new SelectListItem { Value = l.LocId, Text = l.VlaLocation })
                    .ToList();
                viewModel.UserLevels = _roleMappingService.GetUserLevels();
                viewModel.CanEdit = canEdit;
                return View(viewModel);
            }

            try
            {
                if (canEdit)
                {                   
                    var dto = _mapper.Map<UserDto>(viewModel);
                    TempData[userMessage] = await _userService.AddUserAsync(dto);
                }
                else
                {
                    TempData[userMessage] = "You do not have permission to perform this action.";
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[userMessage] = "Save failed: "+sqlEx.Message.ToString();
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[userMessage] = "Save failed: " + ex.Message.ToString();
            }
            return RedirectToAction(nameof(AddUser));
        }

        [HttpGet]
        public async Task<IActionResult> ViewUser(string selectedUserId = "All users")
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);

            var allUsers = await _userService.GetUsersAsync(selectedUserId);
            var userList = await _userService.GetUsersAsync("All users");

            var viewModel = new ViewUserViewModel
            {
                SelectedUserId = selectedUserId,
                Users = userList.Select(u => new SelectListItem { Value = u.UserId, Text = u.UserName }),
                UserList = allUsers,
                CanEdit = canEdit
            };
            return View(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            var dto = await _userService.GetUserByIdAsync(userId);
            if (dto == null)
            {
                TempData[userMessage] = "Invalid user.";
                return RedirectToAction(nameof(ViewUser));
            }

            var viewModel = _mapper.Map<EditUserViewModel>(dto);
            viewModel.Locations = (await _userService.GetLocationsAsync()).Select(l => new SelectListItem { Value = l.LocId, Text = l.VlaLocation });
            viewModel.UserLevels = _roleMappingService.GetUserLevels();
            viewModel.CanEdit = canEdit;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel viewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                viewModel.Locations = (await _userService.GetLocationsAsync()).Select(l => new SelectListItem { Value = l.LocId, Text = l.VlaLocation });
                viewModel.UserLevels = _roleMappingService.GetUserLevels();
                viewModel.CanEdit = canEdit;
                return View(viewModel);
            }
            try
            {
                if (canEdit)
                {
                    var dto = _mapper.Map<UserDto>(viewModel);
                    TempData[userMessage] = await _userService.UpdateUserAsync(dto);
                }
                else
                {
                    TempData[userMessage] = "You do not have permission to perform this action.";
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[userMessage] = "Update failed: " + sqlEx.Message.ToString();
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[userMessage] = "Update failed: " + ex.Message.ToString();
            }
            return RedirectToAction(nameof(EditUser), new {userId=viewModel.UserId});
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            try
            {
                if (canEdit)
                {
                    var message = await _userService.DeleteUserAsync(userId);
                    TempData[userMessage] = message;
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[userMessage] = "Delete failed: "+sqlEx.Message.ToString();
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[userMessage] = "Delete failed: " + ex.Message.ToString();
            }
            return RedirectToAction(nameof(ViewUser));
        }
    }
}
