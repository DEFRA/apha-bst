using System.Diagnostics;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apha.BST.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly INewsService _newsServicee;
        private readonly IMapper _mapper;
       
        public HomeController(INewsService newsServicee, IMapper mapper)
        {
           
            _newsServicee = newsServicee;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var latestNewsDto = await _newsServicee.GetLatestNewsAsync();
            var latestNews = _mapper.Map<IEnumerable<NewsViewModel>>(latestNewsDto);
           
            return View(latestNews);
        }        
        
    }
}
