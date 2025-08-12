using System.Runtime.CompilerServices;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
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
    public class TrainingController : Controller
    {
        private readonly ITrainingService _trainingService;
        private readonly ILogger<TrainingController> _logger;
        private readonly IUserDataService _userDataService;
        private readonly IMapper _mapper;
        private readonly IStaticDropdownService _staticDropdownService;         
        private const string traineeAll = "All";

        public TrainingController(ILogger<TrainingController> logger,ITrainingService trainingService, IMapper mapper,IStaticDropdownService staticDropdownService, IUserDataService userDataService)
        {
            _trainingService = trainingService;
            _mapper = mapper;
            _staticDropdownService = staticDropdownService;
            _userDataService = userDataService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AddTraining()
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            var viewModel = new AddTrainingViewModel
            {
                CanEdit = canEdit,
                Persons = (await _trainingService.GetTraineesAsync())
                    .Select(p => new SelectListItem { Value = p.PersonId.ToString(), Text = p.Person })
                    .ToList()
            };

            viewModel.TrainingTypesList = _staticDropdownService.GetTrainingTypes()
                 .Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
                 .ToList();

            viewModel.TrainingAnimalList = _staticDropdownService.GetTrainingAnimal()
               .Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
               .ToList();
            viewModel.TrainingDateTime = DateTime.Today; // Set default date to now
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTraining(AddTrainingViewModel viewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            viewModel.CanEdit = canEdit;
            if (!ModelState.IsValid)
            {
                viewModel.Persons = (await _trainingService.GetTraineesAsync())
                    .Select(p => new SelectListItem { Value = p.PersonId.ToString(), Text = p.Person })
                    .ToList();
                viewModel.TrainingTypesList = _staticDropdownService.GetTrainingTypes()
                 .Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
                 .ToList();

                viewModel.TrainingAnimalList = _staticDropdownService.GetTrainingAnimal()
                   .Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
                   .ToList();

                return View(viewModel);
            }
            try
            {   if (canEdit)
                {
                    var dto = _mapper.Map<TrainingDto>(viewModel);
                    var message = await _trainingService.AddTrainingAsync(dto);
                    TempData["Message"] = message;
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL Exception with identifier (CloudWatch will receive this)
                _logger.LogError(sqlEx, "[BST.SQLException] Error in [AddTraining]: {Message}", sqlEx.Message);
                TempData["Message"] = "Save failed";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BST.GENERAL_EXCEPTION] Error in [AddTraining]: {Message}", ex.Message);
                TempData["Message"] = "Save failed";
            }          

            return RedirectToAction(nameof(AddTraining));
        }

        [HttpGet]
        public async Task<IActionResult> EditTraining(int traineeId, int trainerId, string species, DateTime dateTrained, string trainingType)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewTraining));
            }
            var dto = await _trainingService.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType);            
            if (dto == null)
            {
                TempData["Message"] = "Invalid data provided for deletion.";
                return RedirectToAction(nameof(ViewTraining));
            }
            var persons = await _trainingService.GetTraineesAsync();
            var viewModel = new EditTrainingViewModel
            {
                CanEdit = canEdit,
                TraineeId = dto.PersonId,
                TrainerId = dto.TrainerId,
                TrainingType = dto.TrainingType,
                TrainingAnimal = dto.TrainingAnimal,
                TrainingDateTime = dto.TrainingDateTime,
                TrainingDateTimeOld = dto.TrainingDateTime,
                TraineeIdOld = dto.PersonId,
                TrainerIdOld = dto.TrainerId,
                TrainingAnimalOld = dto.TrainingAnimal,
                TrainingTypeOld = dto.TrainingType,
            };
            viewModel.TraineeList = persons
                .Select(p => new SelectListItem { Value = p.PersonId.ToString(), Text = p.Person, Selected = p.PersonId == dto.PersonId })
                .ToList();
            viewModel.TrainerList = persons
               .Select(p => new SelectListItem { Value = p.PersonId.ToString(), Text = p.Person, Selected = p.PersonId == dto.TrainerId })
               .ToList();
            viewModel.TrainingTypesList = _staticDropdownService.GetTrainingTypes()
              .Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
              .ToList();
            viewModel.TrainingAnimalList = _staticDropdownService.GetTrainingAnimal()
               .Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
               .ToList();
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTraining(EditTrainingViewModel viewModel)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            viewModel.CanEdit = canEdit;
            if (!ModelState.IsValid)
            {
                var persons = await _trainingService.GetTraineesAsync();
                viewModel.CanEdit = canEdit;

                viewModel.TraineeList = persons
                    .Select(p => new SelectListItem { Value = p.PersonId.ToString(), Text = p.Person, Selected = p.PersonId == viewModel.TraineeIdOld })
                    .ToList();
                viewModel.TrainerList = persons
                   .Select(p => new SelectListItem { Value = p.PersonId.ToString(), Text = p.Person, Selected = p.PersonId == viewModel.TrainerIdOld })
                   .ToList();
                viewModel.TrainingTypesList = _staticDropdownService.GetTrainingTypes()
                  .Select(t => new SelectListItem { Value = t.Value, Text = t.Text, Selected = t.Value == viewModel.TrainingTypeOld })
                  .ToList();
                viewModel.TrainingAnimalList = _staticDropdownService.GetTrainingAnimal()
                   .Select(t => new SelectListItem { Value = t.Value, Text = t.Text, Selected = t.Value == viewModel.TrainingAnimalOld })
                   .ToList();
                viewModel.TrainingDateTime = viewModel.TrainingDateTimeOld;
                return View(viewModel);
            }
            
            var editTraining = _mapper.Map<EditTrainingDto>(viewModel);

            try 
            {
                if (canEdit)
                {
                    var message = await _trainingService.UpdateTrainingAsync(editTraining);
                    TempData["Message"] = message;
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL Exception with identifier (CloudWatch will receive this)
                _logger.LogError(sqlEx, "[BST.SQLException] Error in [UpdateTraining]: {Message}", sqlEx.Message);
                TempData["Message"] = "Error updating training";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BST.GENERAL_EXCEPTION] Error in [UpdateTraining]: {Message}", ex.Message);
                TempData["Message"] = "Error updating training";
            }           
            
            return RedirectToAction("ViewTraining", new { selectedTraineeId = viewModel.TraineeIdOld });            
        }

        [HttpGet]
        public async Task<IActionResult> ViewTraining(string selectedTraineeId = "")
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            var allPersons = await _trainingService.GetTraineesAsync();
            var allTrainee = _mapper.Map<IEnumerable<TraineeTrainerViewModel>>(allPersons);

            var viewSelectList = allTrainee
               .Select(p => new SelectListItem
               {
                   Text = p.Person,
                   Value = p.PersonID.ToString(),

               })
               .ToList();

            var model = new TrainingListViewModel
            {
                AllTrainees = viewSelectList,
                CanEdit = canEdit,

            };
            IEnumerable<TrainingViewModel> filteredTrainings;
            if (!string.IsNullOrEmpty(selectedTraineeId) && selectedTraineeId != "0")
            {
                if (selectedTraineeId == traineeAll)
                {
                    var allTrainings = await _trainingService.GetAllTrainingsAsync(); // <-- new method to get all
                    filteredTrainings = _mapper.Map<IEnumerable<TrainingViewModel>>(allTrainings);
                }
                else
                {
                    var trainingDtos = await _trainingService.GetTrainingByTraineeAsync(selectedTraineeId);
                    filteredTrainings = _mapper.Map<IEnumerable<TrainingViewModel>>(trainingDtos);
                }

                model.FilteredTrainings = filteredTrainings;
                model.SelectedTraineeId = selectedTraineeId;
            }
            return View(model);
        }

        
        // For TrainerHistory
        [HttpGet]
        public async Task<IActionResult> TrainerHistory(int selectedTrainerId = 0, string selectedSpecies = "Cattle")
        {            
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(TrainerHistory)); // Redirect if ModelState is invalid
            }
            var allPersons = await _trainingService.GetTraineesAsync();
            var allTrainers = _mapper.Map<List<TraineeTrainerViewModel>>(allPersons);

            var historyDto = await _trainingService.GetTrainerHistoryAsync(selectedTrainerId, selectedSpecies);

            // FIX: Map DTO to Model
            var historyData = _mapper.Map<List<TrainingHistoryModel>>(historyDto);


            var model = new TrainerHistoryViewModel
            {
                CanEdit = canEdit,
                SelectedTrainerId = selectedTrainerId,
                SelectedSpecies = selectedSpecies,
                AllTrainers = allTrainers,
                HistoryDetails = historyData,
                TrainingAnimalList = _staticDropdownService.GetTrainingAnimal()
            };
            var trainerSelectList = allTrainers
                .Select(trainer => new SelectListItem
                {
                    Value = trainer.PersonID.ToString(),
                    Text = trainer.Person,
                    Selected = trainer.PersonID == selectedTrainerId
                })
                .ToList();

            // Insert the "All people" option at the top of the list
            trainerSelectList.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "All people",               
            });

            model.AllTrainersList = trainerSelectList;           
            return View(model);
        }

        // For TrainerTrained
        [HttpGet]
        public async Task<IActionResult> TrainerTrained(int selectedTrainerId = 0)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(TrainerTrained)); // Redirect if ModelState is invalid
            }
            var allPersons = await _trainingService.GetTraineesAsync();
            var allTrainers = _mapper.Map<List<TrainerTrainedModel>>(allPersons);

            var trainedList = selectedTrainerId != 0
                ? await _trainingService.GetTrainerTrainedAsync(selectedTrainerId)
                : new List<TrainerTrainedDto>();


            var model = new TrainerTrainedViewModel
            {
                CanEdit = canEdit,
                SelectedTrainerId = selectedTrainerId,
                AllTrainers = allTrainers,
                TraineeTrainingDetails = trainedList.ToList()
            };
            var trainerList = allTrainers
                .Select(t => new SelectListItem
                {
                    Value = t.PersonID.ToString(),
                    Text = t.Person,
                    Selected = t.PersonID == selectedTrainerId
                }).ToList();

            trainerList.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "Select trainer",
                Selected = selectedTrainerId == 0
            });

            model.TrainerSelectList = trainerList;          

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTraining(int traineeId, string species, DateTime dateTrained)
        {
            bool canEdit = await _userDataService.CanEditPage(ControllerContext.ActionDescriptor.ActionName);
            if (!ModelState.IsValid) 
            {
                TempData["Message"] = "Invalid data provided for deletion.";
                return RedirectToAction(nameof(ViewTraining), new { selectedTraineeId = traineeId });
            }
            try
            {
                if (canEdit)
                {
                    var message = await _trainingService.DeleteTrainingAsync(traineeId, species, dateTrained);
                    TempData["Message"] = message;
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL Exception with identifier (CloudWatch will receive this)
                _logger.LogError(sqlEx, "[BST.SQLException] Error in [DeleteTraining]: {Message}", sqlEx.Message);
                TempData["Message"] = "Delete failed";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BST.GENERAL_EXCEPTION] Error in [DeleteTraining]: {Message}", ex.Message);
                TempData["Message"] = "Delete failed";
            }
           
            return RedirectToAction(nameof(ViewTraining), new { selectedTraineeId = traineeId });
        }      

    }
}
