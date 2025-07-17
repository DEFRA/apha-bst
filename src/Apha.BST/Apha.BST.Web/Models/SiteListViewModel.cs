namespace Apha.BST.Web.Models
{
    public class SiteListViewModel
    {
        public IEnumerable<SiteViewModel> AllSites { get; set; }
        public IEnumerable<SiteViewModel> FilteredSites { get; set; }
        public string SelectedSite { get; set; }
    }
}
