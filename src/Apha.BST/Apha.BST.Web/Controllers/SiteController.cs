using System.Runtime.CompilerServices;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Apha.BST.Web.Models.SiteViewModel;

namespace Apha.BST.Web.Controllers
{
    public class SiteController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IMapper _mapper;
        private const string siteName = "All";
        private readonly ILogger<SiteController> _logger;

        public SiteController(ISiteService siteService, IMapper mapper, ILogger<SiteController> logger)
        {
            _siteService = siteService;
            _mapper = mapper;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewSite(string selectedSite = siteName)
        {
            var allSitesDto = await _siteService.GetAllSitesAsync(siteName);
            var allSites = _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto);

            IEnumerable<SiteViewModel> filteredSites = selectedSite == siteName
                ? allSites
                : allSites.Where(s => s.PlantNo == selectedSite);

            var model = new SiteListViewModel
            {
                AllSites = allSites,
                FilteredSites = filteredSites,
                SelectedSite = selectedSite
            };

            return View(model);
        }

        //Final code working for SiteTrainee
        [HttpGet]
        public async Task<IActionResult> SiteTrainee(string selectedSite = siteName)
        {
            // Get all sites for the dropdown
            var allSitesDto = await _siteService.GetAllSitesAsync(siteName);
            var allSites = _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto);

            // Get trainees for the selected site (or all if "All" is selected)
            IEnumerable<SiteTraineeViewModel> filteredTrainees = Enumerable.Empty<SiteTraineeViewModel>();
            if (selectedSite != siteName)
            {
                var traineeDtos = await _siteService.GetSiteTraineesAsync(selectedSite);
                filteredTrainees = _mapper.Map<IEnumerable<SiteTraineeViewModel>>(traineeDtos);
            }

            var model = new SiteTraineeListViewModel
            {
                AllSites = allSites,
                FilteredTrainees = filteredTrainees,
                SelectedSite = selectedSite
            };

            return View(model);
        }      

       
        [HttpGet]
        public IActionResult AddSite()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSite(SiteViewModel siteViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var site = _mapper.Map<SiteDTO>(siteViewModel);
                    var message = await _siteService.AddSiteAsync(site);

                    TempData["Message"] = message;
                }
                catch (Exception)
                {
                    TempData["Message"] = "Save failed";
                }

                return RedirectToAction(nameof(AddSite));
            }

            return View(siteViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrainee(int personId, string selectedSite)
        {
            try
            {
                var message = await _siteService.DeleteTraineeAsync(personId);
                TempData["message"] = message;
            }
            catch (Exception)
            {
                TempData["message"] = "Save failed";
            }

            return RedirectToAction("SiteTrainee", new { selectedSite });
        }
    }
}
