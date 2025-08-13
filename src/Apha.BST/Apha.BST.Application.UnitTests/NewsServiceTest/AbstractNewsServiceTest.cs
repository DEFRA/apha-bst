using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using AutoMapper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Application.UnitTests.NewsServiceTest
{
    public abstract class AbstractNewsServiceTest
    {
        protected INewsService? _newsService;
        protected INewsRepository? _newsRepository;
        protected IMapper? _mapper;

        public void MockGetLatestNewsAsync(List<News> newsEntities, List<NewsDto> newsDtos)
        {
            var mockRepo = Substitute.For<INewsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetLatestNewsAsync().Returns(newsEntities);
            mockMapper.Map<IEnumerable<NewsDto>>(newsEntities).Returns(newsDtos);

            _newsService = new NewsService(mockRepo, mockMapper);
            _newsRepository = mockRepo;
            _mapper = mockMapper;
        }

        public void MockAddNewsAsync(NewsDto newsDto, News news, string paramName)
        {
            var mockRepo = Substitute.For<INewsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            if (newsDto == null)
            {
                mockMapper.When(x => x.Map<News>(Arg.Is<NewsDto>(x => x == null)))
                .Do(x => { throw new ArgumentNullException(paramName); });

            }
            else
            {
                mockMapper.Map<News>(newsDto).Returns(news);
            }

            mockRepo.AddNewsAsync(news).Returns(Task.CompletedTask);

            _newsService = new NewsService(mockRepo, mockMapper);
            _newsRepository = mockRepo;
            _mapper = mockMapper;
        }

        public void MockGetNewsAsync(List<News> newsEntities, List<NewsDto> newsDtos)
        {
            var mockRepo = Substitute.For<INewsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetNewsAsync().Returns(newsEntities);
            mockMapper.Map<List<NewsDto>>(newsEntities).Returns(newsDtos);

            _newsService = new NewsService(mockRepo, mockMapper);
            _newsRepository = mockRepo;
            _mapper = mockMapper;
        }

        public void MockDeleteNewsAsync(string title)
        {
            var mockRepo = Substitute.For<INewsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.DeleteNewsAsync(title).Returns(Task.CompletedTask);

            _newsService = new NewsService(mockRepo, mockMapper);
            _newsRepository = mockRepo;
            _mapper = mockMapper;
        }

        public void MockDeleteNewsAsyncWithError(string title, Exception exception)
        {
            var mockRepo = Substitute.For<INewsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.DeleteNewsAsync(title).Throws(exception);

            _newsService = new NewsService(mockRepo, mockMapper);
            _newsRepository = mockRepo;
            _mapper = mockMapper;
        }
    }
}
