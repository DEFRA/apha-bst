using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace Apha.BST.Web.Controllers
{
    [Authorize]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personService;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private const string siteAll = "All";
        private const string personMessage = "PersonMessage";

        public PersonsController(IPersonsService personService, IMapper mapper, IUserDataService userDataService, ILogService logService)
        {
            _personService = personService;
            _mapper = mapper;
            _userDataService = userDataService;
            _logService = logService;
        }
        [HttpGet]
        public async Task<IActionResult> ViewPerson(int selectedPerson = 0)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewPerson)); // Redirect if ModelState is invalid
            }
            // Get dropdown list
            var dropdownDto = await _personService.GetPersonsForDropdownAsync();
            var personSelectedList = dropdownDto
               .Select(persons => new SelectListItem
                {
                   Value = persons.PersonID.ToString(),
                   Text = persons.Person,
                })
                .ToList();
            // Get full list for grid
            var allPersonDto = await _personService.GetAllPersonByNameAsync(selectedPerson);
            var allPerson = _mapper.Map<IEnumerable<PersonViewModel>>(allPersonDto);


            var model = new PersonListViewModel
            {
                AllPerson = personSelectedList,
                FilteredPerson = allPerson,
                SelectedPerson = selectedPerson,
                CanEdit = canEdit
            };

            return View(model);
        }
        

        [HttpPost]
        public async Task<IActionResult> DeletePerson(int personId,string personName,int selectedPerson)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            string failmessage = "Delete failed: ";
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewPerson)); // Redirect if ModelState is invalid
            }
            var personSelectedList = new List<SelectListItem>();
            try
            {
                if (canEdit)
                {
                    
                    var message = await _personService.DeletePersonAsync(personId);
                    TempData[personMessage] = message;
                    var dropdownDto = await _personService.GetPersonsForDropdownAsync();
                    personSelectedList = dropdownDto
                       .Select(persons => new SelectListItem
                       {
                           Value = persons.PersonID.ToString(),
                           Text = persons.Person,
                       })
                        .ToList();
                    if(dropdownDto.Count(x => x.PersonID == personId) == 0)
                    {
                        personSelectedList.Add(new SelectListItem
                        {
                            Value = personId.ToString(),
                            Text = personName
                        });
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[personMessage] = failmessage+sqlEx.Message.ToString();
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[personMessage] = failmessage+ex.Message.ToString();
            }
          
            if(selectedPerson==0)
            {
                return RedirectToAction("ViewPerson", new { selectedPerson });
            }
            
            var allPersonDto = await _personService.GetAllPersonByNameAsync(selectedPerson);
            var allPerson = _mapper.Map<IEnumerable<PersonViewModel>>(allPersonDto);
            var model = new PersonListViewModel
            {
                AllPerson = personSelectedList,
                FilteredPerson = allPerson,
                SelectedPerson = selectedPerson,
                CanEdit = canEdit
            };
            return View("ViewPerson", model);
           
        }

        [HttpGet]
        public async Task<IActionResult> AddPerson()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            var viewModel = new AddPersonViewModel
            {
                Sites = (await _personService.GetAllSitesAsync(siteAll))
                    .Select(p => new SelectListItem { Value = p.PlantNo.ToString(), Text = p.Name })
                    .ToList()
            };
            viewModel.CanEdit = canEdit;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddPerson(AddPersonViewModel viewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);

            string userName = _userDataService.GetUsername() ?? "";
            if (!ModelState.IsValid)
            {
                viewModel.Sites = (await _personService.GetAllSitesAsync(siteAll))
                    .Select(p => new SelectListItem { Value = p.PlantNo.ToString(), Text = p.Name })
                    .ToList();
                viewModel.CanEdit = canEdit;
                return View(viewModel);
            }

            try
            {
                if (canEdit)
                { 
                    var personData = _mapper.Map<AddPersonDto>(viewModel);
                    var message = await _personService.AddPersonAsync(personData, userName);
                    TempData[personMessage] = message;
                }
            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);               
                TempData[personMessage] = "Save failed: "+sqlEx.Message.ToString();
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);               
                TempData[personMessage] = "Save failed: "+ex.Message.ToString();
            }
           
            return RedirectToAction(nameof(AddPerson));
        }
        [HttpGet]
        public async Task<IActionResult> EditPerson(int id)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewPerson)); // Redirect if ModelState is invalid
            }
            var person = await _personService.GetPersonNameByIdAsync(id);
            var siteName = await _personService.GetSiteByIdAsync(id);
            if (person == null)
            {
                return RedirectToAction(nameof(ViewPerson));
            }
            var viewModel = new EditPersonViewModel
            {
                CanEdit = canEdit,
                PersonId = id,
                Person = person,
                Name = siteName,
                SiteOptions = (await _personService.GetAllSitesAsync(siteAll))
                    .Select(p => new SelectListItem { Value = p.PlantNo.ToString(), Text = p.Name })
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPerson(EditPersonViewModel viewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);

            if (!ModelState.IsValid)
            {
                var sites = await _personService.GetAllSitesAsync(siteAll);

                viewModel.SiteOptions = sites.
                 Select(p => new SelectListItem { Value = p.PlantNo.ToString(), Text = p.Name })
                .ToList();

                viewModel.CanEdit = canEdit;
                return View(viewModel);
            }
            try
            {
                if(canEdit)
                {
                    var editTraining = _mapper.Map<EditPersonDto>(viewModel);
                    var message = await _personService.UpdatePersonAsync(editTraining);

                    TempData[personMessage] = message;
                }            

            }
            catch (SqlException sqlEx)
            {
                _logService.LogSqlException(sqlEx, ControllerContext.ActionDescriptor.ActionName);
                TempData[personMessage] = "Update failed: "+sqlEx.Message.ToString();
            }
            catch (Exception ex)
            {
                _logService.LogGeneralException(ex, ControllerContext.ActionDescriptor.ActionName);
                TempData[personMessage] = "Update failed: "+ex.Message.ToString();
            }

            return RedirectToAction(nameof(EditPerson));


        }
    }
}
