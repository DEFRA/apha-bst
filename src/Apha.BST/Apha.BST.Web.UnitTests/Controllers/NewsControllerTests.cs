using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Runtime.CompilerServices;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class NewsControllerTests
    {
        private readonly INewsService _newsService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;
        private readonly NewsController _controller;

        private static SqlException CreateSqlException()
        {
            // This creates an uninitialized SqlException instance for testing purposes.
            return (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));
        }

        public NewsControllerTests()
        {
            _newsService = Substitute.For<INewsService>();
            _userService = Substitute.For<IUserService>();
            _mapper = Substitute.For<IMapper>();
            _userDataService = Substitute.For<IUserDataService>();
            _logService = Substitute.For<ILogService>();

            _controller = new NewsController(_newsService, _userService, _mapper, _userDataService, _logService);

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

        #region AddNews GET Tests
        [Fact]
        public async Task AddNews_GET_ReturnsViewWithModel()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetUsersAsync("All users").Returns(new List<UserViewDto>
            {
                new UserViewDto { UserId = "1", UserName = "User1" }
            });

            // Act
            var result = await _controller.AddNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddNewsViewModel>(viewResult.Model);
            Assert.True(model.CanEdit);
            Assert.NotEmpty(model.Users);
            // Check the default selection item was added
            Assert.Equal("Please select user", model.Users.First().Text);
        }
        #endregion

        #region AddNews POST Tests
        [Fact]
        public async Task AddNews_ValidInput_UseCurrentDateTime_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddNewsViewModel
            {
                Title = "Test News",
                NewsContent = "Test Content",
                Author = "user1",
                UseCurrentDateTime = true
            };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<NewsDto>(Arg.Any<AddNewsViewModel>()).Returns(new NewsDto());
            _newsService.AddNewsAsync(Arg.Any<NewsDto>()).Returns("News added successfully");

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
            Assert.Equal("News added successfully", _controller.TempData["NewsMessage"]);
        }

        [Fact]
        public async Task AddNews_ValidInput_CustomDateTime_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddNewsViewModel
            {
                Title = "Test News",
                NewsContent = "Test Content",
                Author = "user1",
                UseCurrentDateTime = false,
                DatePublished = "2023-05-01 10:00:00"
            };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<NewsDto>(Arg.Any<AddNewsViewModel>()).Returns(new NewsDto());
            _newsService.AddNewsAsync(Arg.Any<NewsDto>()).Returns("News added successfully");

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
            Assert.Equal("News added successfully", _controller.TempData["NewsMessage"]);
        }
        [Fact]
        public async Task AddNews_InvalidInput_MissingRequiredFields_ReturnsViewResult()
        {
            // Arrange
            var viewModel = new AddNewsViewModel
            {
                Title = "Test News",
                NewsContent = "Test Content",
                Author = "user1",
                UseCurrentDateTime = false,
                DatePublished = "Invalid Date"
            };
            _controller.ModelState.AddModelError("Title", "Required");
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            // Add this setup to prevent the null reference exception
            _userService.GetUsersAsync("All users").Returns(new List<UserViewDto>
            {
                new UserViewDto { UserId = "1", UserName = "User1" }
            });
            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AddNewsViewModel>(viewResult.Model);
        }
        [Fact]
        public async Task AddNews_InvalidInput_InvalidDateString_ReturnsViewResult()
        {
            // Arrange
            var viewModel = new AddNewsViewModel
            {
                Title = "Test News",
                NewsContent = "Test Content",
                Author = "user1",
                UseCurrentDateTime = false,
                DatePublished = "Invalid Date"
            };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
           
            _userService.GetUsersAsync("All users").Returns(new List<UserViewDto>
            {
                new UserViewDto { UserId = "1", UserName = "User1" }
            });

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AddNewsViewModel>(viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("DatePublished"));
        }
      
        

        [Fact]
        public async Task AddNews_UserWithoutEditPermissions_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddNewsViewModel
            {
                Title = "Test News",
                NewsContent = "Test Content",
                Author = "user1",
                UseCurrentDateTime = true
            };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
            Assert.Null(_controller.TempData["NewsMessage"]);
        }

        [Fact]
        public async Task AddNews_SqlException_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddNewsViewModel
            {
                Title = "Test News",
                NewsContent = "Test Content",
                Author = "user1",
                UseCurrentDateTime = true
            };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<NewsDto>(Arg.Any<AddNewsViewModel>()).Returns(new NewsDto());
            _newsService.AddNewsAsync(Arg.Any<NewsDto>()).Throws(CreateSqlException());

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
            var newsMessage = _controller.TempData["NewsMessage"]?.ToString() ?? string.Empty;
            Assert.StartsWith("Save failed:", newsMessage);
        }
        

        #endregion

        #region ViewNews Tests
        [Fact]
        public async Task ViewNews_ReturnsViewWithModel()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.GetNewsAsync().Returns(new List<NewsDto> { new NewsDto { Title = "Test News" } });

            // Act
            var result = await _controller.ViewNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewNewsViewModel>(viewResult.Model);
            Assert.True(model.CanEdit);
            Assert.NotEmpty(model.NewsList);
        }

        [Fact]
        public async Task ViewNews_WithTempData_SetsTempDataMessage()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);
            _newsService.GetNewsAsync().Returns(new List<NewsDto>());
            _controller.TempData["NewsMessage"] = "Test Message";

            // Act
            var result = await _controller.ViewNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewNewsViewModel>(viewResult.Model);
            Assert.Equal("Test Message", model.Message);
        }

        [Fact]
        public async Task ViewNews_SqlExceptionThrown_LogsExceptionAndSetsErrorMessage()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.GetNewsAsync().Throws(CreateSqlException());

            // Act
            var result = await _controller.ViewNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewNewsViewModel>(viewResult.Model);
            Assert.Equal("Error loading news", model.Message);
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task ViewNews_GeneralExceptionThrown_LogsExceptionAndSetsErrorMessage()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.GetNewsAsync().Throws(new Exception());

            // Act
            var result = await _controller.ViewNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewNewsViewModel>(viewResult.Model);
            Assert.Equal("Error loading news", model.Message);
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }
        #endregion

        #region DeleteNews Tests
        [Fact]
        public async Task DeleteNews_UserHasPermission_DeletesNewsSuccessfully()
        {
            // Arrange
            var title = "Test News";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.DeleteNewsAsync(title).Returns("News deleted successfully");

            // Act
            var result = await _controller.DeleteNews(title);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewNews", redirectResult.ActionName);
            _controller.TempData.Received()["NewsMessage"] = "News deleted successfully";
            await _newsService.Received(1).DeleteNewsAsync(title);
        }

        [Fact]
        public async Task DeleteNews_UserDoesNotHavePermission_DoesNotDeleteNews()
        {
            // Arrange
            var title = "Test News";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.DeleteNews(title);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewNews", redirectResult.ActionName);
            _controller.TempData.Received()["NewsMessage"] = "You do not have permission to perform this action.";
            await _newsService.DidNotReceive().DeleteNewsAsync(title);
        }

        [Fact]
        public async Task DeleteNews_SqlExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var title = "Test News";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.DeleteNewsAsync(title).Throws(CreateSqlException());

            // Act
            var result = await _controller.DeleteNews(title);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewNews", redirectResult.ActionName);
            _controller.TempData.Received()["NewsMessage"] = "Delete failed";
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task DeleteNews_GeneralExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var title = "Test News";
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.DeleteNewsAsync(title).Throws(new Exception());

            // Act
            var result = await _controller.DeleteNews(title);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewNews", redirectResult.ActionName);
            _controller.TempData.Received()["NewsMessage"] = "Delete failed";
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }
        #endregion

        #region OldNews Tests
        [Fact]
        public async Task OldNews_ReturnsViewWithModel()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.GetNewsAsync().Returns(new List<NewsDto> { new NewsDto { Title = "Test News" } });

            // Act
            var result = await _controller.OldNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<OldNewsViewModel>(viewResult.Model);
            Assert.True(model.CanEdit);
            Assert.NotEmpty(model.NewsList);
        }

        [Fact]
        public async Task OldNews_WithTempData_SetsTempDataMessage()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);
            _newsService.GetNewsAsync().Returns(new List<NewsDto>());
            _controller.TempData["NewsMessage"] = "Test Message";

            // Act
            var result = await _controller.OldNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<OldNewsViewModel>(viewResult.Model);
            Assert.Equal("Test Message", model.Message);
        }

        [Fact]
        public async Task OldNews_SqlExceptionThrown_LogsExceptionAndSetsErrorMessage()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.GetNewsAsync().Throws(CreateSqlException());

            // Act
            var result = await _controller.OldNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<OldNewsViewModel>(viewResult.Model);
            Assert.Equal("Error loading news", model.Message);
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), "OldNews");
        }

        [Fact]
        public async Task OldNews_GeneralExceptionThrown_LogsExceptionAndSetsErrorMessage()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _newsService.GetNewsAsync().Throws(new Exception());

            // Act
            var result = await _controller.OldNews();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<OldNewsViewModel>(viewResult.Model);
            Assert.Equal("Error loading news", model.Message);
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), "OldNews");
        }
        #endregion
    }
}
