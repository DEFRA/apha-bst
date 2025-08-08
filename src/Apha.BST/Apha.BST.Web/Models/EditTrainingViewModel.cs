using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{   
    public class EditTrainingViewModel:BaseViewModel
    {
        [Required]
        public int TraineeId { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [Required]
        public required string TrainingType { get; set; }

        [Required]
        public required string TrainingAnimal { get; set; }

        [Required]
        public DateTime TrainingDateTime { get; set; }

        // Dropdown Lists
        public IEnumerable<SelectListItem> TraineeList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TrainerList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TrainingTypesList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TrainingAnimalList { get; set; } = new List<SelectListItem>();

        //// Keys for locating old record in DB
        [Required]
        public DateTime TrainingDateTimeOld { get; set; }
        [Required]
        public int TraineeIdOld { get; set; }
        [Required]
        public int TrainerIdOld { get; set; }
        [Required]
        public required string TrainingAnimalOld { get; set; }
        [Required]
        public required string TrainingTypeOld { get; set; }
    }

}

