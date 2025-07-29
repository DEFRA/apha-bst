using Apha.BST.Application.DTOs;
using Apha.BST.Core.Entities;
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
            CreateMap<AddTrainingViewModel, TrainingDTO>().ReverseMap();
            CreateMap<TrainerTraining, TrainerTrainingDTO>().ReverseMap();
            CreateMap<TrainerTrainingDTO, TrainingViewModel>().ReverseMap();
            // Mapping between Persons and TraineeViewModel            
            CreateMap<Apha.BST.Core.Entities.Persons, Apha.BST.Web.Models.TraineeViewModel>()
                .ForMember(dest => dest.PersonID, opt => opt.MapFrom(src => src.PersonId))
                .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person));
            CreateMap<EditTrainingDTO, EditTrainingViewModel>().ReverseMap();
            CreateMap<TrainerHistoryDTO, TrainingHistoryModel>().ReverseMap();
        }
    }
}
