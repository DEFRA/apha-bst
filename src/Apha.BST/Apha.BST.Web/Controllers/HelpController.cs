using Apha.BST.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apha.BST.Web.Controllers
{
    [Authorize]
    public class HelpController : Controller
    {
        
        public IActionResult Help(int index = 0)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Help)); // Redirect if ModelState is invalid
            }
            // Ensure index is not negative
            index = Math.Max(0, index);
            var sections = new List<HelpListViewModel>
            {
            new() { Title = "User help", ImageUrl = "/Images/BSTUserAdd.PNG", Description = "* Only superusers can add, delete or alter a user" },
            new() { Title = "Site help", ImageUrl = "/Images/BSTSite.PNG", Description = "Superusers and data entry people can add, edit and delete sites" },
            new() { Title = "Trainee help", ImageUrl = "/Images/BSTTrainee.PNG",Description = "Superusers and data entry people can add, edit and delete training",AdditionalInfo = "Note that a trainee can only be deleted if that person has no training record (where ticked)" },
            new() { Title = "Trainer help", ImageUrl = "/Images/BSTTrainer.PNG",Description = "Superusers and data entry people can add, edit and delete trainers" },
            new() { Title = "Training help", ImageUrl = "/Images/BSTTraining.PNG",Description = "Superusers and data entry people can add, edit and delete training" },
            new() { Title = "Training location help",  ImageUrl = "/Images/BSTTrainingLoc.PNG",Description = "Superusers and data entry people can add and delete training locations" },
            new() { Title = "News help",ImageUrl = "/Images/News.PNG",Description = "Superusers only can add and delete news" },
            new() { Title = "Home help",ImageUrl = "/Images/BSThome.PNG" }
             };

            return View(new HelpViewModel { ActiveViewIndex = index, Sections = sections });
        }
    }
}
