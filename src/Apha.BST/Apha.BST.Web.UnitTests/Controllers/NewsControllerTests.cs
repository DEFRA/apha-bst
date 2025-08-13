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
            Assert.Equal(DateTime.Now.Date, model.DatePublished.Date);
            // Check the default selection item was added
            Assert.Equal("Please select user", model.Users.First().Text);
        }
        #endregion

        #region AddNews POST Tests
        [Fact]
        public async Task AddNews_POST_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Title is required");
            var viewModel = new AddNewsViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userService.GetUsersAsync("All users").Returns(new List<UserViewDto>
            {
                new UserViewDto { UserId = "1", UserName = "User1" }
            });

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddNewsViewModel>(viewResult.Model);
            Assert.Equal(viewModel, model);
            Assert.NotEmpty(model.Users);
        }

        [Fact]
        public async Task AddNews_POST_UseCurrentDateTime_UpdatesDatePublished()
        {
            // Arrange
            var viewModel = new AddNewsViewModel { UseCurrentDateTime = true };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Create a partial mock of the controller to bypass TryValidateModel
            var partialMock = Substitute.ForPartsOf<NewsController>(
                _newsService, _userService, _mapper, _userDataService, _logService);

            // Configure the mock to avoid the NullReferenceException
            partialMock.When(x => x.TryValidateModel(Arg.Any<object>(), Arg.Any<string>()))
                .DoNotCallBase();

            // Make sure to return true when TryValidateModel is called
            partialMock.TryValidateModel(Arg.Any<object>(), Arg.Any<string>()).Returns(true);

            // Set up the ControllerContext and TempData
            partialMock.ControllerContext = new ControllerContext
            {
                ActionDescriptor = new ControllerActionDescriptor { ActionName = "TestAction" }
            };
            partialMock.TempData = Substitute.For<ITempDataDictionary>();

            // Set up other mocks
            _mapper.Map<NewsDto>(viewModel).Returns(new NewsDto());
            _newsService.AddNewsAsync(Arg.Any<NewsDto>()).Returns("News added successfully");

            // Act
            var result = await partialMock.AddNews(viewModel);

            // Assert
            // Check that DatePublished was updated to the current date
            Assert.Equal(DateTime.Now.Date, viewModel.DatePublished.Date);
            // Verify we got the expected redirect
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
        }
        [Fact]
        public async Task AddNews_POST_DateOnlyInput_PreservesDateWithMidnightTime()
        {
            // Arrange
            var dateOnly = new DateTime(2025, 12, 8, 0, 0, 0, DateTimeKind.Local);
            var viewModel = new AddNewsViewModel { DatePublished = dateOnly };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Create a partial mock of the controller to bypass TryValidateModel
            var partialMock = Substitute.ForPartsOf<NewsController>(
                _newsService, _userService, _mapper, _userDataService, _logService);

            // Configure the mock to avoid the NullReferenceException
            partialMock.When(x => x.TryValidateModel(Arg.Any<object>(), Arg.Any<string>()))
                .DoNotCallBase();

            // Make sure to return true when TryValidateModel is called
            partialMock.TryValidateModel(Arg.Any<object>(), Arg.Any<string>()).Returns(true);

            // Set up the ControllerContext and TempData
            partialMock.ControllerContext = new ControllerContext
            {
                ActionDescriptor = new ControllerActionDescriptor { ActionName = "TestAction" }
            };
            partialMock.TempData = Substitute.For<ITempDataDictionary>();

            // Set up other mocks
            _mapper.Map<NewsDto>(viewModel).Returns(new NewsDto());
            _newsService.AddNewsAsync(Arg.Any<NewsDto>()).Returns("News added successfully");

            // Act
            var result = await partialMock.AddNews(viewModel);

            // Assert
            // Verify the time remains at midnight (00:00:00)
            Assert.Equal(0, viewModel.DatePublished.Hour);
            Assert.Equal(0, viewModel.DatePublished.Minute);
            Assert.Equal(0, viewModel.DatePublished.Second);

            // Verify the date is preserved
            Assert.Equal(2025, viewModel.DatePublished.Year);
            Assert.Equal(12, viewModel.DatePublished.Month);
            Assert.Equal(8, viewModel.DatePublished.Day);

            // Verify DateTimeKind is Local
            Assert.Equal(DateTimeKind.Local, viewModel.DatePublished.Kind);

            // Verify we got the expected redirect
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
        }



        [Fact]
        public async Task AddNews_POST_UserHasPermission_AddsNewsSuccessfully()
        {
            // Arrange
            var viewModel = new AddNewsViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<NewsDto>(viewModel).Returns(new NewsDto());
            _newsService.AddNewsAsync(Arg.Any<NewsDto>()).Returns("News added successfully");

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
            _controller.TempData.Received()["NewsMessage"] = "News added successfully";
            await _newsService.Received(1).AddNewsAsync(Arg.Any<NewsDto>());
        }

        [Fact]
        public async Task AddNews_POST_UserDoesNotHavePermission_ReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new AddNewsViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
            _controller.TempData.Received()["NewsMessage"] = "You do not have permission to perform this action.";
            await _newsService.DidNotReceive().AddNewsAsync(Arg.Any<NewsDto>());
        }

        [Fact]
        public async Task AddNews_POST_SqlExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new AddNewsViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<NewsDto>(viewModel).Returns(new NewsDto());
            _newsService.AddNewsAsync(Arg.Any<NewsDto>()).Throws(CreateSqlException());

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
            _controller.TempData.Received()["NewsMessage"] = "Save failed";
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task AddNews_POST_GeneralExceptionThrown_LogsExceptionAndReturnsErrorMessage()
        {
            // Arrange
            var viewModel = new AddNewsViewModel();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<NewsDto>(viewModel).Returns(new NewsDto());
            _newsService.AddNewsAsync(Arg.Any<NewsDto>()).Throws(new Exception());

            // Act
            var result = await _controller.AddNews(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddNews", redirectResult.ActionName);
            _controller.TempData.Received()["NewsMessage"] = "Save failed";
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
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
