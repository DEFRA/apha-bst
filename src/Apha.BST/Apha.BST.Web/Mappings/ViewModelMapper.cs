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
            // Site-related mappings
            CreateMap<SiteDto, SiteViewModel>();
            CreateMap<SiteViewModel, SiteDto>();

            // EditSite-specific mappings
            CreateMap<SiteDto, EditSiteViewModel>()
                .ForMember(dest => dest.IsAhvla, opt => opt.MapFrom(src => src.Ahvla == "AHVLA"));
            CreateMap<EditSiteViewModel, SiteDto>()
                .ForMember(dest => dest.Ahvla, opt => opt.MapFrom(src => src.IsAhvla ? "AHVLA" : "Non-AHVLA"));

            
            CreateMap<SiteTraineeViewModel, SiteTraineeDto>().ReverseMap();

            // Training-related mappings
            CreateMap<AddTrainingViewModel, TrainingDto>().ReverseMap();
            CreateMap<TrainerTraining, TrainerTrainingDto>().ReverseMap();
            CreateMap<TrainerTrainingDto, TrainingViewModel>().ReverseMap();            
            CreateMap<PersonsDto, TraineeTrainerViewModel>().ReverseMap();
            CreateMap<PersonsDto, TrainerTrainedModel>().ReverseMap();
            CreateMap<EditTrainingDto, EditTrainingViewModel>().ReverseMap();
            CreateMap<TrainerHistoryDto, TrainingHistoryModel>().ReverseMap();
            CreateMap<TrainerTrainedDto, TrainerTrainedViewModel>().ReverseMap();
            CreateMap<NewsDto, NewsViewModel>().ReverseMap();
            
            // Person-related mappings
            CreateMap<PersonDetailsDto, PersonViewModel>().ReverseMap();
            CreateMap<AddPersonDto, AddPersonViewModel>().ReverseMap();
            CreateMap<EditPersonDto, EditPersonViewModel>().ReverseMap();
            CreateMap<PersonLookupDto, PersonViewModel>().ReverseMap();
        }
    }
}
