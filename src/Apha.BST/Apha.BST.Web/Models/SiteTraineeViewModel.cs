using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Web.Models
{
    public class SiteTraineeViewModel
    {       
        public int PersonId { get; set; }
        public string? Person { get; set; }
        public bool Cattle { get; set; }
        public bool Sheep { get; set; }
        public bool Goats { get; set; }
    }
}
