using System.Runtime.CompilerServices;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using static Apha.BST.Web.Models.SiteViewModel;

namespace Apha.BST.Web.Controllers
{
    [Authorize]
    public class SiteController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IUserDataService _userDataService;
        private readonly ILogger<SiteController> _logger;
        private readonly IMapper _mapper;
        private const string siteAll = "All";       

        public SiteController(ILogger<SiteController> logger,ISiteService siteService, IMapper mapper,IUserDataService userDataService)
        {
            _siteService = siteService;
            _mapper = mapper;
            _userDataService = userDataService;
            _logger = logger;
        }       

        [HttpGet]
        public async Task<IActionResult> ViewSite(string selectedSite = siteAll)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
           

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
                SelectedSite = selectedSite,
                CanEdit = canEdit
            };

            return View(model);
        }

        //Final code working for SiteTrainee
        public async Task<IActionResult> SiteTrainee(string? selectedSite = null)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
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

                AllSites = siteSelectList,
                CanEdit = canEdit
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
        public async Task<IActionResult> AddSite()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            SiteViewModel model = new SiteViewModel
            {
                PlantNo = string.Empty,
                Name = string.Empty,
                CanEdit = canEdit, // Assuming the user can edit when adding a site
            };
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSite(SiteViewModel siteViewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (ModelState.IsValid)
            {
                try
                {
                    string userName = _userDataService.GetUsername() ?? "";
                    if (canEdit)
                    {
                        var site = _mapper.Map<SiteDto>(siteViewModel);
                        var message = await _siteService.AddSiteAsync(site,userName);

                        TempData["SiteMessage"] = message;
                    }
                }
                catch (SqlException sqlEx)
                {
                    // Log SQL Exception with identifier (CloudWatch will receive this)
                    _logger.LogError(sqlEx, "[BST.SQLException] Error in [AddSite]: {Message}", sqlEx.Message);
                    TempData["SiteMessage"] = "Save failed";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[BST.GENERAL_EXCEPTION] Error in [AddSite]: {Message}", ex.Message);
                    TempData["SiteMessage"] = "Save failed";
                }
               

                return RedirectToAction(nameof(AddSite));
            }

            return View(siteViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrainee(int personId, string selectedSite)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                TempData["SiteMessage"] = "Invalid request";
                return RedirectToAction("SiteTrainee", new { selectedSite });
            }
            try
            {
                if (canEdit)
                {
                    var message = await _siteService.DeleteTraineeAsync(personId);
                    TempData["SiteMessage"] = message;
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL Exception with identifier (CloudWatch will receive this)
                _logger.LogError(sqlEx, "[BST.SQLException] Error in [DeleteTrainne]: {Message}", sqlEx.Message);
                TempData["SiteMessage"] = "Save failed";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BST.GENERAL_EXCEPTION] Error in [DeleteTrainee]: {Message}", ex.Message);
                TempData["SiteMessage"] = "Save failed";
            }

            return RedirectToAction("SiteTrainee", new { selectedSite });
        }
       
    }
}
