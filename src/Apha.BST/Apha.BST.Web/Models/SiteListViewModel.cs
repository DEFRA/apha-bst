using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    public class SiteListViewModel:BaseViewModel
    {        
        public IEnumerable<SelectListItem> AllSites { get; set; } = new List<SelectListItem>();
        public IEnumerable<SiteViewModel>? FilteredSites { get; set; }
        public string SelectedSite { get; set; } = "All";        
    }
}
