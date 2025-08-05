using System.Runtime.CompilerServices;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Apha.BST.Web.Models.SiteViewModel;

namespace Apha.BST.Web.Controllers
{
    [Authorize]
    public class SiteController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IMapper _mapper;
        private const string siteAll = "All";       

        public SiteController(ISiteService siteService, IMapper mapper)
        {
            _siteService = siteService;
            _mapper = mapper;          
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

        //Final code working for SiteTrainee
        public async Task<IActionResult> SiteTrainee(string? selectedSite = null)
        {
            // Get all sites for the dropdown
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
