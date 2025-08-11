using Apha.BST.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    public class PersonListViewModel:BaseViewModel
    {
        public IEnumerable<SelectListItem> AllPerson { get; set; } = new List<SelectListItem>();
      
        public IEnumerable<PersonViewModel> FilteredPerson { get; set; } = new List<PersonViewModel>(); // For filtered list
        public int SelectedPerson { get; set; } = 0;
    }
}
