using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Web.PresentationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Apha.BST.Web.UnitTests.PresentationService
{
    public class UserDataServiceTests
    {
        private readonly IUserDataService _userDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataEntryService _dataEntryService;

        public UserDataServiceTests()
        {
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _dataEntryService = Substitute.For<IDataEntryService>();
            _userDataService = new UserDataService(_httpContextAccessor, _dataEntryService);
        }

        [Fact]
        public void GetUsername_ReturnsUserName_FromHttpContext()
        {
            // Arrange
            var userName = "testuser";
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) });
            var principal = new ClaimsPrincipal(identity);
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = principal;
            _httpContextAccessor.HttpContext.Returns(mockHttpContext);

            // Act
            var result = _userDataService.GetUsername();

            // Assert
            Assert.Equal(userName, result);
        }

        [Fact]
        public void GetUsername_ReturnsNull_WhenHttpContextIsNull()
        {
            // Arrange
            _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

            // Act
            var result = _userDataService.GetUsername();

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("2", true)]
        [InlineData("3", false)]
        [InlineData("4", false)]
        public async Task CanEditPage_ReturnsExpectedResult(string role, bool expectedResult)
        {
            // Arrange
            var action = "TestAction";
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User.AddIdentity(new ClaimsIdentity(new[] { new Claim("roleid", role) }));
            _httpContextAccessor.HttpContext.Returns(mockHttpContext);

            if (role == "2")
            {
                _dataEntryService.CanEditPage(action).Returns(true);
            }

            // Act
            var result = await _userDataService.CanEditPage(action);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}