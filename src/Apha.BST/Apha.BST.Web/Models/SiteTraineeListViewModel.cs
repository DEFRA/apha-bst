using Apha.BST.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    public class SiteTraineeListViewModel
    {
        public IEnumerable<SiteViewModel> AllSites { get; set; }
        public IEnumerable<SiteTraineeViewModel> FilteredTrainees { get; set; }
        public string SelectedSite { get; set; }

        public string? Message { get; set; }
    }
}
