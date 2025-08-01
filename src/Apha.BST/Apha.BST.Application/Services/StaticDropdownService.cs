﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Application.Services
{
    public class StaticDropdownService : IStaticDropdownService
    {
        public List<SelectListItem> GetTrainingTypes() => new List<SelectListItem>
       {
            new SelectListItem { Value = "Cascade trained", Text = "Cascade trained" },
            new SelectListItem { Value = "Trained", Text = "Trained" },
            new SelectListItem { Value = "Training confirmed", Text = "Training confirmed" }
       };

        public List<SelectListItem> GetTrainingAnimal() => new List<SelectListItem>
        {
            new SelectListItem { Value = "Cattle", Text = "Cattle" },
            new SelectListItem { Value = "Sheep", Text = "Sheep" },
            new SelectListItem { Value = "Goat", Text = "Goat" }
        };

    }
}
