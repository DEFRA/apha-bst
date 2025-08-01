namespace Apha.BST.Web.Models
{
    public class TrainingViewModel
    {
        public int PersonID { get; set; }
        public string? Person { get; set; }
        public string? TrainingAnimal { get; set; }
        public string? Name { get; set; }
        public DateTime TrainingDateTime { get; set; }
        public string? TrainingType { get; set; }
        public int TraineeId { get; set; }
    }
}
