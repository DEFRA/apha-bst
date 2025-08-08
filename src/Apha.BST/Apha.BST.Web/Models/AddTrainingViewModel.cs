using Apha.BST.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{
    public class AddTrainingViewModel:BaseViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Trainee is required")]
        public int PersonId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Trainer is required")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "TrainingType is required")]
        public string TrainingType { get; set; } = null!;

        [Required(ErrorMessage = "TrainingAnimal is required")]
        public string TrainingAnimal { get; set; } = null!;

        [Required(ErrorMessage = "TrainingDateTime is required")]
        public DateTime TrainingDateTime { get; set; }

        public IEnumerable<SelectListItem> Persons { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TrainingTypesList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TrainingAnimalList { get; set; } = new List<SelectListItem>();
    }
    

}
