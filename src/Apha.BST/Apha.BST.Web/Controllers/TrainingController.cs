using System.Runtime.CompilerServices;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Controllers
{
    public class TrainingController : Controller
    {
        private readonly ITrainingService _trainingService;
        private readonly IPersonsService _personService;
        private readonly IMapper _mapper;
        private readonly IStaticDropdownService _staticDropdownService;         
        private const string traineeAll = "All";

        public TrainingController(ITrainingService trainingService, IMapper mapper, IPersonsService personService,IStaticDropdownService staticDropdownService)
        {
            _trainingService = trainingService;
            _mapper = mapper;
            _personService = personService;
            _staticDropdownService = staticDropdownService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AddTraining()
        {
            var viewModel = new AddTrainingViewModel
            {
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
            {               
                var dto = _mapper.Map<TrainingDto>(viewModel);
                var message = await _trainingService.AddTrainingAsync(dto);
                TempData["Message"] = message;
            }
            catch (Exception)
            {
                TempData["Message"] = "Save failed";
            }

            return RedirectToAction(nameof(AddTraining));
        }

        [HttpGet]
        public async Task<IActionResult> EditTraining(int traineeId, int trainerId, string species, DateTime dateTrained, string trainingType)
        {

            var dto = await _trainingService.GetTrainingByKeysAsync(traineeId, trainerId, species, dateTrained, trainingType);
            if (dto == null)
            {
                TempData["Message"] = "Invalid data provided for deletion.";
                return RedirectToAction(nameof(ViewTraining));
            }
            var persons = await _trainingService.GetTraineesAsync();
            var viewModel = new EditTrainingViewModel
            {
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
            if (!ModelState.IsValid)
            {
                var persons = await _trainingService.GetTraineesAsync();

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
            var message = await _trainingService.UpdateTrainingAsync(editTraining);
            TempData["Message"] = message;
            return RedirectToAction("ViewTraining", new { selectedTraineeId = viewModel.TraineeIdOld });            
        }

        [HttpGet]
        public async Task<IActionResult> ViewTraining(string selectedTraineeId = "")
        {
            var allPersons = await _personService.GetAllPersonAsync();
            var allTrainee = _mapper.Map<IEnumerable<TraineeViewModel>>(allPersons);

            var viewSelectList = allTrainee
               .Select(p => new SelectListItem
               {
                   Text = p.Person,
                   Value = p.PersonID.ToString(),

               })
               .ToList();

            var model = new TrainingListViewModel
            {
                AllTrainees = viewSelectList

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
            var allPersons = await _personService.GetAllPersonAsync();
            var allTrainers = _mapper.Map<List<TraineeViewModel>>(allPersons);

            var historyDto = await _trainingService.GetTrainerHistoryAsync(selectedTrainerId, selectedSpecies);

            // FIX: Map DTO to Model
            var historyData = _mapper.Map<List<TrainingHistoryModel>>(historyDto);


            var model = new TrainerHistoryViewModel
            {
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
            var allPersons = await _personService.GetAllPersonAsync();
            var allTrainers = _mapper.Map<List<TrainerTrainedModel>>(allPersons);

            var trainedList = selectedTrainerId != 0
                ? await _trainingService.GetTrainerTrainedAsync(selectedTrainerId)
                : new List<TrainerTrainedDto>();


            var model = new TrainerTrainedViewModel
            {
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
            if(!ModelState.IsValid) 
            {
                TempData["Message"] = "Invalid data provided for deletion.";
                return RedirectToAction(nameof(ViewTraining), new { selectedTraineeId = traineeId });
            }
            var message = await _trainingService.DeleteTrainingAsync(traineeId, species, dateTrained);
            TempData["Message"] = message;
            return RedirectToAction(nameof(ViewTraining), new { selectedTraineeId = traineeId });
        }


    }
}
