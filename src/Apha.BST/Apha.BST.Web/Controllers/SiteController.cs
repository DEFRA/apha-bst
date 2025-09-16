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
        private readonly ILogService _logService;

        private readonly IMapper _mapper;
        private const string siteAll = "All";
        private const string status = "Save failed: ";
        private const string siteMessage = "SiteMessage";
        public SiteController(ISiteService siteService, IMapper mapper, IUserDataService userDataService, ILogService logService)
        {
            _siteService = siteService;
            _mapper = mapper;
            _userDataService = userDataService;
            _logService = logService;

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
            siteViewModel.CanEdit = canEdit;
            if (ModelState.IsValid)
            {
                try
                {
                    string userName = _userDataService.GetUsername() ?? "";
                    if (canEdit)
                    {
                        var site = _mapper.Map<SiteDto>(siteViewModel);
                        var message = await _siteService.AddSiteAsync(site, userName);

                        TempData[siteMessage] = message;
                    }
                }
                catch (SqlException sqlEx)
                {
                    _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                    TempData[siteMessage] = status +sqlEx.Message.ToString();
                }
                catch (Exception ex)
                {
                    _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                    TempData[siteMessage] = status + ex.Message.ToString();
                }


                return RedirectToAction(nameof(AddSite));
            }

            return View(siteViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrainee(int personId, string selectedSite)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                TempData[siteMessage] = "Invalid request";
                return RedirectToAction("SiteTrainee", new { selectedSite });
            }
            try
            {
                if (canEdit)
                {
                    var message = await _siteService.DeleteTraineeAsync(personId);
                    TempData[siteMessage] = message;
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[siteMessage] = status + sqlEx.Message.ToString(); 
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[siteMessage] = status + ex.Message.ToString(); 
            }

            return RedirectToAction("SiteTrainee", new { selectedSite });
        }
        [HttpGet]
        public async Task<IActionResult> EditSite(string plantNo)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (string.IsNullOrEmpty(plantNo))
            {
                return RedirectToAction(nameof(ViewSite));
            }

            var siteDtos = await _siteService.GetAllSitesAsync(plantNo);
            var siteDto = siteDtos.FirstOrDefault();

            if (siteDto == null)
            {
                TempData[siteMessage] = "Invalid data provided for editing.";
                return RedirectToAction(nameof(ViewSite));
            }

            var viewModel = new EditSiteViewModel
            {
                CanEdit = canEdit,
                PlantNo = siteDto.PlantNo,
                Name = siteDto.Name,
                AddressLine1 = siteDto.AddressLine1,
                AddressLine2 = siteDto.AddressLine2,
                AddressTown = siteDto.AddressTown,
                AddressCounty = siteDto.AddressCounty,
                AddressPostCode = siteDto.AddressPostCode,
                Telephone = siteDto.Telephone,
                Fax = siteDto.Fax,
                IsAhvla = siteDto.Ahvla?.Trim().Equals("AHVLA") ?? false,

            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSite(EditSiteViewModel editSiteViewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            editSiteViewModel.CanEdit = canEdit;
            if (!ModelState.IsValid)
            {
                editSiteViewModel.CanEdit = canEdit;
                return View(editSiteViewModel);
            }

            try
            {
                var siteInputDto = _mapper.Map<SiteInputDto>(editSiteViewModel);
                var message = await _siteService.UpdateSiteAsync(siteInputDto);
                TempData[siteMessage] = message;

            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[siteMessage] = status + sqlEx.Message.ToString(); 
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[siteMessage] = status + ex.Message.ToString(); 
            }

            return View(editSiteViewModel);
        }
    }
}
