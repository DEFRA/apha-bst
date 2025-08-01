using Apha.BST.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    public class TrainerTrainedViewModel
    {
        public int SelectedTrainerId { get; set; }

        public List<TrainerTrainedModel>? AllTrainers { get; set; }
        public List<SelectListItem> TrainerSelectList { get; set; } = new();
        public List<TrainerTrainedDto>? TraineeTrainingDetails { get; set; }       
    }
}
