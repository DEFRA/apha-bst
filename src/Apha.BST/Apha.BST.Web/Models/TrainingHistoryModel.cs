namespace Apha.BST.Web.Models
{
    public class TrainingHistoryModel
    {
        public int PersonID { get; set; }
        public string Person { get; set; }
        public string Role { get; set; }
        public int TrainerID { get; set; }
        public string TrainingAnimal { get; set; }
        public DateTime TrainingDateTime { get; set; }
        public string Trainer { get; set; }

    }
}
