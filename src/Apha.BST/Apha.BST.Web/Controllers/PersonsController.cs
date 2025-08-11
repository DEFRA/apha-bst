using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;

namespace Apha.BST.Web.Controllers
{
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personService;
        private readonly IMapper _mapper;
        private const string personName = "All";
        private readonly ILogger<PersonsController> _logger;
        private const string siteAll = "All";

        public PersonsController(IPersonsService personService, IMapper mapper, ILogger<PersonsController> logger)
        {
            _personService = personService;
            _mapper = mapper;
            _logger=logger;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var person = await _personService.GetAllPersonAsync();
            _mapper.Map<IEnumerable<PersonsDto>>(person);
            return View(person);

        }


        [HttpGet]
        public async Task<IActionResult> ViewPerson(int selectedPerson = 0)
        {

            // Get dropdown list
            var dropdownDto = await _personService.GetPersonsForDropdownAsync();
            var dropdownList = _mapper.Map<IEnumerable<PersonViewModel>>(dropdownDto);


            var personSelectedList = dropdownList
               .Select(persons => new SelectListItem
               {
                   Value = persons.PersonId.ToString(),
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
                SelectedPerson = selectedPerson
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> DeletePerson(int personId, string selectedPerson)
        {

            try
            {
                var message = await _personService.DeletePersonAsync(personId);
                TempData["message"] = message;
            }
            catch (Exception e)
            {
                TempData["message"] = "Save failed";
                _logger.LogError(e, "Error deleting person with ID {PersonId}", personId);
            }

            return RedirectToAction("ViewPerson", new { selectedPerson });
        }




        [HttpGet]
        public async Task<IActionResult> AddPerson()
        {
            var viewModel = new AddPersonViewModel
            {
                Sites = (await _personService.GetAllSitesAsync(siteAll))
                    .Select(p => new SelectListItem { Value = p.PlantNo.ToString(), Text = p.Name })
                    .ToList()
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson(AddPersonViewModel viewModel, string? userName =null)
        {

            userName="BSTUser";
            if (!ModelState.IsValid)
            {
                viewModel.Sites = (await _personService.GetAllSitesAsync(siteAll))
                    .Select(p => new SelectListItem { Value = p.PlantNo.ToString(), Text = p.Name })
                    .ToList();
                return View(viewModel);
            }

            try
            {
               
               
                var personData = _mapper.Map<AddPersonDto>(viewModel);
                var message = await _personService.AddPersonAsync(personData, userName);

               
                TempData["Message"]=message;
            }
            catch (Exception e)
            {
                TempData["Message"]="Save failed";
                _logger.LogError(e, "Error adding person with name {PersonName}", viewModel.Name);
            }

            return RedirectToAction(nameof(AddPerson));
        }
        [HttpGet]
        public async Task<IActionResult> EditPerson(int id)
        {
            var person = await _personService.GetPersonNameByIdAsync(id);
            var siteName = await _personService.GetSiteByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            var viewModel = new EditPersonViewModel
            {
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

            if (!ModelState.IsValid)
            {
                var sites = await _personService.GetAllSitesAsync(siteAll);

                viewModel.SiteOptions = sites.
                 Select(p => new SelectListItem { Value = p.PlantNo.ToString(), Text = p.Name })
                .ToList();


                return View(viewModel);
            }
            try
            {
                var editTraining = _mapper.Map<EditPersonDto>(viewModel);
                var message = await _personService.UpdatePersonAsync(editTraining);
               
                TempData["Message"]=message;


            }
            catch (Exception ex)
            {
               
                TempData["Message"]="Update failed";
                _logger.LogError(ex, "Error updating person with ID {PersonId}", viewModel.PersonId);

            }
          
            return RedirectToAction(nameof(EditPerson));


        }

    }


}
