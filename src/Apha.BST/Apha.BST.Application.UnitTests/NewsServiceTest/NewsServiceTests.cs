using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Services;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using AutoMapper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Application.UnitTests.NewsServiceTest
{
    public class NewsServiceTests : AbstractNewsServiceTest
    {
        [Fact]
        public async Task GetLatestNewsAsync_ShouldReturnMappedNewsDtos()
        {
            // Arrange
            var newsEntities = new List<News>
            {
                new News { Title = "Test News 1", NewsContent = "Content 1" },
                new News { Title = "Test News 2", NewsContent = "Content 2" }
            };

            var newsDtos = new List<NewsDto>
            {
                new NewsDto { Title = "Test News 1", NewsContent = "Content 1" },
                new NewsDto { Title = "Test News 2", NewsContent = "Content 2" }
            };

            MockGetLatestNewsAsync(newsEntities, newsDtos);

            // Act
            var result = await _newsService!.GetLatestNewsAsync();

            // Assert
            await _newsRepository!.Received(1).GetLatestNewsAsync();
            _mapper!.Received(1).Map<IEnumerable<NewsDto>>(Arg.Is<IEnumerable<News>>(n => n == newsEntities));
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Test News 1", result.First().Title);
            Assert.Equal("Test News 2", result.Last().Title);
        }

        [Fact]
        public async Task AddNewsAsync_SuccessfulAddition_ReturnsCorrectString()
        {
            // Arrange
            var newsDto = new NewsDto { Title = "Test News" };
            var news = new News { Title = "Test News" };

            MockAddNewsAsync(newsDto, news,"dto");

            // Act
            var result = await _newsService!.AddNewsAsync(newsDto);

            // Assert
            await _newsRepository!.Received(1).AddNewsAsync(Arg.Is<News>(n => n.Title == "Test News"));
            Assert.Equal($"This news: {newsDto.Title} has been added to the list", result);
        }

        [Fact]
        public async Task AddNewsAsync_NullInput_ThrowsArgumentNullException()
        {
            // Arrange
            MockAddNewsAsync(null!, null!,null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _newsService!.AddNewsAsync(null!));
        }

        [Fact]
        public async Task AddNewsAsync_InvalidInput_ThrowsException()
        {
            // Arrange
            var invalidNewsDto = new NewsDto(); // Empty DTO
            var mockRepo = Substitute.For<INewsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockMapper.Map<News>(invalidNewsDto).Throws(new AutoMapperMappingException("Invalid mapping"));
            _newsService = new NewsService(mockRepo, mockMapper);

            // Act & Assert
            await Assert.ThrowsAsync<AutoMapperMappingException>(() => _newsService.AddNewsAsync(invalidNewsDto));
        }

        [Fact]
        public async Task AddNewsAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var newsDto = new NewsDto { Title = "Test News" };
            var news = new News { Title = "Test News" };

            var mockRepo = Substitute.For<INewsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockMapper.Map<News>(newsDto).Returns(news);
            mockRepo.AddNewsAsync(Arg.Any<News>()).Throws(new Exception("Repository error"));

            _newsService = new NewsService(mockRepo, mockMapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _newsService.AddNewsAsync(newsDto));
        }

        [Fact]
        public async Task GetNewsAsync_ShouldReturnListOfNewsDto()
        {
            // Arrange
            var newsList = new List<News>
            {
                new News { Title = "Test News 1", NewsContent = "Content 1" },
                new News { Title = "Test News 2", NewsContent = "Content 2" }
            };

            var expectedNewsDtoList = new List<NewsDto>
            {
                new NewsDto { Title = "Test News 1", NewsContent = "Content 1" },
                new NewsDto { Title = "Test News 2", NewsContent = "Content 2" }
            };

            MockGetNewsAsync(newsList, expectedNewsDtoList);

            // Act
            var result = await _newsService!.GetNewsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<NewsDto>>(result);
            Assert.Equal(expectedNewsDtoList.Count, result.Count);
            Assert.Equal("Test News 1", result[0].Title);
            Assert.Equal("Test News 2", result[1].Title);
        }

        [Fact]
        public async Task GetNewsAsync_ShouldCallRepositoryAndMapper()
        {
            // Arrange
            var newsList = new List<News>();
            var dtoList = new List<NewsDto>();

            MockGetNewsAsync(newsList, dtoList);

            // Act
            await _newsService!.GetNewsAsync();

            // Assert
            await _newsRepository!.Received(1).GetNewsAsync();
            _mapper!.Received(1).Map<List<NewsDto>>(Arg.Is<List<News>>(n => n == newsList));
        }

        [Fact]
        public async Task GetNewsAsync_WhenRepositoryThrowsException_ShouldThrowException()
        {
            // Arrange
            var mockRepo = Substitute.For<INewsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetNewsAsync().ThrowsAsync(new Exception("Repository error"));
            _newsService = new NewsService(mockRepo, mockMapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _newsService.GetNewsAsync());
        }

        [Fact]
        public async Task GetNewsAsync_ShouldReturnEmptyListWhenNoNews()
        {
            // Arrange
            var emptyNewsList = new List<News>();
            var emptyNewsDtoList = new List<NewsDto>();

            MockGetNewsAsync(emptyNewsList, emptyNewsDtoList);

            // Act
            var result = await _newsService!.GetNewsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<NewsDto>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteNewsAsync_SuccessfulDeletion_ReturnsCorrectMessage()
        {
            // Arrange
            var title = "Test News";

            MockDeleteNewsAsync(title);

            // Act
            var result = await _newsService!.DeleteNewsAsync(title);

            // Assert
            await _newsRepository!.Received(1).DeleteNewsAsync(title);
            Assert.Equal($"The news entitled: {title} has been deleted", result);
        }

        [Fact]
        public async Task DeleteNewsAsync_NonExistentNews_ThrowsException()
        {
            // Arrange
            var title = "Non-existent News";

            MockDeleteNewsAsyncWithError(title, new InvalidOperationException("News not found"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _newsService!.DeleteNewsAsync(title));
            await _newsRepository!.Received(1).DeleteNewsAsync(title);
        }

        [Fact]
        public async Task DeleteNewsAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var title = "Test News";

            MockDeleteNewsAsyncWithError(title, new Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _newsService!.DeleteNewsAsync(title));
            await _newsRepository!.Received(1).DeleteNewsAsync(title);
        }
    }
}
