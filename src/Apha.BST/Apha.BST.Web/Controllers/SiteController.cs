using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Apha.BST.Web.Controllers
{
    //[Authorize]
    public class SiteController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IMapper _mapper;
        private readonly ILogger<SiteController> _logger;
        private const string siteAll = "All";       

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
        public async Task<IActionResult> ViewSite(string selectedSite = siteAll)
        {
            var allSitesDto = await _siteService.GetAllSitesAsync("All");
            var allSites = _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto);

            var siteSelectList = allSites
                .Select(site => new SelectListItem
                {
                    Value = site.PlantNo,
                    Text = site.Name,                   
                })
                .ToList();           

            IEnumerable<SiteViewModel> filteredSites = selectedSite == "All"
                ? allSites
                : allSites.Where(s => s.PlantNo == selectedSite);

            var model = new SiteListViewModel
            {
                AllSites = siteSelectList,
                FilteredSites = filteredSites,
                SelectedSite = selectedSite
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditSite(string plantNo)
        {
            if (string.IsNullOrEmpty(plantNo))
            {
                _logger.LogWarning("EditSite called with null or empty plantNo");
                return BadRequest("Plant number is required");
            }

            _logger.LogInformation("Fetching site details for plant number: {PlantNo}", plantNo);

            var siteDtos = await _siteService.GetAllSitesAsync(plantNo);
            var siteDto = siteDtos.FirstOrDefault();
            
            if (siteDto == null)
            {
                _logger.LogWarning("No site found for plant number: {PlantNo}", plantNo);
                return NotFound($"No site found with plant number: {plantNo}");
            }

            var editSiteViewModel = _mapper.Map<EditSiteViewModel>(siteDto);
            return View(editSiteViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSite(EditSiteViewModel editSiteViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editSiteViewModel);
            }

            try
            {
                var siteDto = _mapper.Map<SiteDto>(editSiteViewModel);
                var message = await _siteService.UpdateSiteAsync(siteDto);
                TempData["Message"] = message;
                
                    if (message.StartsWith("SUCCESS"))
                {
                    return RedirectToAction(nameof(ViewSite));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating site with plant number {PlantNo}", editSiteViewModel.PlantNo);
                TempData["Message"] = "Update failed";
            }

            return View(editSiteViewModel);
        }

        public async Task<IActionResult> SiteTrainee(string? selectedSite = null)
        {
            var allSitesDto = await _siteService.GetAllSitesAsync(siteAll);
            var allSites = _mapper.Map<IEnumerable<SiteViewModel>>(allSitesDto);

            var siteSelectList = allSites
            .Select(site => new SelectListItem
            {
                Value = site.PlantNo,
                Text = site.Name,
                Selected = (site.PlantNo == selectedSite)
            })
            .ToList();

            var model = new SiteTraineeListViewModel
            {
                AllSites = siteSelectList
            };

            if (selectedSite != null)
            {
                // Get trainees for the selected site (or all if "All" is selected)
                IEnumerable<SiteTraineeViewModel> filteredTrainees;

                var traineeDtos = await _siteService.GetSiteTraineesAsync(selectedSite);
                filteredTrainees = _mapper.Map<IEnumerable<SiteTraineeViewModel>>(traineeDtos);

                model.FilteredTrainees = filteredTrainees;
                model.SelectedSite = selectedSite;
            }

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
                    var site = _mapper.Map<SiteDto>(siteViewModel);
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
            if(!ModelState.IsValid)
            {
                TempData["message"] = "Invalid request";
                return RedirectToAction("SiteTrainee", new { selectedSite });
            }
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
