namespace Apha.BST.Web.Models
{
    public class TrainingListViewModel
    {
        public IEnumerable<TraineeViewModel> AllTrainees { get; set; }
        public IEnumerable<TrainingViewModel> FilteredTrainings { get; set; }
        public string SelectedTraineeId { get; set; }
    }
}
