namespace Apha.BST.Web.Models
{
    public class TrainerTrainedModel
    {
        public int TraineeNo { get; set; }
        public string? Trainee { get; set; }
        public string? Site { get; set; }
        public string? SpeciesTrained { get; set; }
        public DateTime DateTrained { get; set; }
        public int PersonID { get; set; }
        public string Person { get; set; } = string.Empty;
    }
}
