using Apha.BST.Application.DTOs;

namespace Apha.BST.Web.Models
{
    public class ViewNewsViewModel : BaseViewModel
    {
        public List<NewsDto> NewsList { get; set; } = new List<NewsDto>();
        public string? Message { get; set; }
    }
}
