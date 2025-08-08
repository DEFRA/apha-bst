using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    public class TrainingListViewModel : BaseViewModel
    {
        public List<SelectListItem>? AllTrainees { get; set; } = new List<SelectListItem>();
        public IEnumerable<TrainingViewModel>? FilteredTrainings { get; set; }
        public string? SelectedTraineeId { get; set; }

    }
}
