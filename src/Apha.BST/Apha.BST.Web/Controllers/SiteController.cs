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

        //[HttpGet]
        //public async Task<IActionResult> SiteTrainee(string selectedSite)
        //{
        //    var allSites = await _siteService.GetAllSitesAsync(); // Implement as needed
        //    var trainees = string.IsNullOrEmpty(selectedSite) || selectedSite == "All"
        //        ? new List<SiteTraineeDTO>()
        //        : await _siteService.GetSiteTraineesAsync(selectedSite);

        //    var model = new SiteTraineeViewModel
        //    {
        //        SelectedSite = selectedSite,
        //        AllSites = allSites.Select(s => new SelectListItem { Value = s.PlantNo, Text = s.Name }).ToList(),
        //        Trainees = trainees
        //    };
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> SiteTrainee(string selectedSite)
        //{
        //    //var traineeDTOs = await _siteService.GetSiteTraineesAsync(selectedSite);
        //    //var trainees = _mapper.Map<List<SiteTraineeViewModel>>(traineeDTOs);

        //    //var viewModel = new SiteTraineeListViewModel
        //    //{
        //    //    SelectedSite = selectedSite,
        //    //    AllSites = allSites,
        //    //    Trainees = trainees
        //    //};

        //    //return View(viewModel);
        //    var traineeDTOs = await _siteService.GetSiteTraineesAsync(selectedSite);
        //    var viewModel = new SiteTraineeListViewModel
        //    {
        //        SelectedSite = selectedSite,
        //        AllSites = allSites, // however you populate this
        //        Trainees = traineeDTOs.Select(dto => new SiteTraineeViewModel
        //        {
        //            PersonId = dto.PersonId,
        //            Person = dto.Person,
        //            Cattle = dto.Cattle,
        //            Sheep = dto.Sheep,
        //            Goats = dto.Goats
        //        }).ToList()
        //    };
        //    return View(viewModel);
        //}

        ////Final code working for SiteTrainee
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
        public async Task<IActionResult> DeleteTrainee(int personId)
        {
            var deleted = await _siteService.DeleteTraineeAsync(personId);
            if (deleted)
                TempData["message"] = "Trainee deleted successfully.";
            else
                TempData["message"] = "Trainee not found.";

            return RedirectToAction("SiteTrainee");
        }

        //public async Task<IActionResult> SiteTrainee(string selectedSite)
        //{
        //    var trainees = string.IsNullOrEmpty(selectedSite)
        //        ? new List<SiteTraineeDTO>()
        //        : await _siteService.GetSiteTraineesAsync(selectedSite);

        //    var model = new SiteTraineeListViewModel
        //    {
        //        SelectedSite = selectedSite,
        //        Trainees = trainees
        //    };

        //    return View(model);
        //}

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
                //await _siteService.CreateSiteAsync(site);
                var message = await _siteService.CreateSiteAsync(site);
                //TempData["Message"] = $"'{siteViewModel.Name}' saved as site";
                // Set TempData message based on the result
                if (message == "Site added successfully.")
                {
                    TempData["Message"] = $"'{siteViewModel.Name}' saved as site";
                }
                else
                {
                    TempData["Message"] = message; // e.g., "Site already exists."
                }
                return RedirectToAction(nameof(AddSite));
            }
            return View(siteViewModel);
        }


        //[HttpGet]
        //public async Task<IActionResult> ViewSite()
        //{
        //    var sites = await _siteService.GetAllSitesAsync();
        //    var viewModels = _mapper.Map<IEnumerable<SiteViewModel>>(sites);
        //    return View(viewModels);
        //}

        //[HttpGet]
        //public IActionResult ViewSite()
        //{
        //    var sites = await _siteService.GetAllSitesAsync();
        //    return View(sites);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(SiteViewModel siteViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var site = _mapper.Map<Site>(siteViewModel);
        //            var result = await _siteService.AddSiteAsync(site);
        //            //if (result.IsSuccess)
        //            //{
        //            //    TempData["SuccessMessage"] = $"'{site.Name}' saved as site";
        //            //    return RedirectToAction(nameof(Index));
        //            //}
        //            //else
        //            //{
        //            //    ModelState.AddModelError("", result.ErrorMessage);
        //            //}
        //            if (result > 0)
        //            {
        //                TempData["SuccessMessage"] = $"'{site.Name}' saved as site";
        //                return RedirectToAction(nameof(Index));
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Failed to save site.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error occurred while creating site");
        //            ModelState.AddModelError("", "Save failed: " + ex.Message);
        //        }
        //    }
        //    return View(siteViewModel);
        //}
    }
}
