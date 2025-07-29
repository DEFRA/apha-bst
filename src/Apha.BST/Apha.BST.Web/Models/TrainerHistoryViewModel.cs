using Apha.BST.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    //    public class TrainerHistoryViewModel
    //    {
    //        public int SelectedTrainerId { get; set; }
    //        public string SelectedSpecies { get; set; } = "Cattle";
    //        public IEnumerable<SelectListItem> Trainers { get; set; } = new List<SelectListItem>();
    //        public IEnumerable<SelectListItem> SpeciesList { get; set; } = new List<SelectListItem>
    //{
    //    new SelectListItem { Value = "Cattle", Text = "Cattle" },
    //    new SelectListItem { Value = "Sheep", Text = "Sheep" },
    //    new SelectListItem { Value = "Goat", Text = "Goat" }
    //};
    //        public IEnumerable<TrainerHistoryDTO> TrainerDetails { get; set; } = new List<TrainerHistoryDTO>();
    //    }
    public class TrainerHistoryViewModel
    {
        public int SelectedTrainerId { get; set; }
        public string SelectedSpecies { get; set; }
        public List<TraineeViewModel> AllTrainers { get; set; }
        public List<TrainingHistoryModel> HistoryDetails { get; set; }
        // public List<SelectListItem> SpeciesSelectList { get; set; }

        //    // Static method or property to get species list
        //    //public static List<SelectListItem> GetDefaultSpeciesList(string selectedSpecies)
        //    //{
        //    //    return new List<SelectListItem>
        //    //{
        //    //    new SelectListItem { Text = "Cattle", Value = "Cattle", Selected = selectedSpecies == "Cattle" },
        //    //    new SelectListItem { Text = "Sheep", Value = "Sheep", Selected = selectedSpecies == "Sheep" },
        //    //    new SelectListItem { Text = "Goat", Value = "Goat", Selected = selectedSpecies == "Goat" },
        //    //};
        //    //}
        //    public static List<SelectListItem> SpeciesList => new List<SelectListItem>
        //{
        //    new SelectListItem { Text = "Cattle", Value = "Cattle" },
        //    new SelectListItem { Text = "Sheep", Value = "Sheep" },
        //    new SelectListItem { Text = "Goat", Value = "Goat" }
        //};
        //}
        public static List<SelectListItem> SpeciesList => new()
    {
        new SelectListItem { Text = "Cattle", Value = "Cattle" },
        new SelectListItem { Text = "Sheep", Value = "Sheep" },
        new SelectListItem { Text = "Goat", Value = "Goat" },
    };

        public List<SelectListItem> SpeciesSelectList => SpeciesList
            .Select(item => new SelectListItem
            {
                Text = item.Text,
                Value = item.Value,
                Selected = item.Value == SelectedSpecies
            }).ToList();
    }
}
