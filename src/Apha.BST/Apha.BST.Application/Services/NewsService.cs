using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Repositories;
using AutoMapper;

namespace Apha.BST.Application.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;
        public NewsService(INewsRepository newseRepository, IMapper mapper)
        {
            _newsRepository = newseRepository ?? throw new ArgumentNullException(nameof(newseRepository));
            _mapper = mapper;
        }

        public async Task<IEnumerable<NewsDto>> GetLatestNewsAsync()
        {
            var latestNews = await _newsRepository.GetLatestNewsAsync();
            return _mapper.Map<IEnumerable<NewsDto>>(latestNews);
        }
    }
}
