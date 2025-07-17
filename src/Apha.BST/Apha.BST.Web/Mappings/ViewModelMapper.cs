using Apha.BST.Application.DTOs;
using Apha.BST.Web.Models;
using AutoMapper;

namespace Apha.BST.Web.Mappings
{
    public class ViewModelMapper : Profile
    {
        public ViewModelMapper() 
        {
            CreateMap<SiteViewModel, SiteDTO>().ReverseMap();
            CreateMap<SiteTraineeViewModel, SiteTraineeDTO>().ReverseMap();
        }
    }
}
