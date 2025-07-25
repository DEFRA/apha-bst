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
        //private readonly ILogger<SiteController> _logger;

        public SiteController(ISiteService siteService, IMapper mapper/*, ILogger<SiteController> logger*/)
        {
            _siteService = siteService;
            _mapper = mapper;
            //_logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewSite(string selectedSite = "All")
        {
            var allSitesDto = await _siteService.GetAllSitesAsync("All");
            var allSites = _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto);

            IEnumerable<SiteViewModel> filteredSites = selectedSite == "All"
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
        public async Task<IActionResult> SiteTrainee(string selectedSite = "All")
        {
            // Get all sites for the dropdown
            var allSitesDto = await _siteService.GetAllSitesAsync("All");
            var allSites = _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto);

            // Get trainees for the selected site (or all if "All" is selected)
            IEnumerable<SiteTraineeViewModel> filteredTrainees = Enumerable.Empty<SiteTraineeViewModel>();
            if (selectedSite != "All")
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

        [HttpPost]
        public async Task<IActionResult> DeleteTrainee(int personId, string selectedSite)
        {
            var deleted = await _siteService.DeleteTraineeAsync(personId);
            if (deleted)
                TempData["message"] = "Trainee deleted successfully.";
            else
                //TempData["message"] = "Trainee not found.";
                TempData["message"] = "Trainee has training records. Delete them first if you wish to remove the person.";

            return RedirectToAction("SiteTrainee", new { selectedSite });
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
                var site = _mapper.Map<SiteDTO>(siteViewModel);
                var message = await _siteService.CreateSiteAsync(site);
                //TempData["Message"] = $"'{siteViewModel.Name}' saved as site";
                // Set TempData message based on the result
                if (message == "Site added successfully.")
                {
                    TempData["Message"] = $"'{siteViewModel.Name}' saved as site";
                }
                else
                {
                    TempData["Message"] = message; // "Site already exists."
                }
                return RedirectToAction(nameof(AddSite));
            }
            return View(siteViewModel);
        }
    }
}
