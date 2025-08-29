using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class UserControllerTests
    {
        private readonly IUserService _userService;
        private readonly IRoleMappingService _roleMappingService;
        private readonly IMapper _mapper;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;
        private readonly UserController _controller;
        private static SqlException CreateSqlException()
        {
            // This creates an uninitialized SqlException instance for testing purposes.
            return (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));
        }

        public UserControllerTests()
        {
            _userService = Substitute.For<IUserService>();
            _roleMappingService = Substitute.For<IRoleMappingService>();
            _mapper = Substitute.For<IMapper>();
            _userDataService = Substitute.For<IUserDataService>();
            _logService = Substitute.For<ILogService>();

            _controller = new UserController(_userService, _roleMappingService, _mapper, _userDataService, _logService);

            // Setup TempData for the controller
            var tempData = Substitute.For<ITempDataDictionary>();
            _controller.TempData = tempData;

            // Setup ControllerContext with ActionDescriptor
            var controllerActionDescriptor = new ControllerActionDescriptor { ActionName = "TestAction" };
            _controller.ControllerContext = new ControllerContext
            {
                ActionDescriptor = controllerActionDescriptor
            };
        }

        #region AddUser Tests

        [Fact]
        public async Task AddUser_GET_ReturnsViewWithModel()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetLocationsAsync().Returns(new List<VlaLocDto> { new VlaLocDto { LocId = "1", VlaLocation = "Loc1" } });
            _roleMappingService.GetUserLevels().Returns(new List<SelectListItem> { new SelectListItem { Value = "1", Text = "Admin" } });

            // Act
            var result = await _controller.AddUser();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddUserViewModel>(viewResult.Model);
            Assert.True(model.CanEdit);
            Assert.Single(model.Locations);
            Assert.Single(model.UserLevels);
        }

        [Fact]
        public async Task AddUser_POST_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Model is invalid");
            var viewModel = new AddUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetLocationsAsync().Returns(new List<VlaLocDto>());
            _roleMappingService.GetUserLevels().Returns(new List<SelectListItem>());

            // Act
            var result = await _controller.AddUser(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task AddUser_POST_UserHasPermission_AddsUserSuccessfully()
        {
            // Arrange
            var viewModel = new AddUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.AddUserAsync(Arg.Any<UserDto>()).Returns("User added successfully");
            _mapper.Map<UserDto>(viewModel).Returns(new UserDto());

            // Act
            var result = await _controller.AddUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "User added successfully";
            await _userService.Received(1).AddUserAsync(Arg.Any<UserDto>());
        }

        [Fact]
        public async Task AddUser_POST_UserDoesNotHavePermission_ReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new AddUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.AddUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "You do not have permission to perform this action.";
            await _userService.DidNotReceive().AddUserAsync(Arg.Any<UserDto>());
        }

        [Fact]
        public async Task AddUser_POST_SqlExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new AddUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<UserDto>(viewModel).Returns(new UserDto());
            _userService.AddUserAsync(Arg.Any<UserDto>()).Throws(CreateSqlException());

            // Act
            var result = await _controller.AddUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "Save failed: Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.";
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task AddUser_POST_GeneralExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new AddUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<UserDto>(viewModel).Returns(new UserDto());
            _userService.AddUserAsync(Arg.Any<UserDto>()).Throws(new Exception());

            // Act
            var result = await _controller.AddUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "Save failed: Exception of type 'System.Exception' was thrown.";
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }
        #endregion

        #region ViewUser Tests
        [Fact]
        public async Task ViewUser_GET_ReturnsViewWithModel()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetUsersAsync(Arg.Any<string>()).Returns(new List<UserViewDto> { new UserViewDto { UserId = "1", UserName = "Test" } });
            _roleMappingService.GetUserLevels().Returns(new List<SelectListItem>());

            // Act
            var result = await _controller.ViewUser();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewUserViewModel>(viewResult.Model);
            Assert.True(model.CanEdit);
            Assert.Single(model.UserList);
        }

        [Fact]
        public async Task ViewUser_AllUsers_ReturnsViewWithCorrectModel()
        {
            // Arrange
            const string selectedUserId = "All users";
            var users = new List<UserViewDto> { new UserViewDto { UserId = "1", UserName = "User1" } };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetUsersAsync(Arg.Any<string>()).Returns(users);

            // Act
            var result = await _controller.ViewUser(selectedUserId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewUserViewModel>(viewResult.Model);
            Assert.Equal(selectedUserId, model.SelectedUserId);
            Assert.True(model.CanEdit);
            Assert.NotEmpty(model.UserList);
            Assert.NotEmpty(model.Users);
        }

        [Fact]
        public async Task ViewUser_SpecificUser_ReturnsViewWithCorrectModel()
        {
            // Arrange
            const string selectedUserId = "User1";
            var users = new List<UserViewDto> { new UserViewDto { UserId = "User1", UserName = "User One" } };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetUsersAsync(selectedUserId).Returns(users);
            _userService.GetUsersAsync("All users").Returns(users);

            // Act
            var result = await _controller.ViewUser(selectedUserId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewUserViewModel>(viewResult.Model);
            Assert.Equal(selectedUserId, model.SelectedUserId);
            Assert.True(model.CanEdit);
            Assert.NotEmpty(model.UserList);
            Assert.NotEmpty(model.Users);
        }

        [Fact]
        public async Task ViewUser_UserCannotEdit_ReturnsViewWithCorrectModelAndEditFalse()
        {
            // Arrange
            const string selectedUserId = "All users";
            var users = new List<UserViewDto> { new UserViewDto { UserId = "1", UserName = "User1" } };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);
            _userService.GetUsersAsync(Arg.Any<string>()).Returns(users);

            // Act
            var result = await _controller.ViewUser(selectedUserId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewUserViewModel>(viewResult.Model);
            Assert.Equal(selectedUserId, model.SelectedUserId);
            Assert.False(model.CanEdit);
            Assert.NotEmpty(model.UserList);
            Assert.NotEmpty(model.Users);
        }
        #endregion

        #region EditUser Tests
        [Fact]
        public async Task EditUser_GET_ValidUser_ReturnsViewWithModel()
        {
            // Arrange
            var userId = "1";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetUserByIdAsync(userId).Returns(new UserDto { UserId = userId });
            _mapper.Map<EditUserViewModel>(Arg.Any<UserDto>()).Returns(new EditUserViewModel { UserId = userId });
            _userService.GetLocationsAsync().Returns(new List<VlaLocDto> { new VlaLocDto { LocId = "1", VlaLocation = "Loc1" } });
            _roleMappingService.GetUserLevels().Returns(new List<SelectListItem> { new SelectListItem { Value = "1", Text = "Admin" } });

            // Act
            var result = await _controller.EditUser(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditUserViewModel>(viewResult.Model);
            Assert.Equal(userId, model.UserId);
            Assert.True(model.CanEdit);
            Assert.Single(model.Locations);
            Assert.Single(model.UserLevels);
        }

        [Fact]
        public async Task EditUser_GET_InvalidUser_RedirectsToViewUser()
        {
            // Arrange
            var userId = "invalid";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetUserByIdAsync(userId).Returns((UserDto?)null);

            // Act
            var result = await _controller.EditUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "Invalid user.";
        }

        [Fact]
        public async Task EditUser_POST_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var viewModel = new EditUserViewModel();
            _controller.ModelState.AddModelError("Error", "Invalid");
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetLocationsAsync().Returns(new List<VlaLocDto>());
            _roleMappingService.GetUserLevels().Returns(new List<SelectListItem>());

            // Act
            var result = await _controller.EditUser(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task EditUser_POST_UserHasPermission_UpdatesUserSuccessfully()
        {
            // Arrange
            var viewModel = new EditUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<UserDto>(viewModel).Returns(new UserDto());
            _userService.UpdateUserAsync(Arg.Any<UserDto>()).Returns("User updated successfully");

            // Act
            var result = await _controller.EditUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "User updated successfully";
            await _userService.Received(1).UpdateUserAsync(Arg.Any<UserDto>());
        }

        [Fact]
        public async Task EditUser_POST_UserDoesNotHavePermission_ReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new EditUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.EditUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "You do not have permission to perform this action.";
            await _userService.DidNotReceive().UpdateUserAsync(Arg.Any<UserDto>());
        }

        [Fact]
        public async Task EditUser_POST_SqlExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new EditUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            // EditUser POST SqlException test
            _mapper.Map<UserDto>(viewModel).Returns(new UserDto());
            _userService.UpdateUserAsync(Arg.Any<UserDto>()).Throws(CreateSqlException());

            // Act
            var result = await _controller.EditUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "Update failed: Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.";
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task EditUser_POST_GeneralExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new EditUserViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<UserDto>(viewModel).Returns(new UserDto());
            _userService.UpdateUserAsync(Arg.Any<UserDto>()).Throws(new Exception());

            // Act
            var result = await _controller.EditUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "Update failed: Exception of type 'System.Exception' was thrown.";
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task EditUser_UserExists_ReturnsViewWithModel()
        {
            // Arrange
            string userId = "123";
            var userDto = new UserDto { UserId = userId, UserName = "TestUser" };
            var editUserViewModel = new EditUserViewModel { UserId = userId, UserName = "TestUser" };
            var locations = new List<VlaLocDto> { new VlaLocDto { LocId = "1", VlaLocation = "Location1" } };
            var userLevels = new List<SelectListItem> { new SelectListItem { Value = "1", Text = "Level1" } };

            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetUserByIdAsync(userId).Returns(userDto);
            _userService.GetLocationsAsync().Returns(locations);
            _roleMappingService.GetUserLevels().Returns(userLevels);
            _mapper.Map<EditUserViewModel>(userDto).Returns(editUserViewModel);

            // Act
            var result = await _controller.EditUser(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditUserViewModel>(viewResult.Model);
            Assert.Equal(userId, model.UserId);
            Assert.Equal("TestUser", model.UserName);
            Assert.True(model.CanEdit);
            Assert.Single(model.Locations);
            Assert.Single(model.UserLevels);
        }

        [Fact]
        public async Task EditUser_UserExistsButNoEditPermission_ReturnsViewWithCanEditFalse()
        {
            // Arrange
            string userId = "123";
            var userDto = new UserDto { UserId = userId, UserName = "TestUser" };
            var editUserViewModel = new EditUserViewModel { UserId = userId, UserName = "TestUser" };
            var locations = new List<VlaLocDto> { new VlaLocDto { LocId = "1", VlaLocation = "Location1" } };
            var userLevels = new List<SelectListItem> { new SelectListItem { Value = "1", Text = "Level1" } };

            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);
            _userService.GetUserByIdAsync(userId).Returns(userDto);
            _userService.GetLocationsAsync().Returns(locations);
            _roleMappingService.GetUserLevels().Returns(userLevels);
            _mapper.Map<EditUserViewModel>(userDto).Returns(editUserViewModel);

            // Act
            var result = await _controller.EditUser(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditUserViewModel>(viewResult.Model);
            Assert.False(model.CanEdit);
        }

        [Fact]
        public async Task EditUser_ValidInput_SuccessfulUpdate()
        {
            // Arrange
            var viewModel = new EditUserViewModel { UserId = "123" };
            var userDto = new UserDto();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<UserDto>(viewModel).Returns(userDto);
            _userService.UpdateUserAsync(userDto).Returns("User updated successfully");

            // Act
            var result = await _controller.EditUser(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "User updated successfully";
            await _userService.Received(1).UpdateUserAsync(userDto);
        }
        #endregion

        #region DeleteUser Tests
        [Fact]
        public async Task DeleteUser_UserHasPermission_DeletesUserSuccessfully()
        {
            // Arrange
            var userId = "1";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.DeleteUserAsync(userId).Returns("User deleted successfully");

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "User deleted successfully";
            await _userService.Received(1).DeleteUserAsync(userId);
        }

        [Fact]
        public async Task DeleteUser_UserDoesNotHavePermission_DoesNotDeleteUser()
        {
            // Arrange
            var userId = "1";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            await _userService.DidNotReceive().DeleteUserAsync(userId);
        }

        [Fact]
        public async Task DeleteUser_SqlExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var userId = "1";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            // DeleteUser SqlException test
            _userService.DeleteUserAsync(userId).Throws(CreateSqlException());
            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "Delete failed: Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.";
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task DeleteUser_GeneralExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var userId = "1";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.DeleteUserAsync(userId).Throws(new Exception());

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewUser", redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "Delete failed: Exception of type 'System.Exception' was thrown.";
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task DeleteUser_WithEditPermissions_ShouldDeleteUserAndRedirect()
        {
            // Arrange
            const string userId = "testUser";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.DeleteUserAsync(userId).Returns("User deleted successfully");

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            await _userDataService.Received(1).CanEditPage(Arg.Any<string>());
            await _userService.Received(1).DeleteUserAsync(userId);
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UserController.ViewUser), redirectResult.ActionName);
            _controller.TempData.Received()["UserMessage"] = "User deleted successfully";
        }

        [Fact]
        public async Task DeleteUser_WithoutEditPermissions_ShouldNotDeleteUserAndRedirect()
        {
            // Arrange
            const string userId = "testUser";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            await _userDataService.Received(1).CanEditPage(Arg.Any<string>());
            await _userService.DidNotReceive().DeleteUserAsync(Arg.Any<string>());
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UserController.ViewUser), redirectResult.ActionName);
        }
        #endregion
    }
}
