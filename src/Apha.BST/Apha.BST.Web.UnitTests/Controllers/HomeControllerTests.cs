using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Diagnostics;


namespace Apha.BST.Web.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        private readonly INewsService _mockNewsService;
        private readonly IMapper _mockMapper;
        private readonly HomeController _controller;
        private readonly HttpContext _httpContext;

        public HomeControllerTests()
        {
            _mockNewsService = Substitute.For<INewsService>();
            _mockMapper = Substitute.For<IMapper>();
            _httpContext = new DefaultHttpContext();
            _controller = new HomeController(_mockNewsService, _mockMapper)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };
        }

     

        [Fact]
        public async Task Index_CallsGetLatestNewsAsync()
        {
            // Arrange
            _mockNewsService.GetLatestNewsAsync().Returns(new List<NewsDto>());
            _mockMapper.Map<IEnumerable<NewsViewModel>>(Arg.Any<IEnumerable<NewsDto>>()).Returns(new List<NewsViewModel>());

            // Act
            await _controller.Index();

            // Assert
            await _mockNewsService.Received(1).GetLatestNewsAsync();
        }

        [Fact]
        public async Task Index_MapsNewsDtoToNewsViewModel()
        {
            // Arrange
            var newsDtos = new List<NewsDto>();
            _mockNewsService.GetLatestNewsAsync().Returns(newsDtos);

            // Act
            await _controller.Index();

            // Assert
            _mockMapper.Received(1).Map<IEnumerable<NewsViewModel>>(Arg.Is<IEnumerable<NewsDto>>(dto => dto == newsDtos));
        }

        [Fact]
        public async Task Index_ReturnsViewWithNewsItems_WhenNewsItemsExist()
        {
            // Arrange
            var newsDtos = new List<NewsDto>
{
new() { Title = "News 1", NewsContent = "Content 1", DatePublished = DateTime.Now, Author = "Author 1" },
new() { Title = "News 2", NewsContent = "Content 2", DatePublished = DateTime.Now, Author = "Author 2" }
};

            var newsViewModels = new List<NewsViewModel>
{
new() { Title = "News 1", NewsContent = "Content 1", DatePublished = DateTime.Now, Author = "Author 1" },
new() { Title = "News 2", NewsContent = "Content 2", DatePublished = DateTime.Now, Author = "Author 2" }
};

            _mockNewsService.GetLatestNewsAsync().Returns(newsDtos);
            _mockMapper.Map<IEnumerable<NewsViewModel>>(Arg.Any<IEnumerable<NewsDto>>()).Returns(newsViewModels);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<NewsViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_ReturnsViewWithEmptyList_WhenNoNewsItemsExist()
        {
            // Arrange
            var emptyNewsDtos = new List<NewsDto>();
            var emptyNewsViewModels = new List<NewsViewModel>();

            _mockNewsService.GetLatestNewsAsync().Returns(emptyNewsDtos);
            _mockMapper.Map<IEnumerable<NewsViewModel>>(Arg.Any<IEnumerable<NewsDto>>()).Returns(emptyNewsViewModels);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<NewsViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

    }
}



