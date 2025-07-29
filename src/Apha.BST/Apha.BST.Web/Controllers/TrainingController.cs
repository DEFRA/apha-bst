using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
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
        private readonly IPersonsRepository _personRepository;
        private readonly IMapper _mapper;
        private const string siteName = "All";
        private readonly ILogger<TrainingController> _logger;

        public TrainingController(ITrainingService trainingService, IMapper mapper, IPersonsRepository personRepository, ILogger<TrainingController> logger)
        {
            _trainingService = trainingService;
            _mapper = mapper;
            _personRepository = personRepository;
            _logger = logger;
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
                return View(viewModel);
            }
            try
            {
                viewModel.TrainingDateTime = DateTime.Now;
                var dto = _mapper.Map<TrainingDTO>(viewModel);
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
        public async Task<IActionResult> EditTraining(int trainerId, string species, DateTime dateTrained)
        {
            var dto = await _trainingService.GetTrainingByKeysAsync(trainerId, species, dateTrained);

            if (dto == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<EditTrainingViewModel>(dto);
            viewModel.Persons = (await _trainingService.GetTraineesAsync())
                .Select(p => new SelectListItem { Value = p.PersonId.ToString(), Text = p.Person })
                .ToList();

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTraining(EditTrainingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Persons = (await _trainingService.GetTraineesAsync())
                    .Select(p => new SelectListItem { Value = p.PersonId.ToString(), Text = p.Person })
                    .ToList();
                return View(viewModel);
            }

            var dto = _mapper.Map<EditTrainingDTO>(viewModel);
            var message = await _trainingService.UpdateTrainingAsync(dto);
            TempData["Message"] = message;
            return RedirectToAction("ViewTrainer");
        }

        [HttpGet]
        public async Task<IActionResult> ViewTrainer(string selectedTraineeId = siteName)
        {
            var allPersons = await _personRepository.GetAllAsync(); // PersonID + Person
            var allTrainees = _mapper.Map<IEnumerable<TraineeViewModel>>(allPersons);

            IEnumerable<TrainingViewModel> filteredTrainings;

            if (selectedTraineeId == "All")
            {
                var allTrainings = await _trainingService.GetAllTrainingsAsync(); // <-- new method to get all
                filteredTrainings = _mapper.Map<IEnumerable<TrainingViewModel>>(allTrainings);
            }
            else
            {
                var trainingDtos = await _trainingService.GetTrainingByTraineeAsync(selectedTraineeId);
                filteredTrainings = _mapper.Map<IEnumerable<TrainingViewModel>>(trainingDtos);
            }

            var model = new TrainingListViewModel
            {
                AllTrainees = allTrainees,
                FilteredTrainings = filteredTrainings,
                SelectedTraineeId = selectedTraineeId
            };

            return View(model);
        }

        
        // For TrainerHistory
        [HttpGet]
        public async Task<IActionResult> TrainerHistory(int selectedTrainerId = 0, string selectedSpecies = "Cattle")
        {
            var allPersons = await _personRepository.GetAllAsync();
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
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTraining(int traineeId, string species, DateTime dateTrained)
        {
            var message = await _trainingService.DeleteTrainingAsync(traineeId, species, dateTrained);
            TempData["Message"] = message;
            return RedirectToAction(nameof(ViewTrainer), new { selectedTraineeId = traineeId });
        }


    }
}
