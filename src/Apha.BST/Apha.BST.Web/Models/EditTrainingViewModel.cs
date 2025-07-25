using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apha.BST.Web.Models
{   
    public class EditTrainingViewModel
    {        
        [Required]
        public int PersonId { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [Required]
        public string TrainingType { get; set; }

        [Required]
        public string TrainingAnimal { get; set; }

        [Required]
        public DateTime TrainingDateTime { get; set; }

        // Dropdown Lists
        public IEnumerable<SelectListItem> Persons { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Trainers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> TrainingTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Species { get; set; } = new List<SelectListItem>();

        // Keys for locating old record in DB
        public DateTime TrainingDateTimeOld { get; set; }
        public string TrainingAnimalOld { get; set; } = string.Empty;
    }

}

