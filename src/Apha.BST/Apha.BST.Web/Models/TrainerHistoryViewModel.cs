﻿using Apha.BST.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    public class TrainerHistoryViewModel
    {
        public int SelectedTrainerId { get; set; }
        public string? SelectedSpecies { get; set; }
        public List<TraineeViewModel>? AllTrainers { get; set; }
        public List<SelectListItem> AllTrainersList { get; set; } = new List<SelectListItem>();
        public List<TrainingHistoryModel>? HistoryDetails { get; set; }
        public List<SelectListItem> TrainingAnimalList { get; set; } = new();       
    }
}
